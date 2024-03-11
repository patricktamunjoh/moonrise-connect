using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions
{
    public class RecipientsExtensionsTest
    {
        [Test]
        public void ShouldOnlyContainHostInHost()
        {
            Assert.True(Recipients.Host.Contains(Roles.Host, true));
            Assert.True(Recipients.Host.Contains(Roles.Host, false));

            Assert.False(Recipients.Host.Contains(Roles.Client, true));
            Assert.False(Recipients.Host.Contains(Roles.Client, false));
        }

        [Test]
        public void ShouldOnlyContainClientInClients()
        {
            Assert.True(Recipients.Clients.Contains(Roles.Client, true));
            Assert.True(Recipients.Clients.Contains(Roles.Client, false));

            Assert.False(Recipients.Clients.Contains(Roles.Host, true));
            Assert.False(Recipients.Clients.Contains(Roles.Host, false));
        }

        [Test]
        public void ShouldContainAllExceptSenderInOthers()
        {
            Assert.True(Recipients.Others.Contains(Roles.Client, false));
            Assert.True(Recipients.Others.Contains(Roles.Host, false));

            Assert.False(Recipients.Others.Contains(Roles.Client, true));
            Assert.False(Recipients.Others.Contains(Roles.Host, true));
        }

        [Test]
        public void ShouldContainAllInAll()
        {
            Assert.True(Recipients.All.Contains(Roles.Client, true));
            Assert.True(Recipients.All.Contains(Roles.Client, false));
            Assert.True(Recipients.All.Contains(Roles.Host, true));
            Assert.True(Recipients.All.Contains(Roles.Host, false));
        }
    }
}
