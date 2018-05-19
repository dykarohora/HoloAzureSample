using Zenject;
using UniRx;
using Debug = UnityEngine.Debug;

namespace HoloAzureSample.SpeechToText.MVP
{

    /// <summary>
    /// ViewとUsecaseを監視し、モデルを取得したり、それをビューに反映させたりする
    /// Viewロジック自体は持たせない
    /// </summary>
    public class SpeechToTextPresenter : IInitializable
    {
        // 依存関係はZenJectのフィールドインジェクションで依存性を解決している

        [Inject]
        private SpeechToTextUsecase _textUsecase;

        [Inject]
        private VoiceInputProvider _voiceInputProvider;

        [Inject]
        private SpeechToTextViewEx _view;

        public void Initialize()
        {
            _voiceInputProvider.RecordingEventStream
                .Subscribe(eventType =>
                {
                    switch (eventType)
                    {
                        case VoiceInputProvider.RecordingEvent.Start:
                            _view.SetStartRecordingMessage();
                            break;
                        case VoiceInputProvider.RecordingEvent.Complete:
                            _view.SetCompleteRecordingMessage();
                            break;
                        case VoiceInputProvider.RecordingEvent.Cancel:
                            _view.SetRecordingCanceledMessage();
                            break;
                        case VoiceInputProvider.RecordingEvent.DisabledMic:
                            break;
                    }
                });

            _voiceInputProvider.RecordDataStream
                .Subscribe(async bytes =>
                {
                    // 非同期で動く
                    var result = await _textUsecase.GetTextFromSpeech(bytes);

                    _view.ShowResponse(result);
                });
        }
    }
}


