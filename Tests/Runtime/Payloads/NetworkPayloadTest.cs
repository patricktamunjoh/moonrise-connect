using System.Linq;
using MoonriseGames.Connect.Payloads;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Payloads
{
    public class NetworkPayloadTest
    {
        [Test]
        public void ShouldAlwaysProvideAllArguments()
        {
            var sut = new NetworkPayload<string, string, string, string>() as INetworkPayload;

            var arguments = Enumerable.ToArray<object>(sut.Arguments());

            Assert.AreEqual(4, arguments.Length);
        }

        [Test]
        public void ShouldProvideArgumentValues()
        {
            var sut = new NetworkPayload<int, int, int, int>(0, 1, 2, 3) as INetworkPayload;

            var arguments = Enumerable.ToArray<object>(sut.Arguments());

            Assert.AreEqual(0, arguments[0]);
            Assert.AreEqual(1, arguments[1]);
            Assert.AreEqual(2, arguments[2]);
            Assert.AreEqual(3, arguments[3]);
        }

        [Test]
        public void ShouldProvideDefaultValuesForUninitializedArguments()
        {
            var sut = new NetworkPayload<string, string, string, string>("example") as INetworkPayload;

            var arguments = Enumerable.ToArray<object>(sut.Arguments());

            Assert.IsNotNull(arguments[0]);
            Assert.IsNull(arguments[1]);
        }
    }
}
