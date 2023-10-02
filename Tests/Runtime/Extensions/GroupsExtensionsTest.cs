using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions {
    public class GroupsExtensionsTest {

        [Test]
        public void ShouldOnlyContainHostInHost() {
            Assert.True(Groups.Host.Contains(Roles.Host));
            Assert.True(Groups.Host.Contains(Roles.Host));

            Assert.False(Groups.Host.Contains(Roles.Client));
            Assert.False(Groups.Host.Contains(Roles.Client));
        }

        [Test]
        public void ShouldOnlyContainClientInClients() {
            Assert.True(Groups.Clients.Contains(Roles.Client));
            Assert.True(Groups.Clients.Contains(Roles.Client));

            Assert.False(Groups.Clients.Contains(Roles.Host));
            Assert.False(Groups.Clients.Contains(Roles.Host));
        }

        [Test]
        public void ShouldContainAllInAll() {
            Assert.True(Groups.All.Contains(Roles.Client));
            Assert.True(Groups.All.Contains(Roles.Client));
            Assert.True(Groups.All.Contains(Roles.Host));
            Assert.True(Groups.All.Contains(Roles.Host));
        }
    }
}
