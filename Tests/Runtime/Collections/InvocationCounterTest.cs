using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Collections {
    public class InvocationCounterTest {

        [Test]
        public void ShouldInitializeToZero() {
            var sut = new InvocationCounter();

            Assert.Zero(sut.InvocationCount("example"));
        }

        [Test]
        public void ShouldCorrectlyRecordInvocations() {
            var sut = new InvocationCounter();

            sut.RecordInvocation("example");
            Assert.AreEqual(1, sut.InvocationCount("example"));

            sut.RecordInvocation("example");
            Assert.AreEqual(2, sut.InvocationCount("example"));
        }

        [Test]
        public void ShouldCorrectlyRecordArguments() {
            var sut = new InvocationCounter();

            sut.RecordInvocation("example", "argument");
            Assert.AreEqual("argument", sut.Arguments("example", 0)[0]);
        }

        [Test]
        public void ShouldProvideInvocationForMethodInfo() {
            var sut = new InvocationCounter();

            sut.RecordInvocation(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction)).Name);
            Assert.AreEqual(1, sut.InvocationCount(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction))));

            sut.RecordInvocation(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction)).Name);
            Assert.AreEqual(2, sut.InvocationCount(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction))));
        }
    }
}
