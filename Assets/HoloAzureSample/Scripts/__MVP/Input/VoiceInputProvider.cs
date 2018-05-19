using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UniRx;

namespace HoloAzureSample.SpeechToText.MVP
{
    /// <summary>
    /// 音声録音UIを管理するクラス
    /// Azureへ渡す音声の入力方法をいくつか作るなら、インタフェースを定義して切り替えられるようにしておくとよいかも
    /// </summary>
    public class VoiceInputProvider : MonoBehaviour, IHoldHandler
    {
        /// <summary>
        /// 録音イベント種別
        /// </summary>
        public enum RecordingEvent
        {
            Start,
            Complete,
            Cancel,
            DisabledMic
        }

        // *************************************
        // Private member
        // *************************************

        /// <summary>
        /// マイクの接続状態
        /// </summary>
        private bool _isMicConnected = false;

        /// <summary>
        /// 録音時の最小周波数
        /// </summary>
        private int _minFreq;

        /// <summary>
        /// 録音時の最大周波数
        /// </summary>
        private int _maxFreq;

        private readonly BehaviorSubject<RecordingEvent> _recordingEventEventSubject = new BehaviorSubject<RecordingEvent>(RecordingEvent.Complete);
        private readonly Subject<byte[]> _recordDataSubject = new Subject<byte[]>();
        
        [SerializeField] private AudioSource _audioSource;

        // *************************************
        // Public Property
        // *************************************

        /// <summary>
        /// 録音イベント発生時のタイミングでイベント内容を流すストリーム
        /// </summary>
        public IObservable<RecordingEvent> RecordingEventStream => _recordingEventEventSubject;

        /// <summary>
        /// 録音したデータが流れるストリーム
        /// </summary>
        public IObservable<byte[]> RecordDataStream => _recordDataSubject;

        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);

            if (Microphone.devices.Length <= 0)
            {
                // マイクがないことを通知
                _recordingEventEventSubject.OnNext(RecordingEvent.DisabledMic);
            }
            else
            {
                _isMicConnected = true;

                // マイクが接続されているフラグを設定
                _isMicConnected = true;

                // デフォルトマイクの周波数の範囲を取得する
                Microphone.GetDeviceCaps(null, out _minFreq, out _maxFreq);

                //minFreq や maxFreq 引数で 0 の値が返されるとデバイスが任意の周波数をサポートすることを示す
                if (_minFreq == 0 && _maxFreq == 0)
                {
                    // 録音サンプリングレートを設定する
                    _maxFreq = 16000;
                }
                
            }
        }

        /// <summary>
        /// ホールドの開始
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldStarted(HoldEventData eventData)
        {
            if (!_isMicConnected)
            {
                return;
            }

            _recordingEventEventSubject.OnNext(RecordingEvent.Start);

            if (!Microphone.IsRecording(null))
            {
                _audioSource.clip = Microphone.Start(null, true, 3, _maxFreq);
            }
        }

        /// <summary>
        /// ホールドの完了
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCompleted(HoldEventData eventData)
        {
            // 録音の終了を通知
            _recordingEventEventSubject.OnNext(RecordingEvent.Complete);

            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
                string filePath;
                var bytes = WavUtility.FromAudioClip(_audioSource.clip, out filePath, false);
                _recordDataSubject.OnNext(bytes);
            }
        }

        /// <summary>
        /// ホールドのキャンセル
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCanceled(HoldEventData eventData)
        {
            // 録音の中断
            _recordingEventEventSubject.OnNext(RecordingEvent.Cancel);

            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
            }
        }
    }
}
