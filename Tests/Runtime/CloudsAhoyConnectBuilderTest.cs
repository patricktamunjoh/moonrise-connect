using System;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests {
    public class CloudsAhoyConnectBuilderTest {

        [SetUp]
        public void SetUp() => CloudsAhoyConnect.Instance = null;

        [Test]
        public void ShouldBuildForSteam() {
            var sut = new CloudsAhoyConnect.Builder();

            Assert.DoesNotThrow(() => sut.ForSteam().Build());
        }

        [Test]
        public void ShouldBuildWithDefaults() {
            var sut = new CloudsAhoyConnect.Builder();

            Assert.DoesNotThrow(() => sut.Build());
        }

        [Test]
        public void ShouldBuildWithInstance() {
            var sut = new CloudsAhoyConnect.Builder();
            var cac = sut.Build();

            Assert.AreEqual(cac, CloudsAhoyConnect.Instance);
        }

        [Test]
        public void ShouldThrowIfInstanceIsAlreadyDefined() {
            var sut = new CloudsAhoyConnect.Builder();

            sut.Build();

            Assert.Throws<InvalidOperationException>(() => sut.Build());
        }
    }
}
