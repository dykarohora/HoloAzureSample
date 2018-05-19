using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace HoloAzureSample.SpeechToText.MVP
{

    public class AzureApiGateway : IUsefulSpeechToTextApi
    {
        public readonly string Error = "Error";

        private readonly string _subscriptionKey;

        // 以下のパラメータを変更したい場合は
        // Subscriptionキーと同様にコンストラクタで設定するようにし、
        // DIコンテナ側で切り替えられるようにしておくといいかも
        private string _recognitionMode = "interactive";
        private string _lang = "ja-JP";
        private string _format = "simple";

        private const string _endPoint = @"https://speech.platform.bing.com/speech/recognition/";

        private bool _isRequest = false;

        /// <summary>
        /// コンストラクタ、ZenJectからコールされる
        /// </summary>
        /// <param name="subscriptionKey"></param>
        public AzureApiGateway(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// オーディオのバイナリをAzureに渡して、その結果をJSONで得る（非同期）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public async Task<string> GetTextFromAudioClipAsync(byte[] bytes)
        {
            if (_isRequest)
            {
                // 本来はエラー仕様をちゃんと設計して返すべき
                return Error;
            }

            _isRequest = true;

            var requestUrl = $@"{_endPoint}{_recognitionMode}/cognitiveservices/v1?language={_lang}&format={_format}";

            var form = new WWWForm();
            form.AddBinaryData("file", bytes, "voice.wav", @"application/octet-stream");

            var headers = new Dictionary<string, string>
            {
                {"Ocp-Apim-Subscription-Key", _subscriptionKey},
            };

            try
            {
                // Azureへのアクセスは別スレッドで行う
                var resultText = await ObservableWWW.Post(requestUrl, form, headers);
                return resultText;
            }
            catch (WWWErrorException ex)
            {
                Debug.Log(ex.StatusCode);
                Debug.Log(ex.RawErrorMessage);
                Debug.Log(ex.Message);
                Debug.Log(ex.Text);
                // 本来はエラー仕様をちゃんと設計して返すべき
                return Error;
            }
            finally
            {
                _isRequest = false;
            }
        }
    }
}