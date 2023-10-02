using MoonriseGames.CloudsAhoyConnect.Functions;
using Moq;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories {
    internal static class NetworkFunctionQueueFactory {

        public static Mock<NetworkFunctionQueue> BuildMock() {
            var registry = new Mock<NetworkFunctionRegistry>();
            return new Mock<NetworkFunctionQueue>(registry.Object);
        }
    }
}
