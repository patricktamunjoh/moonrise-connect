using System;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Logging;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Logging
{
    public class SnapshotTest
    {
        [Test]
        public void ShouldRecordObjectRegistrations()
        {
            var sample = new Sample();
            var sut = new Snapshot();

            sut.RecordObjectRegistration(2, sample);
            sut.RecordObjectRegistration(3, sample);

            Assert.AreEqual(2, sut.ObjectRegistrations.Count);
            Assert.AreEqual((2ul, sample.GetType(), sample.ToString()), sut.ObjectRegistrations[0]);
            Assert.AreEqual((3ul, sample.GetType(), sample.ToString()), sut.ObjectRegistrations[1]);
        }

        [Test]
        public void ShouldRecordObjectUnregistrations()
        {
            var sample = new Sample();
            var sut = new Snapshot();

            sut.RecordObjectUnregistration(2, sample);
            sut.RecordObjectUnregistration(null, null, "example");

            Assert.AreEqual(2, sut.ObjectUnregistrations.Count);
            Assert.AreEqual((2ul as ulong?, sample.GetType(), sample.ToString(), null as string), sut.ObjectUnregistrations[0]);
            Assert.AreEqual((null as ulong?, null as Type, null as string, "example"), sut.ObjectUnregistrations[1]);
        }

        [Test]
        public void ShouldRecordOutgoingNetworkCalls()
        {
            var call1 = new NetworkFunctionCall(12, NetworkHashing.Hash("example A"), Transmission.Unreliable);
            var call2 = new NetworkFunctionCall(42, NetworkHashing.Hash("example B"), Transmission.Reliable);

            var identity = new Mock<NetworkIdentity>();
            identity.Setup(x => x.DisplayName).Returns("display name");

            var sut = new Snapshot();

            sut.RecordOutgoingNetworkCall(identity.Object, call1);
            sut.RecordOutgoingNetworkCall(identity.Object, call2);

            Assert.AreEqual(2, sut.OutgoingNetworkCalls.Count);
            Assert.AreEqual(("display name", call1.ObjectId, call1.FunctionId.ToBase64(), call1.Transmission), sut.OutgoingNetworkCalls[0]);
            Assert.AreEqual(("display name", call2.ObjectId, call2.FunctionId.ToBase64(), call2.Transmission), sut.OutgoingNetworkCalls[1]);
        }

        [Test]
        public void ShouldRecordIncomingNetworkCalls()
        {
            var call1 = new NetworkFunctionCall(12, NetworkHashing.Hash("example A"), Transmission.Unreliable);
            var call2 = new NetworkFunctionCall(42, NetworkHashing.Hash("example B"), Transmission.Reliable);

            var identity = new Mock<NetworkIdentity>();
            identity.Setup(x => x.DisplayName).Returns("display name");

            var sut = new Snapshot();

            sut.RecordIncomingNetworkCall(identity.Object, call1);
            sut.RecordIncomingNetworkCall(identity.Object, call2);

            Assert.AreEqual(2, sut.IncomingNetworkCalls.Count);
            Assert.AreEqual(("display name", call1.ObjectId, call1.FunctionId.ToBase64(), call1.Transmission), sut.IncomingNetworkCalls[0]);
            Assert.AreEqual(("display name", call2.ObjectId, call2.FunctionId.ToBase64(), call2.Transmission), sut.IncomingNetworkCalls[1]);
        }

        [Test]
        public void ShouldRecordNetworkEvents()
        {
            var identity = new Mock<NetworkIdentity>();
            identity.Setup(x => x.DisplayName).Returns("display name");

            var args1 = NetworkConnectionEventArgs.ForConnectionEstablished();
            var args2 = NetworkConnectionEventArgs.ForConnectionToClientLost(identity.Object);

            var sut = new Snapshot();

            sut.RecordNetworkEvent(args1);
            sut.RecordNetworkEvent(args2);

            Assert.AreEqual(2, sut.NetworkEvents.Count);
            Assert.AreEqual((args1.Type, null as string), sut.NetworkEvents[0]);
            Assert.AreEqual((args2.Type, "display name"), sut.NetworkEvents[1]);
        }
    }
}
