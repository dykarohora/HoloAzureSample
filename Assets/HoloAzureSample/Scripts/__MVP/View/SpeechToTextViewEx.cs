using UnityEngine;

namespace HoloAzureSample.SpeechToText.MVP
{
    public class SpeechToTextViewEx : MonoBehaviour
    {
        [SerializeField] private TextMesh _debug;
        [SerializeField] private GameObject _messagePrefab;

        private Transform _cameraCache;

        private void Start()
        {
            _cameraCache = Camera.main.transform;
        }

        private void SetDebugMessage(string message)
        {
            _debug.text = message;
        }

        /// <summary>
        /// タップのホールドを開始したときの表示処理
        /// </summary>
        public void SetStartRecordingMessage()
        {
            _debug.color = new Color(1, 0, 0, 1);
            SetDebugMessage("Recording");
        }

        /// <summary>
        /// タップのホールドを終了したときの表示処理
        /// </summary>
        public void SetCompleteRecordingMessage()
        {
            _debug.color = new Color(1, 1, 1, 1);
            SetDebugMessage("タップ&ホールドで録音開始");
        }

        /// <summary>
        /// タップのホールドをキャンセルしたときの表示処理
        /// </summary>
        public void SetRecordingCanceledMessage()
        {
            _debug.color = new Color(1, 1, 1, 1);
            SetDebugMessage("タップ&ホールドで録音開始");
        }

        /// <summary>
        /// マイクが接続されていないときのメッセージ
        /// </summary>
        public void SetMicDisconnectedMessage()
        {
            _debug.color = new Color(1, 1, 1, 1);
            SetDebugMessage("Microphone not connected!");
        }


        /// <summary>
        /// 認識結果を表示する
        /// </summary>
        /// <param name="response"></param>
        public void ShowResponse(string response)
        {
            // カメラから距離10の位置にテキストを生成
            var ray = new Ray(_cameraCache.position, _cameraCache.rotation * Vector3.forward);
            var message = GameObject.Instantiate(_messagePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            message.transform.position = ray.GetPoint(10);
            message.transform.LookAt(_cameraCache);
            message.transform.Rotate(new Vector3(0, 180, 0));

            if (string.IsNullOrEmpty(response))
            {
                _debug.color = new Color(1, 0, 0, 1);
                SetDebugMessage("音声を認識できませんでした");
            }
            else
            {
                message.GetComponent<TextMesh>().text = response;
            }
        }

        /*
        public void ShowErrorResponse(string response)
        {
            _debug.color = new Color(1, 0, 0, 1);
            SetDebugMessage(response);
        }
        */
    }
}
