using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions {
    public class InvocationExtensionsTest {

        [Test]
        public void ShouldNotThrowIfInstanceIsNull() {
            var sample = new SampleNetwork();

            CloudsAhoyConnect.Instance = null;

            Assert.DoesNotThrow(() => this.Send(sample.NiladicFunction));
            Assert.DoesNotThrow(() => 12.Send(sample.MonadicFunction));
            Assert.DoesNotThrow(() => (12, 42).Send(sample.DyadicFunction));
            Assert.DoesNotThrow(() => (12, 42, 30).Send(sample.TriadicFunction));
            Assert.DoesNotThrow(() => (12, 42, 30, 55).Send(sample.QuadradicFunction));
        }

        [Test]
        public void ShouldSendNiladicFunction() {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            CloudsAhoyConnect.Instance = cac;

            this.Send(sample.NiladicFunction);

            emitter.Verify(x => x.Call(sample.NiladicFunction));
        }

        [Test]
        public void ShouldSendMonadicFunction() {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            CloudsAhoyConnect.Instance = cac;

            12.Send(sample.MonadicFunction);

            emitter.Verify(x => x.Call(sample.MonadicFunction, 12));
        }

        [Test]
        public void ShouldSendDyadicFunction() {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            CloudsAhoyConnect.Instance = cac;

            (12, 42).Send(sample.DyadicFunction);

            emitter.Verify(x => x.Call(sample.DyadicFunction, 12, 42));
        }

        [Test]
        public void ShouldSendTriadicFunction() {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            CloudsAhoyConnect.Instance = cac;

            (12, 42, 4).Send(sample.TriadicFunction);

            emitter.Verify(x => x.Call(sample.TriadicFunction, 12, 42, 4));
        }

        [Test]
        public void ShouldSendQuadradicFunction() {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            CloudsAhoyConnect.Instance = cac;

            (12, 42, 4, -4).Send(sample.QuadradicFunction);

            emitter.Verify(x => x.Call(sample.QuadradicFunction, 12, 42, 4, -4));
        }
    }
}
