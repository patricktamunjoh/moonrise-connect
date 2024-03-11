using System;
using System.Collections.Generic;
using MoonriseGames.CloudsAhoyConnect.Connection;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Connection
{
    internal class TestNetworkConnectionStrategy : NetworkConnectionStrategy
    {
        private static Dictionary<TestNetworkIdentity, TestNetworkConnectionStrategy> Instances { get; } = new();

        private TestNetworkIdentity Identity { get; }
        private bool IsListeningForClientConnections { get; set; }

        public TestNetworkConnectionStrategy(TestNetworkIdentity identity)
        {
            Identity = identity;

            if (Instances.ContainsKey(identity))
                throw new ArgumentException();
            Instances[identity] = this;
        }

        public static void Reset() => Instances.Clear();

        public override void EstablishConnectionToHost(NetworkIdentity host)
        {
            if (host is not TestNetworkIdentity otherIdentity)
                throw new ArgumentException();

            if (!Instances.TryGetValue(otherIdentity, out var other) || !other.IsListeningForClientConnections)
            {
                Connection?.HandleConnectionEstablishmentFailed();
                return;
            }

            var linkToOther = new TestNetworkLink(otherIdentity, this);
            var linkToThis = new TestNetworkLink(Identity, other);

            linkToOther.ConnectedLink = linkToThis;
            linkToThis.ConnectedLink = linkToOther;

            Connection?.ReceiveNewActiveNetworkLink(linkToOther);
            other.Connection?.ReceiveNewActiveNetworkLink(linkToThis);
        }

        public override void StartListeningForClientConnections() => IsListeningForClientConnections = true;

        public override void StopListeningForClientConnections() => IsListeningForClientConnections = false;
    }
}
