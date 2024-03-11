using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using NUnit.Framework;
using static MoonriseGames.CloudsAhoyConnect.Invocation;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions
{
    public class InvocationTest
    {
        [Test]
        public void ShouldNotThrowIfInstanceIsNull()
        {
            var sample = new SampleNetwork();

            Session.Instance = null;

            Assert.DoesNotThrow(() => Invoke(sample.NiladicFunction));
            Assert.DoesNotThrow(() => Invoke(sample.MonadicFunction, 12));
            Assert.DoesNotThrow(() => Invoke(sample.DyadicFunction, 12, 42));
            Assert.DoesNotThrow(() => Invoke(sample.TriadicFunction, 12, 42, 30));
            Assert.DoesNotThrow(() => Invoke(sample.QuadradicFunction, 12, 42, 30, 55));
        }

        [Test]
        public void ShouldSendNiladicFunction()
        {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            Session.Instance = cac;

            Invoke(sample.NiladicFunction);

            emitter.Verify(x => x.Call(sample.NiladicFunction));
        }

        [Test]
        public void ShouldSendMonadicFunction()
        {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            Session.Instance = cac;

            Invoke(sample.MonadicFunction, 12);

            emitter.Verify(x => x.Call(sample.MonadicFunction, 12));
        }

        [Test]
        public void ShouldSendDyadicFunction()
        {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            Session.Instance = cac;

            Invoke(sample.DyadicFunction, 12, 42);

            emitter.Verify(x => x.Call(sample.DyadicFunction, 12, 42));
        }

        [Test]
        public void ShouldSendTriadicFunction()
        {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            Session.Instance = cac;

            Invoke(sample.TriadicFunction, 12, 42, 4);

            emitter.Verify(x => x.Call(sample.TriadicFunction, 12, 42, 4));
        }

        [Test]
        public void ShouldSendQuadradicFunction()
        {
            var sample = new SampleNetwork();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var cac = CloudsAhoyConnectFactory.Build(emitter.Object);

            Session.Instance = cac;

            Invoke(sample.QuadradicFunction, 12, 42, 4, -4);

            emitter.Verify(x => x.Call(sample.QuadradicFunction, 12, 42, 4, -4));
        }
    }
}
