using System.Collections.Generic;
using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Enums;

namespace MoonriseGames.Connect.Tests.Utilities.Connection
{
    internal class TestNetworkLink : NetworkLink
    {
        public TestNetworkLink ConnectedLink { get; set; }
        private TestNetworkConnectionStrategy Strategy { get; }

        private Queue<byte[]> ReceivedMessages { get; } = new();

        public TestNetworkLink(TestNetworkIdentity identity, TestNetworkConnectionStrategy strategy)
            : base(identity) => Strategy = strategy;

        public override void Send(byte[] data, Transmission transmission)
        {
            if (!IsActive || ConnectedLink == null)
                return;
            ConnectedLink.Receive(data);
        }

        private void Receive(byte[] data)
        {
            if (!IsActive)
                return;
            ReceivedMessages.Enqueue(data);
        }

        public override byte[] Receive()
        {
            if (ReceivedMessages.Count == 0)
                return null;
            return ReceivedMessages.Dequeue();
        }

        public override void Close()
        {
            var wasActive = IsActive;
            base.Close();

            if (!wasActive)
                return;
            Strategy.Connection?.HandleConnectionDisrupted(Identity);
            ConnectedLink?.Close();
        }
    }
}
