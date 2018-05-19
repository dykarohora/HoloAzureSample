using System.Threading.Tasks;

namespace HoloAzureSample.SpeechToText.MVP
{
    /// <summary>
    /// SpeechToTextUsecaseにはSpeechToTextApiしか使わせたくないのでインタフェースを定義
    /// </summary>
    public interface IUsefulSpeechToTextApi
    {
        Task<string> GetTextFromAudioClipAsync(byte[] bytes);
    }
}
