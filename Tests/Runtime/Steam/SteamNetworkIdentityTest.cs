using MoonriseGames.Connect.Steam;
using Moq;
using NUnit.Framework;
using Steamworks;

namespace MoonriseGames.Connect.Tests.Steam
{
    public class SteamNetworkIdentityTest
    {
        [Test]
        public void ShouldProvideDisplayName()
        {
            var steamId = new CSteamID(98234);
            var proxy = new Mock<SteamProxy>();

            proxy.Setup(x => x.DisplayName(steamId)).Returns("example");
            SteamProxy.Instance = proxy.Object;

            var sut = new SteamNetworkIdentity(steamId);

            Assert.AreEqual("example", sut.DisplayName);
        }

        [Test]
        public void ShouldCastToSteamId()
        {
            const uint id = 2876U;
            var sut = new SteamNetworkIdentity(id);

            Assert.AreEqual(id, ((CSteamID)sut).m_SteamID);
        }

        [Test]
        public void ShouldCastToSteamNetworkingId()
        {
            var steamId = new CSteamID(27023);
            var sut = new SteamNetworkIdentity(steamId);

            Assert.AreEqual(steamId, ((SteamNetworkingIdentity)sut).GetSteamID());
        }

        [Test]
        public void ShouldCastFromSteamId()
        {
            var steamId = new CSteamID(27023);
            var sut = (SteamNetworkIdentity)steamId;

            Assert.AreEqual(steamId, (CSteamID)sut);
        }

        [Test]
        public void ShouldCastFromLongId()
        {
            const uint id = 2876U;
            var sut = (SteamNetworkIdentity)id;

            Assert.AreEqual(id, ((CSteamID)sut).m_SteamID);
        }

        [Test]
        public void ShouldEqualIdentityWithEqualId()
        {
            var a = new SteamNetworkIdentity(8246);
            var b = new SteamNetworkIdentity(8246);

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldNotEqualNull()
        {
            var sut = new SteamNetworkIdentity(2367);

            Assert.False(sut.Equals(null));
        }

        [Test]
        public void ShouldNotEqualIdentityWithDifferentId()
        {
            var a = new SteamNetworkIdentity(8246);
            var b = new SteamNetworkIdentity(9937);

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideSameHashCodeForEqualIdentities()
        {
            var a = new SteamNetworkIdentity(8246);
            var b = new SteamNetworkIdentity(8246);

            Assert.True(a.GetHashCode().Equals(b.GetHashCode()));
        }
    }
}
