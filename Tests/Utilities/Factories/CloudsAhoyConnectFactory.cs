using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Connection;
using Moq;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories {
    internal static class CloudsAhoyConnectFactory {

        public static CloudsAhoyConnect BuildForIntegrationTest(TestNetworkIdentity id) {
            var registry = new NetworkFunctionRegistry();
            var queue = new NetworkFunctionQueue(registry);

            var strategy = new TestNetworkConnectionStrategy(id);
            var connection = new NetworkConnection(strategy, queue);
            var emitter = new NetworkFunctionEmitter(queue, registry, connection);

            return new CloudsAhoyConnect(connection, queue, registry, emitter);
        }

        public static CloudsAhoyConnect Build() {
            var connection = NetworkConnectionFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new CloudsAhoyConnect(connection.Object, queue.Object, registry.Object, emitter.Object);
        }

        public static CloudsAhoyConnect Build(NetworkFunctionEmitter emitter) {
            var connection = NetworkConnectionFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);

            return new CloudsAhoyConnect(connection.Object, queue.Object, registry.Object, emitter);
        }

        public static CloudsAhoyConnect Build(NetworkConnection connection) {
            var registry = new Mock<NetworkFunctionRegistry>();
            var queue = new Mock<NetworkFunctionQueue>(registry.Object);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new CloudsAhoyConnect(connection, queue.Object, registry.Object, emitter.Object);
        }

        public static CloudsAhoyConnect Build(NetworkFunctionRegistry registry) {
            var connection = NetworkConnectionFactory.BuildMock();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new CloudsAhoyConnect(connection.Object, queue.Object, registry, emitter.Object);
        }

        public static CloudsAhoyConnect Build(NetworkFunctionRegistry registry, NetworkConnection connection) {
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var emitter = NetworkFunctionEmitterFactory.BuildMock();

            return new CloudsAhoyConnect(connection, queue.Object, registry, emitter.Object);
        }

        public static CloudsAhoyConnect Build(NetworkFunctionQueue queue) {
            var connection = NetworkConnectionFactory.BuildMock();
            var emitter = NetworkFunctionEmitterFactory.BuildMock();
            var registry = new Mock<NetworkFunctionRegistry>();

            return new CloudsAhoyConnect(connection.Object, queue, registry.Object, emitter.Object);
        }
    }
}
