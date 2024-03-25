using System;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests
{
    public class SessionBuilderTest
    {
        [SetUp]
        public void SetUp() => Session.Instance = null;

        [Test]
        public void ShouldBuildForSteam()
        {
            var sut = new Session.Builder();

            Assert.DoesNotThrow(() => sut.ForSteam().Build());
        }

        [Test]
        public void ShouldBuildWithDefaults()
        {
            var sut = new Session.Builder();

            Assert.DoesNotThrow(() => sut.Build());
        }

        [Test]
        public void ShouldBuildWithInstance()
        {
            var sut = new Session.Builder();
            var cac = sut.Build();

            Assert.AreEqual(cac, Session.Instance);
        }

        [Test]
        public void ShouldThrowIfInstanceIsAlreadyDefined()
        {
            var sut = new Session.Builder();

            sut.Build();

            Assert.Throws<InvalidOperationException>(() => sut.Build());
        }
    }
}
