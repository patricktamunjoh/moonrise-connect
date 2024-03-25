using MoonriseGames.Connect.Connection;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Tests.Utilities.Connection;
using Moq;

namespace MoonriseGames.Connect.Tests.Utilities.Factories
{
    internal static class SessionFactory
    {
        public static Session BuildForIntegrationTest(TestNetworkIdentity id)
        {
            var registry = new NetworkFunctionRegistry();
            var queue = new NetworkFunctionQueue(registry);

            var strategy = new TestNetworkConnectionStrategy(id);
            var connection = new NetworkConnection(strategy, queue);
            var emitter = new NetworkFunctionEmitter(queue, registry, connection);

            return new Session(connection, queue, registry, emitter);
        }

        public static Session Build()
        {
            var connection = NetworkConnectionFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new Session(connection.Object, queue.Object, registry.Object, emitter.Object);
        }

        public static Session Build(NetworkFunctionEmitter emitter)
        {
            var connection = NetworkConnectionFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);

            return new Session(connection.Object, queue.Object, registry.Object, emitter);
        }

        public static Session Build(NetworkConnection connection)
        {
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new Session(connection, queue.Object, registry.Object, emitter.Object);
        }

        public static Session Build(NetworkFunctionRegistry registry)
        {
            var connection = NetworkConnectionFactory.BuildMock();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new Session(connection.Object, queue.Object, registry, emitter.Object);
        }

        public static Session Build(NetworkFunctionRegistry registry, NetworkConnection connection)
        {
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new Session(connection, queue.Object, registry, emitter.Object);
        }

        public static Session Build(NetworkFunctionQueue queue)
        {
            var connection = NetworkConnectionFactory.BuildMock();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();

            return new Session(connection.Object, queue, registry.Object, emitter.Object);
        }
    }
}
