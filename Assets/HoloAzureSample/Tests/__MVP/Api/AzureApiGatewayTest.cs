using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;

namespace HoloAzureSample.SpeechToText.MVP.Test
{
    public class AzureApiGatewayTest
    {
        /// <summary>
        /// SpeechToTextAPIにて音声認識が成功したときのゲートウェイのふるまいをテスト
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator GetTextFromCorrectAudioClip()
        {
            yield return new MonoBehaviourTest<GetTextFromCorrectAudioClip_ScenarioDriver>();
        }

        /// <summary>
        /// Unity TestRunnerでasync Taskなテストメソッドが使えないので苦肉の策…
        /// </summary>
        public class GetTextFromCorrectAudioClip_ScenarioDriver : MonoBehaviour, IMonoBehaviourTest
        {
            private string _subscriptionKey = "";
            public bool IsTestFinished {
                get; private set;
            }

            private async void Start()
            {
                var sut = new AzureApiGateway(_subscriptionKey);
                var bytes = File.ReadAllBytes(Application.persistentDataPath + "sample.mp4");
                var result = await sut.GetTextFromAudioClipAsync(bytes);

                Assert.That(result, Is.Not.Empty);
                Assert.That(result, Is.Not.EqualTo(sut.Error));

                IsTestFinished = true;
                gameObject.SetActive(false);
            }
        }
    }
}
