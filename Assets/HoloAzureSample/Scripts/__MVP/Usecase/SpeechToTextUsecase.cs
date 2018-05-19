using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HoloAzureSample.SpeechToText.MVP
{
    public class SpeechToTextUsecase
    {
        // APIゲートウェイへの参照
        private IUsefulSpeechToTextApi _apiGateway;

        /// <summary>
        /// 依存関係はZenJectを使い、コンストラクタインジェクションで解決する
        /// </summary>
        /// <param name="apiGateway"></param>
        public SpeechToTextUsecase(IUsefulSpeechToTextApi apiGateway)
        {
            _apiGateway = apiGateway;
        }

        // APIを叩いてエンティティを取得して、モデルに変換
        public async Task<string> GetTextFromSpeech(byte[] bytes)
        {
            var response = await _apiGateway.GetTextFromAudioClipAsync(bytes);

            if (response == "Error")
            {
                // 本来はエラー仕様に応じて返すべき
                return "Error";
            }

            var model = JsonConvert.DeserializeObject<STTSimpleResponse>(response);
            return model.DisplayText;
        }
    }
}
