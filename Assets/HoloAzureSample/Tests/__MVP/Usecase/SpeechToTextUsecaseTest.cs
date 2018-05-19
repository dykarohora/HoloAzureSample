using System;
using NUnit.Framework;
using Zenject;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HoloAzureSample.SpeechToText.MVP.Test
{
    /// <summary>
    /// Usecaseクラスのテスト。Editorテストでもよい。
    /// </summary>
    [TestFixture]
    public class SpeechToTextUsecaseTest : ZenjectUnitTestFixture
    {
        /// <summary>
        /// APIのスタブ。振る舞いはコンストラクト時に定義する。
        /// </summary>
        private class SpeechToTextApiStub : IUsefulSpeechToTextApi
        {
            // SpeechToTextApiのスタブメソッドのデリゲート
            private readonly Func<string> _sttStubFunc;

            public SpeechToTextApiStub(Func<string> sttStubFunc)
            {
                _sttStubFunc = sttStubFunc;
            }

            public async Task<string> GetTextFromAudioClipAsync(byte[] bytes)
            {
                return _sttStubFunc();
            }
        }

        /// <summary>
        /// 正しいレスポンスを返すAPIのスタブを持ったテスト対象クラスを作成する
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private SpeechToTextUsecase CreateSutWithReturnTextApiStub(InjectContext _)
        {
            return new SpeechToTextUsecase(
                new SpeechToTextApiStub(() =>
                {
                    var model = new STTSimpleResponse
                    {
                        RecognitionStatus = string.Empty,
                        DisplayText = "apple",
                        Offset = 1,
                        Duration = 1,
                    };

                    return JsonConvert.SerializeObject(model);
                }));
        }

        /// <summary>
        /// エラーレスポンスを返すAPIのスタブを持ったテスト対象クラスを作成する
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private SpeechToTextUsecase CreateSutWithErrorApiStub(InjectContext _)
        {
            return new SpeechToTextUsecase(
                new SpeechToTextApiStub(() => "Error"));
        }

        /// <summary>
        /// SUTのインジェクション時にどのスタブを設定するかを明示するラベル
        /// </summary>
        private enum StubType
        {
            ReturnText,
            Error
        }

        [SetUp]
        public void DIContainerSetup()
        {
            // 正しいレスポンスを返すAPIのスタブを持ったSUTをコンテナにバインド
            Container.Bind<SpeechToTextUsecase>()
                .WithId(StubType.ReturnText)
                .FromMethod(CreateSutWithReturnTextApiStub)
                .AsTransient();

            // エラーレスポンスを返すAPIのスタブを持ったSUTをコンテナにバインド
            Container.Bind<SpeechToTextUsecase>()
                .WithId(StubType.Error)
                .FromMethod(CreateSutWithErrorApiStub)
                .AsTransient();

            Container.Inject(this);
        }

        // *************
        // SUT
        // *************

        /// <summary>
        /// 正しいレスポンスを返すAPIのスタブを持つSUT
        /// </summary>
        [Inject(Id = StubType.ReturnText)] private SpeechToTextUsecase _sutWithReturnTextStub;
        
        /// <summary>
        /// エラーレスポンスを返すAPIのスタブを持つSUT
        /// </summary>
        [Inject(Id = StubType.Error)] private SpeechToTextUsecase _sutWithReturnErrorStub;

        /// <summary>
        /// APIが正しくレスポンスを返すときのUsecaseのふるまいをテスト
        /// </summary>
        [Test]
        public void GetTextWhenApiReturnCorrectResponse()
        {
            var rand = new Random();
            var bytes = new byte[10];
            rand.NextBytes(bytes);
            // 非同期メソッドをWaitするが、スタブは別スレッドで処理をさせてないのでデッドロックしない
            var text = _sutWithReturnTextStub.GetTextFromSpeech(bytes).Result;
            Assert.That(text, Is.EqualTo("apple"));
        }

        /// <summary>
        /// APIがエラーレスポンスを返すときのUsecaseのふるまいをテスト
        /// </summary>
        [Test]
        public void GetErrorWhenApiReturnErrorResponse()
        {
            var rand = new Random();
            var bytes = new byte[10];
            rand.NextBytes(bytes);
            // 非同期メソッドをWaitするが、スタブは別スレッドで処理をさせてないのでデッドロックしない
            var text = _sutWithReturnErrorStub.GetTextFromSpeech(bytes).Result;
            Assert.That(text, Is.EqualTo("Error"));
        }
    }
}
