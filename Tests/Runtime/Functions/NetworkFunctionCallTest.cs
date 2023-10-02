using System;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Payloads;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Functions {
    public class NetworkFunctionCallTest {

        [Test]
        public void ShouldThrowWhenInvalidFunctionIsSupplied() {
            var functionId = new NetworkHash();

            Assert.Throws<InvalidOperationException>(() => _ = new NetworkFunctionCall(12u, functionId, Transmission.Reliable));
        }

        [Test]
        public void ShouldThrowWhenInvalidByteArrayIsSupplied() {
            var bytes = Array.Empty<byte>();

            Assert.Throws<InvalidOperationException>(() => _ = new NetworkFunctionCall(bytes));
        }

        [Test]
        public void ShouldRetainPropertyValues() {
            const uint objectId = 12u;

            var functionId = NetworkHashing.Hash("example functionId");

            foreach (var transmission in typeof(Transmission).EnumValues<Transmission>()) {
                var sut = new NetworkFunctionCall(objectId, functionId, transmission);

                Assert.AreEqual(objectId, sut.ObjectId);
                Assert.AreEqual(functionId, sut.FunctionId);
                Assert.AreEqual(transmission, sut.Transmission);
            }
        }

        [Test]
        public void ShouldEncodeAndDecodeToBytes() {
            const uint objectId = 12u;

            var functionId = NetworkHashing.Hash("example functionId");

            foreach (var transmission in typeof(Transmission).EnumValues<Transmission>()) {
                var sut = new NetworkFunctionCall(objectId, functionId, transmission);

                var bytes = sut.ToBytes();
                var decoded = new NetworkFunctionCall(bytes);

                Assert.AreEqual(sut.ObjectId, decoded.ObjectId);
                Assert.AreEqual(sut.FunctionId, decoded.FunctionId);
                Assert.AreEqual(sut.Transmission, decoded.Transmission);
            }
        }

        [Test]
        public void ShouldEncodeAndDecodePayload() {
            const uint objectId = 12u;

            var functionId = NetworkHashing.Hash("example functionId");

            var sut = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);
            var payload = new NetworkPayload<int, int, string, object>(0, 1);

            sut.EncodePayload(payload);

            var decoded = sut.DecodedPayload(payload.GetType());

            Assert.NotNull(decoded);

            var arguments = decoded.Arguments().ToArray();

            Assert.AreEqual(0, arguments[0]);
            Assert.AreEqual(1, arguments[1]);

            Assert.True(string.IsNullOrEmpty(arguments[2] as string));
            Assert.Null(arguments[3]);
        }

        [Test]
        public void ShouldEncodeAndDecodeSelfWithPayload() {
            const uint objectId = 12u;

            var functionId = NetworkHashing.Hash("example functionId");

            var sut = new NetworkFunctionCall(objectId, functionId, Transmission.Reliable);
            var payload = new NetworkPayload<int, int, string, object>(0, 1);

            sut.EncodePayload(payload);

            var decodedCall = new NetworkFunctionCall(sut.ToBytes());
            var decoded = decodedCall.DecodedPayload(payload.GetType());

            Assert.NotNull(decoded);

            var arguments = decoded.Arguments().ToArray();

            Assert.AreEqual(0, arguments[0]);
            Assert.AreEqual(1, arguments[1]);

            Assert.True(string.IsNullOrEmpty(arguments[2] as string));
            Assert.Null(arguments[3]);
        }
    }
}
