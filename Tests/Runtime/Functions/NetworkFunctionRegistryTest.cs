using System;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Hashing;
using MoonriseGames.Connect.Logging;
using MoonriseGames.Connect.Tests.Samples.Network;
using MoonriseGames.Connect.Tests.Samples.Object;
using MoonriseGames.Connect.Tests.Utilities.Extensions;
using MoonriseGames.Connect.Tests.Utilities.Functions;
using Moq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoonriseGames.Connect.Tests.Functions
{
    public class NetworkFunctionRegistryTest
    {
        private ulong FirstObjectId => NetworkFunctionRegistry.NULL_OBJECT_ID + 1;

        [SetUp]
        public void Setup() => Function.ClearScene();

        [Test]
        public void ShouldRegisterAllObjectsInScene()
        {
            var root1 = new GameObject("example A");
            var root2 = new GameObject("example B");

            var behaviour1 = root1.AddComponent<SampleBehaviour>();
            var behaviour2 = root1.AddComponent<SampleBehaviour>();
            var behaviour3 = root2.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterAllGameObjects();

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour1));
            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour2));
            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour3));
        }

        [Test]
        public void ShouldRegisterChildObjectsInScene()
        {
            var root1 = new GameObject("example A");
            var root2 = new GameObject("example B") { transform = { parent = root1.transform } };

            var behaviour = root2.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterAllGameObjects();

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour));
        }

        [Test]
        public void ShouldRegisterObjectsInAdditionalScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var root = new GameObject();
            var behaviour = root.AddComponent<SampleBehaviour>();

            SceneManager.MergeScenes(scene, SceneManager.GetActiveScene());
            SceneManager.MoveGameObjectToScene(root, scene);

            var sut = new NetworkFunctionRegistry();

            sut.RegisterAllGameObjects();

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour));
        }

        [Test]
        public void ShouldRegisterInSceneInAlphabeticalOrder()
        {
            var root1 = new GameObject("example B");
            var root2 = new GameObject("example A");

            var behaviour1 = root1.AddComponent<SampleBehaviour>();
            var behaviour2 = root2.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterAllGameObjects();

            Assert.True(sut.GetRegisteredObjectId(behaviour2) < sut.GetRegisteredObjectId(behaviour1));
        }

        [Test]
        public void ShouldMaintainIdsWhenRegisteringIdenticalScene()
        {
            var root1 = new GameObject("example B");
            var root2 = new GameObject("example A");

            var behaviour1 = root1.AddComponent<SampleBehaviour>();
            var behaviour2 = root2.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterAllGameObjects();

            var behaviourId1 = sut.GetRegisteredObjectId(behaviour1);
            var behaviourId2 = sut.GetRegisteredObjectId(behaviour2);

            Function.ClearScene();
            sut.ClearRegistrationsAndResetCounter();

            root2 = new GameObject("example A");
            root1 = new GameObject("example B");

            behaviour1 = root1.AddComponent<SampleBehaviour>();
            behaviour2 = root2.AddComponent<SampleBehaviour>();

            sut.RegisterAllGameObjects();

            Assert.AreEqual(behaviourId1, sut.GetRegisteredObjectId(behaviour1));
            Assert.AreEqual(behaviourId2, sut.GetRegisteredObjectId(behaviour2));
        }

        [Test]
        public void ShouldThrowIfObjectsHaveDuplicateNames()
        {
            var root1 = new GameObject("example");
            var root2 = new GameObject("example");

            root1.AddComponent<SampleBehaviour>();
            root2.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            Assert.Throws<InvalidOperationException>(() => sut.RegisterAllGameObjects());
        }

        [Test]
        public void ShouldNotThrowIfObjectsHaveDuplicateNamesOnDifferentPaths()
        {
            var root1 = new GameObject("example");
            var root2 = new GameObject("example B");
            var child = new GameObject("example") { transform = { parent = root1.transform } };

            root1.AddComponent<SampleBehaviour>();
            root2.AddComponent<SampleBehaviour>();
            child.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            Assert.DoesNotThrow(() => sut.RegisterAllGameObjects());
        }

        [Test]
        public void ShouldNotThrowIfObjectsHaveDuplicateNamesButAreNotNetworked()
        {
            new GameObject("example");
            new GameObject("example");

            var sut = new NetworkFunctionRegistry();

            Assert.DoesNotThrow(() => sut.RegisterAllGameObjects());
        }

        [Test]
        public void ShouldClearAllRegistrations()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);
            sut.ClearRegistrations();

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldClearAllRegistrationsAndResetCounter()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var objectId = sut.GetRegisteredObjectId(sample);

            sut.ClearRegistrationsAndResetCounter();

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(sample));
            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObject(objectId));

            sut.RegisterObject(sample);

            Assert.AreEqual(FirstObjectId, sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldUnregisterObject()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.True(sut.UnregisterObject(sample));
            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldNotUnregisterObjectNotRegistered()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            Assert.False(sut.UnregisterObject(sample));
        }

        [Test]
        public void ShouldThrowIfUnregisteringNull()
        {
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<ArgumentNullException>(() => sut.UnregisterObject(null));
        }

        [Test]
        public void ShouldRegisterOnlyParentGameObject()
        {
            var root = new GameObject();
            var child = new GameObject { transform = { parent = root.transform } };

            var behaviour1 = root.AddComponent<SampleBehaviour>();
            var behaviour2 = child.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            Assert.True(sut.RegisterGameObject(root, false));

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour1));
            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(behaviour2));
        }

        [Test]
        public void ShouldRegisterAllChildGameObjects()
        {
            var root = new GameObject();
            var child = new GameObject { transform = { parent = root.transform } };

            var behaviour1 = root.AddComponent<SampleBehaviour>();
            var behaviour2 = child.AddComponent<SampleBehaviour>();

            var sut = new NetworkFunctionRegistry();

            Assert.True(sut.RegisterGameObject(root, true));

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour1));
            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(behaviour2));
        }

        [Test]
        public void ShouldNotRegisterGameObjectsWithoutNetworkObjectComponents()
        {
            var root = new GameObject();
            var sut = new NetworkFunctionRegistry();

            Assert.False(sut.RegisterGameObject(root, false));
        }

        [Test]
        public void ShouldThrowIfRegisteringNullGameObject()
        {
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<ArgumentNullException>(() => sut.RegisterGameObject(null, false));
            Assert.Throws<ArgumentNullException>(() => sut.RegisterGameObject(null, true));
        }

        [Test]
        public void ShouldRegisterNetworkObjects()
        {
            var sample = new SampleNetworkEmpty();
            var sut = new NetworkFunctionRegistry();

            Assert.True(sut.RegisterObject(sample));
            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldRegisterInheritedNetworkObjects()
        {
            var sample = new SampleNetworkOnlyBase();
            var sut = new NetworkFunctionRegistry();

            Assert.True(sut.RegisterObject(sample));
            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldNotRegisterNoneNetworkObjects()
        {
            var sample = new Sample();
            var sut = new NetworkFunctionRegistry();

            Assert.False(sut.RegisterObject(sample));
        }

        [Test]
        public void ShouldRegisterObjectsWithUniqueIds()
        {
            var sample1 = new SampleNetwork();
            var sample2 = new SampleNetwork();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample1);
            sut.RegisterObject(sample2);

            Assert.False(sut.GetRegisteredObjectId(sample1) == sut.GetRegisteredObjectId(sample2));
        }

        [Test]
        public void ShouldRegisterObjectsWithIncreasingIdsStartingOneAfterTheNullId()
        {
            var sample1 = new SampleNetwork();
            var sample2 = new SampleNetwork();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample1);
            sut.RegisterObject(sample2);

            Assert.AreEqual(FirstObjectId, sut.GetRegisteredObjectId(sample1));
            Assert.AreEqual(NetworkFunctionRegistry.NULL_OBJECT_ID + 2, sut.GetRegisteredObjectId(sample2));
        }

        [Test]
        public void ShouldNotIncreaseIdCounterForNonNetworkObjects()
        {
            var sample1 = new Sample();
            var sample2 = new SampleNetwork();

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample1);
            sut.RegisterObject(sample2);

            Assert.AreEqual(FirstObjectId, sut.GetRegisteredObjectId(sample2));
        }

        [Test]
        public void ShouldRegisterAllNetworkFunctions()
        {
            var sample = new SampleNetwork();
            var functionId = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunction)));

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId));
            Assert.NotNull(sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId));
        }

        [Test]
        public void ShouldCorrectlyRegisterNetworkFunctions()
        {
            var sample = new SampleNetwork();
            var methodInfo = typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunction));
            var functionId = NetworkHashing.Hash(methodInfo);

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var function = sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId);

            Assert.AreEqual(methodInfo, function.Data.MethodInfo);
        }

        [Test]
        public void ShouldRegisterAllNetworkFunctionsOnParent()
        {
            var sample = new SampleNetwork();

            var functionId1 = NetworkHashing.Hash(typeof(SampleNetworkBase).GetDeclaredMethod(nameof(SampleNetworkBase.NetworkFunction)));
            var functionId2 = NetworkHashing.Hash(typeof(SampleNetworkBase).GetDeclaredMethod(nameof(SampleNetworkBase.NetworkFunctionOnlyBase)));

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId1));
            Assert.NotNull(sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId1));

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId2));
            Assert.NotNull(sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId2));
        }

        [Test]
        public void ShouldRegisterOverwrittenNetworkFunctionWithoutAttributeOnChild()
        {
            var sample = new SampleNetwork();
            var functionId = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunctionOnlyBase)));

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var id = sut.GetRegisteredObjectId(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(id, functionId));
        }

        [Test]
        public void ShouldOnlyRegisterNetworkObjects()
        {
            var sample = new Sample();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldNotRegisterNonNetworkFunctions()
        {
            var sample = new SampleNetwork();
            var functionId = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.RegularFunction)));

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredFunctionDelegate(sut.GetRegisteredObjectId(sample), functionId));
        }

        [Test]
        public void ShouldThrowIfRegisteringNullObject()
        {
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<ArgumentNullException>(() => sut.RegisterObject(null));
        }

        [Test]
        public void ShouldThrowIfObjectIsAlreadyRegistered()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.Throws<ArgumentException>(() => sut.RegisterObject(sample));
        }

        [Test]
        public void ShouldThrowIfNetworkFunctionIsOverloaded()
        {
            var sample = new SampleNetworkOverload();
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<InvalidOperationException>(() => sut.RegisterObject(sample));
        }

        [Test]
        public void ShouldProvideIdForRegisteredObject()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredObjectId(sample));
            Assert.AreEqual(FirstObjectId, sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldProvideNullIdIfAccessingIdWithNullObject()
        {
            var sut = new NetworkFunctionRegistry();

            Assert.AreEqual(NetworkFunctionRegistry.NULL_OBJECT_ID, sut.GetRegisteredObjectId(null));
        }

        [Test]
        public void ShouldThrowIfAccessingIdOfObjectNotRegistered()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObjectId(sample));
        }

        [Test]
        public void ShouldProvideRegisteredObjectForId()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var id = sut.GetRegisteredObjectId(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredObject(id));
            Assert.AreEqual(sample, sut.GetRegisteredObject(id));
        }

        [Test]
        public void ShouldProvideNullIfObjectWasPreviouslyRegistered()
        {
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var id = sut.GetRegisteredObjectId(sample);

            sut.ClearRegistrations();

            Assert.DoesNotThrow(() => sut.GetRegisteredObject(id));
            Assert.IsNull(sut.GetRegisteredObject(id));
        }

        [Test]
        public void ShouldThrowIfAccessingIdNotRegistered()
        {
            var sut = new NetworkFunctionRegistry();

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObject(12));
            Assert.Throws<ArgumentException>(() => sut.GetRegisteredObject(1));
        }

        [Test]
        public void ShouldProvideRegisteredFunctionForId()
        {
            var sample = new SampleNetwork();
            var methodInfo = typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunction));
            var functionId = NetworkHashing.Hash(methodInfo);

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var id = sut.GetRegisteredObjectId(sample);

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(id, functionId));
            Assert.AreEqual(methodInfo, sut.GetRegisteredFunctionDelegate(id, functionId).Data.MethodInfo);
        }

        [Test]
        public void ShouldProvideNullIfFunctionIdWasPreviouslyRegistered()
        {
            var sample = new SampleNetwork();
            var functionId = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunction)));

            var sut = new NetworkFunctionRegistry();

            sut.RegisterObject(sample);

            var id = sut.GetRegisteredObjectId(sample);

            sut.ClearRegistrations();

            Assert.DoesNotThrow(() => sut.GetRegisteredFunctionDelegate(id, functionId));
            Assert.IsNull(sut.GetRegisteredFunctionDelegate(id, functionId));
        }

        [Test]
        public void ShouldThrowIfAccessingFunctionIdNotRegistered()
        {
            var sut = new NetworkFunctionRegistry();
            var sample = new SampleNetwork();

            var functionIdValid = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.NetworkFunction)));
            var functionIdInvalid = NetworkHashing.Hash(typeof(SampleNetwork).GetDeclaredMethod(nameof(SampleNetwork.RegularFunction)));

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredFunctionDelegate(12, functionIdValid));

            sut.RegisterObject(sample);

            Assert.Throws<ArgumentException>(() => sut.GetRegisteredFunctionDelegate(12, functionIdInvalid));
        }

        [Test]
        public void ShouldRecordObjectRegistrationsToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry { Snapshot = snapshot.Object };

            sut.RegisterObject(sample);

            snapshot.Verify(x => x.RecordObjectRegistration(FirstObjectId, sample));
        }

        [Test]
        public void ShouldRecordObjectUnregistrationsToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry { Snapshot = snapshot.Object };

            sut.RegisterObject(sample);
            sut.UnregisterObject(sample);

            snapshot.Verify(x => x.RecordObjectUnregistration(FirstObjectId, sample, null));
        }

        [Test]
        public void ShouldRecordResetToSnapshot()
        {
            var snapshot = new Mock<Snapshot>();
            var sample = new SampleNetwork();
            var sut = new NetworkFunctionRegistry { Snapshot = snapshot.Object };

            sut.RegisterObject(sample);
            sut.ClearRegistrations();

            snapshot.Verify(x => x.RecordObjectUnregistration(null, null, "CLEARED"));
        }
    }
}
