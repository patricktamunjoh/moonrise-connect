using System;
using System.Collections.Generic;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Payloads;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Functions
{
    public class NetworkFunctionEmitterTest
    {
        private void VerifyFunctionIdsSent(Mock<NetworkConnection> connection, ulong objectId, NetworkHash functionId)
        {
            connection.Verify(x => x.Send(It.Is<NetworkFunctionCall>(y => y.ObjectId == objectId && y.FunctionId.Equals(functionId))));
        }

        private void VerifyPayloadSent(Mock<NetworkConnection> connection, NetworkFunctionDelegate function, IEnumerable<object> args)
        {
            connection.Verify(x => x.Send(It.Is<NetworkFunctionCall>(call => args.SequenceEqual(call.DecodedPayload(function.Data.PayloadType).Arguments()))));
        }

        private void VerifyDelegateEnqueued(Mock<NetworkFunctionQueue> queue, NetworkFunctionDelegate function, IEnumerable<object> args) =>
            queue.Verify(x => x.EnqueueDelegate(function, It.Is<INetworkPayload>(payload => args.SequenceEqual(payload.Arguments())), It.IsAny<Roles>(), true));

        private object[] ExpectedArguments(NetworkFunctionDelegate function, NetworkFunctionRegistry registry, object[] args) =>
            function.Data.PayloadType.CreateInstanceFromArguments(registry, args).Arguments().ToArray();

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithNoArguments()
        {
            var sample = new SampleNetwork();
            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            sut.Call(sample.NiladicFunction);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.NiladicFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);

            connection.Verify(x => x.Send(It.Is<NetworkFunctionCall>(y => y.ObjectId == objectId && y.FunctionId.Equals(functionId))));

            queue.Verify(x => x.EnqueueDelegate(functionDelegate, null, It.IsAny<Roles>(), true));
        }

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithOneArgument()
        {
            var sample = new SampleNetwork();
            var arguments = new object[] { 12 };

            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            sut.Call(sample.MonadicFunction, (int)arguments[0]);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.MonadicFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);
            var expectedArguments = ExpectedArguments(functionDelegate, registry, arguments);

            VerifyFunctionIdsSent(connection, objectId, functionId);
            VerifyPayloadSent(connection, functionDelegate, expectedArguments);
            VerifyDelegateEnqueued(queue, functionDelegate, expectedArguments);
        }

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithNetworkObjectArgument()
        {
            var sample = new SampleNetwork();
            var arguments = new object[] { new SampleNetworkEmpty() };

            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            registry.RegisterObject(arguments[0]);

            sut.Call(sample.NetworkObjectFunction, (SampleNetworkEmpty)arguments[0]);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.NetworkObjectFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);
            var expectedArguments = ExpectedArguments(functionDelegate, registry, arguments);

            VerifyFunctionIdsSent(connection, objectId, functionId);
            VerifyPayloadSent(connection, functionDelegate, expectedArguments);
            VerifyDelegateEnqueued(queue, functionDelegate, expectedArguments);
        }

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithTwoArguments()
        {
            var sample = new SampleNetwork();
            var arguments = new object[] { 12, 42 };

            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            sut.Call(sample.DyadicFunction, (int)arguments[0], (int)arguments[1]);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.DyadicFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);
            var expectedArguments = ExpectedArguments(functionDelegate, registry, arguments);

            VerifyFunctionIdsSent(connection, objectId, functionId);
            VerifyPayloadSent(connection, functionDelegate, expectedArguments);
            VerifyDelegateEnqueued(queue, functionDelegate, expectedArguments);
        }

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithThreeArguments()
        {
            var sample = new SampleNetwork();
            var arguments = new object[] { 12, 42, 102 };

            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            sut.Call(sample.TriadicFunction, (int)arguments[0], (int)arguments[1], (int)arguments[2]);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.TriadicFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);
            var expectedArguments = ExpectedArguments(functionDelegate, registry, arguments);

            VerifyFunctionIdsSent(connection, objectId, functionId);
            VerifyPayloadSent(connection, functionDelegate, expectedArguments);
            VerifyDelegateEnqueued(queue, functionDelegate, expectedArguments);
        }

        [Test]
        public void ShouldCorrectlyProcessNetworkCallWithFourArguments()
        {
            var sample = new SampleNetwork();
            var arguments = new object[] { 12, 42, 102, 1982 };

            var registry = new NetworkFunctionRegistry();
            var queue = new Mock<NetworkFunctionQueue>(registry);
            var connection = NetworkConnectionFactory.BuildMock(queue.Object);

            var sut = new NetworkFunctionEmitter(queue.Object, registry, connection.Object);

            registry.RegisterObject(sample);
            sut.Call(sample.QuadradicFunction, (int)arguments[0], (int)arguments[1], (int)arguments[2], (int)arguments[3]);

            var objectId = registry.GetRegisteredObjectId(sample);
            var functionId = NetworkHashing.Hash(sample.GetType().GetDeclaredMethod(nameof(SampleNetwork.QuadradicFunction)));
            var functionDelegate = registry.GetRegisteredFunctionDelegate(objectId, functionId);
            var expectedArguments = ExpectedArguments(functionDelegate, registry, arguments);

            VerifyFunctionIdsSent(connection, objectId, functionId);
            VerifyPayloadSent(connection, functionDelegate, expectedArguments);
            VerifyDelegateEnqueued(queue, functionDelegate, expectedArguments);
        }

        [Test]
        public void ShouldThrowIfFunctionIsNull()
        {
            var connection = NetworkConnectionFactory.BuildMock(out var registry, out var queue);

            var sut = new NetworkFunctionEmitter(queue, registry, connection.Object);

            Assert.Throws<ArgumentNullException>(() => sut.Call(null));

            Assert.Throws<ArgumentNullException>(() => sut.Call(null, 12));
            Assert.Throws<ArgumentNullException>(() => sut.Call(null, 12, 42));
            Assert.Throws<ArgumentNullException>(() => sut.Call(null, 12, 42, 102));
            Assert.Throws<ArgumentNullException>(() => sut.Call(null, 12, 42, 102, 3));
        }
    }
}
