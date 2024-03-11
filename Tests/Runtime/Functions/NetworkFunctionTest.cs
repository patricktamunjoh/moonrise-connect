using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Functions
{
    public class NetworkFunctionTest
    {
        [Test]
        public void ShouldRetainPropertyValues()
        {
            var sut = new NetworkFunction(Groups.Clients, Recipients.Clients, Transmission.Unreliable, true);

            Assert.AreEqual(Groups.Clients, sut.Authority);
            Assert.AreEqual(Recipients.Clients, sut.Recipients);
            Assert.AreEqual(Transmission.Unreliable, sut.Transmission);
            Assert.AreEqual(true, sut.IsDeferred);
        }

        [Test]
        public void ShouldHaveTransmissionTypesReliableAsDefault()
        {
            var sut = new NetworkFunction(Groups.Clients, Recipients.Clients);

            Assert.AreEqual(Transmission.Reliable, sut.Transmission);
        }

        [Test]
        public void ShouldHaveDeferredFalseAsDefault()
        {
            var sut = new NetworkFunction(Groups.Clients, Recipients.Clients);

            Assert.False(sut.IsDeferred);
        }
    }
}
