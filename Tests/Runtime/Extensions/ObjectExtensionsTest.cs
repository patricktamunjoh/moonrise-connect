using System;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using NUnit.Framework;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions
{
    public class ObjectExtensionsTest
    {
        [Test]
        public void ShouldProvideFullName()
        {
            var root = new GameObject("parent");
            var child = new GameObject("child") { transform = { parent = root.transform } };

            Assert.AreEqual("parent.child", child.transform.FullName());
        }

        [Test]
        public void ShouldThrowIfObjectIsNull()
        {
            var sut = null as object;

            Assert.Throws<ArgumentNullException>(() => sut.ThrowIfNull());
        }

        [Test]
        public void ShouldNotThrowIfObjectIsNotNull()
        {
            const string sut = "example";

            Assert.DoesNotThrow(() => sut.ThrowIfNull());
        }

        [Test]
        public void ShouldReturnTheSameObject()
        {
            const string sut = "example";

            Assert.AreEqual(sut, sut.ThrowIfNull());
        }

        [Test]
        public void ShouldProvideAllChildObjects()
        {
            var root = new GameObject();
            var child1 = new GameObject { transform = { parent = root.transform } };
            var child2 = new GameObject { transform = { parent = child1.transform } };

            var sut = root.ChildGameObjects().ToList();

            Assert.Contains(child1, sut);
            Assert.Contains(child2, sut);
        }

        [Test]
        public void ShouldNotProvideSelfAsChild()
        {
            var root = new GameObject();
            var sut = root.ChildGameObjects().ToList();

            Assert.False(sut.Contains(root));
        }

        [Test]
        public void ShouldFindAllMonoBehavioursOnObject()
        {
            var root = new GameObject();
            var behaviour1 = root.AddComponent<SampleBehaviour>();
            var behaviour2 = root.AddComponent<SampleBehaviour>();

            var sut = root.MonoBehaviours().ToList();

            Assert.Contains(behaviour1, sut);
            Assert.Contains(behaviour2, sut);
        }

        [Test]
        public void ShouldNotProvideMonoBehavioursOnChildObjects()
        {
            var root = new GameObject();
            var child1 = new GameObject { transform = { parent = root.transform } };
            var behaviour = child1.AddComponent<SampleBehaviour>();

            var sut = root.MonoBehaviours().ToList();

            Assert.False(sut.Contains(behaviour));
        }
    }
}
