using MoonriseGames.Connect.Steam;
using Moq;

namespace MoonriseGames.Connect.Tests.Utilities.Factories
{
    internal static class SteamProxyFactory
    {
        public static Mock<SteamProxy> BuildMock()
        {
            var proxy = new Mock<SteamProxy>();

            SteamProxy.Instance = proxy.Object;
            return proxy;
        }
    }
}
