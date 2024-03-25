using System;
using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Hashing;
using MoonriseGames.Connect.Payloads;
using MoonriseGames.Connect.Tests.Samples.Object;
using MoonriseGames.Connect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Functions
{
    public class NetworkFunctionQueueTest
    {
        [Test]
        public void ShouldProcessCalls()
        {
            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction));

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, false);
            sut.ProcessQueuedElements();

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldImmediatelyProcessNonDeferredCallsFromSender()
        {
            var sample = new SampleBase();
            var function = new NetworkFunction(Groups.All, Recipients.All);
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction), function);

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, true);

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldNotImmediatelyProcessNonDeferredCallsFromNetwork()
        {
            var sample = new SampleBase();
            var function = new NetworkFunction(Groups.All, Recipients.All);
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction), function);

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, false);

            Assert.Zero(sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldImmediatelyProcessCallsOnlyOnce()
        {
            var sample = new SampleBase();
            var function = new NetworkFunction(Groups.All, Recipients.All);
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction), function);

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, true);
            sut.ProcessQueuedElements();

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldProcessCallsWithArguments()
        {
            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.MonadicFunction));
            var payload = new NetworkPayload<string, object, object, object>("example");

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, payload, Roles.Host, false);
            sut.ProcessQueuedElements();

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.MonadicFunction)));
            Assert.AreEqual("example", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 0)[0]);
        }

        [Test]
        public void ShouldProcessAllCalls()
        {
            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction));

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, false);
            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, false);
            sut.EnqueueDelegate(functionDelegate, null, Roles.Host, false);

            sut.ProcessQueuedElements();

            Assert.AreEqual(3, sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldProcessQueuedDelegatesInSameOrder()
        {
            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.MonadicFunction));
            var payload1 = new NetworkPayload<string, object, object, object>("A");
            var payload2 = new NetworkPayload<string, object, object, object>("B");

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueDelegate(functionDelegate, payload1, Roles.Host, false);
            sut.EnqueueDelegate(functionDelegate, payload2, Roles.Host, false);

            sut.ProcessQueuedElements();

            Assert.AreEqual("A", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 0)[0]);
            Assert.AreEqual("B", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 1)[0]);
        }

        [Test]
        public void ShouldProcessQueuedElementsInSameOrder()
        {
            const ulong objectId = 12;

            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.MonadicFunction));
            var payload1 = new NetworkPayload<string, object, object, object>("A");
            var payload2 = new NetworkPayload<string, object, object, object>("B");

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            call.EncodePayload(payload2);

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(functionDelegate);

            sut.EnqueueDelegate(functionDelegate, payload1, Roles.Host, false);
            sut.EnqueueCall(call, Roles.Host, false);

            sut.ProcessQueuedElements();

            Assert.AreEqual("A", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 0)[0]);
            Assert.AreEqual("B", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 1)[0]);
        }

        [Test]
        public void ShouldProcessQueuedNetworkCalls()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction));

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(functionDelegate);

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);
            sut.ProcessQueuedElements();

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.NiladicFunction)));
        }

        [Test]
        public void ShouldNotAccessRegistryForEnqueuingCall()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var registry = new Mock<NetworkFunctionRegistry>();
            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);

            registry.Verify(x => x.GetRegisteredFunctionDelegate(It.IsAny<ulong>(), It.IsAny<NetworkHash>()), Times.Never);
        }

        [Test]
        public void ShouldOnlyAccessRegistryOnceForProcessingCall()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.NiladicFunction));

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(functionDelegate);

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);
            sut.ProcessQueuedElements();

            registry.Verify(x => x.GetRegisteredFunctionDelegate(objectId, functionId), Times.Once);
        }

        [Test]
        public void ShouldProcessQueuedNetworkCallsWithPayload()
        {
            const ulong objectId = 12;

            var payload = new NetworkPayload<string, object, object, object>("example");
            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            call.EncodePayload(payload);

            var sample = new SampleBase();
            var functionDelegate = NetworkFunctionDelegateFactory.Build(sample, nameof(SampleBase.MonadicFunction));

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(functionDelegate);

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);
            sut.ProcessQueuedElements();

            Assert.AreEqual(1, sample.InvocationCounter.InvocationCount(nameof(SampleBase.MonadicFunction)));
            Assert.AreEqual("example", sample.InvocationCounter.Arguments(nameof(SampleBase.MonadicFunction), 0)[0]);
        }

        [Test]
        public void ShouldInitializeAsEmpty()
        {
            var sut = new NetworkFunctionQueue(null);

            Assert.True(sut.IsEmpty);
        }

        [Test]
        public void ShouldBeEmptyIfAllElementsWereProcessed()
        {
            var function = new NetworkFunction(Groups.All, Recipients.All);
            var element = NetworkFunctionDelegateFactory.Build(function);

            var sut = new NetworkFunctionQueue(null);

            sut.EnqueueDelegate(element, null, Roles.Host, false);
            sut.ProcessQueuedElements();

            Assert.True(sut.IsEmpty);
        }

        [Test]
        public void ShouldNotBeEmptyWhenElementsAreQueued()
        {
            var function = new NetworkFunction(Groups.All, Recipients.All);
            var element = NetworkFunctionDelegateFactory.Build(function);

            var sut = new NetworkFunctionQueue(null);

            sut.EnqueueDelegate(element, null, Roles.Host, false);

            Assert.False(sut.IsEmpty);
        }

        [Test]
        public void ShouldQueueNetworkCallsForClearedFunctions()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(null as NetworkFunctionDelegate);

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);

            Assert.False(sut.IsEmpty);
        }

        [Test]
        public void ShouldNotThrowWhenProcessingNetworkCallsForClearedFunctions()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Returns(null as NetworkFunctionDelegate);

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);

            Assert.DoesNotThrow(() => sut.ProcessQueuedElements());
        }

        [Test]
        public void ShouldNotThrowWhenFailingToFindDelegateAsSenderWhileEnqueuing()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Unreliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Throws<ArgumentException>();

            var sut = new NetworkFunctionQueue(registry.Object);

            Assert.DoesNotThrow(() => sut.EnqueueCall(call, Roles.Host, true));
        }

        [Test]
        public void ShouldThrowWhenFailingToFindDelegateAsSenderWhileProcessing()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Unreliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Throws<ArgumentException>();

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, true);

            Assert.Throws<ArgumentException>(() => sut.ProcessQueuedElements());
        }

        [Test]
        public void ShouldNotThrowWhenFailingToFindDelegateForUnreliableCallAsReceiverWhileProcessing()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Unreliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Throws<ArgumentException>();

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);

            Assert.DoesNotThrow(() => sut.ProcessQueuedElements());
        }

        [Test]
        public void ShouldThrowWhenFailingToFindDelegateForReliableCallAsReceiverWhileProcessing()
        {
            const ulong objectId = 12;

            var functionId = NetworkHashing.Hash("example");
            var call = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredFunctionDelegate(objectId, functionId)).Throws<ArgumentException>();

            var sut = new NetworkFunctionQueue(registry.Object);

            sut.EnqueueCall(call, Roles.Host, false);

            Assert.Throws<ArgumentException>(() => sut.ProcessQueuedElements());
        }

        [Test]
        public void ShouldOnlyQueueElementsThatShouldBeExecuted()
        {
            var function = new NetworkFunction(Groups.All, Recipients.Clients);
            var element = NetworkFunctionDelegateFactory.Build(function);

            var sut = new NetworkFunctionQueue(null);

            sut.EnqueueDelegate(element, null, Roles.Host, true);

            Assert.True(sut.IsEmpty);
        }

        [Test]
        public void ShouldThrowWhenQueuingNullDelegate()
        {
            var payload = new NetworkPayload<int, int, int, int>(12);
            var sut = new NetworkFunctionQueue(null);

            Assert.Throws<ArgumentNullException>(() => sut.EnqueueDelegate(null, payload, Roles.Client, true));
        }

        [Test]
        public void ShouldThrowWhenQueuingNullNetworkCall()
        {
            var sut = new NetworkFunctionQueue(null);

            Assert.Throws<ArgumentNullException>(() => sut.EnqueueCall(null, Roles.Client, true));
        }
    }
}
