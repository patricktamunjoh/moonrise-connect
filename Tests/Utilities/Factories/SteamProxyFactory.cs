using MoonriseGames.CloudsAhoyConnect.Steam;
using Moq;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories {
    internal static class SteamProxyFactory {

        public static Mock<SteamProxy> BuildMock() {
            var proxy = new Mock<SteamProxy>();

            SteamProxy.Instance = proxy.Object;
            return proxy;
        }
    }
}
