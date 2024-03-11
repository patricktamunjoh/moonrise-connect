using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Functions;
using Moq;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories
{
    internal static class NetworkConnectionFactory
    {
        public static NetworkConnection Build()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var queue = NetworkFunctionQueueFactory.BuildMock();

            return new NetworkConnection(strategy.Object, queue.Object);
        }

        public static NetworkConnection Build(NetworkConnectionStrategy strategy)
        {
            var queue = NetworkFunctionQueueFactory.BuildMock();
            var connection = new NetworkConnection(strategy, queue.Object);

            strategy.Connection = connection;

            return connection;
        }

        public static NetworkConnection Build(NetworkFunctionQueue queue)
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            return new NetworkConnection(strategy.Object, queue);
        }

        public static Mock<NetworkConnection> BuildMock(out NetworkFunctionRegistry registry, out NetworkFunctionQueue queue)
        {
            registry = new NetworkFunctionRegistry();
            queue = new NetworkFunctionQueue(registry);

            return BuildMock(queue);
        }

        public static Mock<NetworkConnection> BuildMock(NetworkFunctionQueue queue)
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            return new Mock<NetworkConnection>(strategy.Object, queue);
        }

        public static Mock<NetworkConnection> BuildMock(NetworkConnectionStrategy strategy)
        {
            var queue = NetworkFunctionQueueFactory.BuildMock();
            var connection = new Mock<NetworkConnection>(strategy, queue.Object);

            strategy.Connection = connection.Object;

            return connection;
        }

        public static Mock<NetworkConnection> BuildMock()
        {
            var strategy = new Mock<NetworkConnectionStrategy>();
            var queue = NetworkFunctionQueueFactory.BuildMock();

            return new Mock<NetworkConnection>(strategy.Object, queue.Object);
        }
    }
}
