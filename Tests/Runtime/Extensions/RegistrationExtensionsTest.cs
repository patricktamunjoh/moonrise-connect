using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions {
    public class RegistrationExtensionsTest {

        [Test]
        public void ShouldNotThrowIfInstanceInNull() {
            var gameObj = new GameObject();

            CloudsAhoyConnect.Instance = null;

            Assert.DoesNotThrow(() => gameObj.Register());
            Assert.DoesNotThrow(() => gameObj.Register());
            Assert.DoesNotThrow(() => gameObj.RegisterInstance());
            Assert.DoesNotThrow(() => gameObj.UnregisterInstance());
        }

        [Test]
        public void ShouldRegisterGameObject() {
            var gameObj = new GameObject();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            gameObj.Register();

            registry.Verify(x => x.RegisterGameObject(gameObj, false));
        }

        [Test]
        public void ShouldRegisterGameObjectOfBehaviour() {
            var behaviour = new GameObject().AddComponent<SampleBehaviour>();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            behaviour.RegisterGameObject();

            registry.Verify(x => x.RegisterGameObject(behaviour.gameObject, false));
        }

        [Test]
        public void ShouldRegisterGameObjectWithChildren() {
            var gameObj = new GameObject();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            gameObj.Register(true);

            registry.Verify(x => x.RegisterGameObject(gameObj, true));
        }


        [Test]
        public void ShouldRegisterGameObjectOfBehaviourWithChildren() {
            var behaviour = new GameObject().AddComponent<SampleBehaviour>();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            behaviour.RegisterGameObject(true);

            registry.Verify(x => x.RegisterGameObject(behaviour.gameObject, true));
        }

        [Test]
        public void ShouldReturnSameGameObject() {
            var gameObj = new GameObject();

            Assert.AreSame(gameObj, gameObj.Register());
        }

        [Test]
        public void ShouldReturnSameBehaviour() {
            var behaviour = new GameObject().AddComponent<SampleBehaviour>();

            Assert.AreSame(behaviour, behaviour.RegisterGameObject());
        }

        [Test]
        public void ShouldRegisterObject() {
            var gameObj = new GameObject();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            gameObj.RegisterInstance();

            registry.Verify(x => x.RegisterObject(gameObj));
        }

        [Test]
        public void ShouldUnregisterObject() {
            var gameObj = new GameObject();
            var registry = new Mock<NetworkFunctionRegistry>();
            var cac = CloudsAhoyConnectFactory.Build(registry.Object);

            CloudsAhoyConnect.Instance = cac;

            gameObj.UnregisterInstance();

            registry.Verify(x => x.UnregisterObject(gameObj));
        }

        [Test]
        public void ShouldReturnSameObject() {
            var behaviour = new GameObject().AddComponent<SampleBehaviour>();

            Assert.AreSame(behaviour, behaviour.RegisterInstance());
            Assert.AreSame(behaviour, behaviour.UnregisterInstance());
        }
    }
}
