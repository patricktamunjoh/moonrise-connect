using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Tests.Samples.Network;
using MoonriseGames.Connect.Tests.Utilities.Connection;
using MoonriseGames.Connect.Tests.Utilities.Factories;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Integration
{
    public class SendReceiveIntegrationTest
    {
        [SetUp]
        public void SetUp() => TestNetworkConnectionStrategy.Reset();

        private void CreateConnectedLibraryInstances(out Session a, out Session b)
        {
            var id1 = new TestNetworkIdentity("cac 01");
            var id2 = new TestNetworkIdentity("cac 02");

            a = SessionFactory.BuildForIntegrationTest(id1);
            b = SessionFactory.BuildForIntegrationTest(id2);

            a.EstablishConnection(new NetworkConnectionConfig(new NetworkIdentity[] { id2 }));
            b.EstablishConnection(new NetworkConnectionConfig(id1));
        }

        private void ProcessIncomingNetworkCalls(Session a, Session b)
        {
            a.PollConnection();
            b.PollConnection();

            a.ProcessQueuedNetworkFunctionCalls();
            b.ProcessQueuedNetworkFunctionCalls();
        }

        [Test]
        public void ShouldSendAndReceiveNiladicFunction()
        {
            var sample1 = new SampleNetwork();
            var sample2 = new SampleNetwork();

            CreateConnectedLibraryInstances(out var cac1, out var cac2);

            cac1.Registry.RegisterObject(sample1);
            cac2.Registry.RegisterObject(sample2);

            cac1.Emitter.Call(sample1.NiladicFunction);

            ProcessIncomingNetworkCalls(cac1, cac2);

            Assert.AreEqual(1, sample2.InvocationCounter.InvocationCount(nameof(sample2.NiladicFunction)));
            Assert.AreEqual(1, sample1.InvocationCounter.InvocationCount(nameof(sample2.NiladicFunction)));
        }

        [Test]
        public void ShouldSendAndReceiveNetworkObjectArgumentFunction()
        {
            var sample1 = new SampleNetwork();
            var sample2 = new SampleNetwork();

            var data1 = new SampleNetworkEmpty();
            var data2 = new SampleNetworkEmpty();

            CreateConnectedLibraryInstances(out var cac1, out var cac2);

            cac1.Registry.RegisterObject(sample1);
            cac2.Registry.RegisterObject(sample2);

            cac1.Registry.RegisterObject(data1);
            cac2.Registry.RegisterObject(data2);

            cac1.Emitter.Call(sample1.NetworkObjectFunction, data1);

            ProcessIncomingNetworkCalls(cac1, cac2);

            Assert.AreEqual(1, sample1.InvocationCounter.InvocationCount(nameof(sample2.NetworkObjectFunction)));
            Assert.AreEqual(1, sample2.InvocationCounter.InvocationCount(nameof(sample2.NetworkObjectFunction)));

            Assert.AreEqual(data1, sample1.InvocationCounter.Arguments(nameof(sample1.NetworkObjectFunction), 0)[0]);
            Assert.AreEqual(data2, sample2.InvocationCounter.Arguments(nameof(sample2.NetworkObjectFunction), 0)[0]);
        }
    }
}
