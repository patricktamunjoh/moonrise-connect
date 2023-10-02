using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Payloads;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Payloads {
    public class NetworkPayloadTypeTest {

        private int MaxParameterCount => typeof(NetworkPayload<,,,>).GetTypeInfo().GenericTypeParameters.Length;

        [Test]
        public void ShouldThrowIfTooManyParameters() {
            var types = Enumerable.Repeat(typeof(int), MaxParameterCount + 1).ToArray();

            Assert.Throws<ArgumentException>(() => new NetworkPayloadType(types));
        }

        [Test]
        public void ShouldThrowIfUnsupportedType() {
            var types = new[] { typeof(NetworkFunctionData) };

            Assert.Throws<ArgumentException>(() => new NetworkPayloadType(types));
        }

        [Test]
        public void ShouldThrowIfInstanceCreationReceivesTooFewArguments() {
            var arguments = new[] { "first", "second", "third" };
            var types = Enumerable.Repeat(typeof(string), arguments.Length + 1).ToArray();

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);

            Assert.Throws<ArgumentException>(() => sut.CreateInstanceFromArguments(registry.Object, arguments));
        }

        [Test]
        public void ShouldThrowIfInstanceCreationReceivesTooManyArguments() {
            var arguments = new[] { "first", "second", "third" };
            var types = Enumerable.Repeat(typeof(string), arguments.Length - 1).ToArray();

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);

            Assert.Throws<ArgumentException>(() => sut.CreateInstanceFromArguments(registry.Object, arguments));
        }

        [Test]
        public void ShouldNotModifyPrimitiveTypes() {
            var types = Enumerable.Repeat(typeof(int), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldFillMissingTypesWithDefault() {
            var expectedTypes = Enumerable.Repeat(typeof(object), MaxParameterCount).ToArray();

            var sut = new NetworkPayloadType();

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(expectedTypes), (Type)sut);
        }

        [Test]
        public void ShouldReplaceNetworkObjectsWithUnsignedLongs() {
            var types = Enumerable.Repeat(typeof(SampleNetwork), MaxParameterCount).ToArray();
            var expectedTypes = Enumerable.Repeat(typeof(ulong), MaxParameterCount).ToArray();

            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(expectedTypes), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessStrings() {
            var types = Enumerable.Repeat(typeof(string), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessDecimals() {
            var types = Enumerable.Repeat(typeof(decimal), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessEnums() {
            var types = Enumerable.Repeat(typeof(Roles), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessVector2() {
            var types = Enumerable.Repeat(typeof(Vector2), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessVector3() {
            var types = Enumerable.Repeat(typeof(Vector3), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessVector2Int() {
            var types = Enumerable.Repeat(typeof(Vector2Int), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessVector3Int() {
            var types = Enumerable.Repeat(typeof(Vector3Int), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessVector4() {
            var types = Enumerable.Repeat(typeof(Vector4), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessMatrix4x4() {
            var types = Enumerable.Repeat(typeof(Matrix4x4), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessQuaternion() {
            var types = Enumerable.Repeat(typeof(Quaternion), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessColor() {
            var types = Enumerable.Repeat(typeof(Color), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessRect() {
            var types = Enumerable.Repeat(typeof(Rect), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessLayerMask() {
            var types = Enumerable.Repeat(typeof(LayerMask), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessArrays() {
            var types = Enumerable.Repeat(typeof(string[]), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessLists() {
            var types = Enumerable.Repeat(typeof(List<string>), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCorrectlyProcessSerializableTypes() {
            var types = Enumerable.Repeat(typeof(SampleData), MaxParameterCount).ToArray();
            var sut = new NetworkPayloadType(types);

            Assert.AreEqual(typeof(NetworkPayload<,,,>).MakeGenericType(types), (Type)sut);
        }

        [Test]
        public void ShouldCreatePayloadInstance() {
            var types = Enumerable.Repeat(typeof(string), MaxParameterCount).ToArray();
            var arguments = new[] { "first", "second", "third", "fourth" };

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);
            var output = sut.CreateInstanceFromArguments(registry.Object, arguments).Arguments();

            Assert.True(output.SequenceEqual(arguments));
        }

        [Test]
        public void ShouldCreatePayloadInstanceWithMinimalArguments() {
            var types = new[] { typeof(string) };
            var arguments = new[] { "first" };

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);
            var output = sut.CreateInstanceFromArguments(registry.Object, arguments).Arguments().FirstOrDefault();

            Assert.AreEqual(arguments[0], output);
        }

        [Test]
        public void ShouldCreatePayloadInstanceWithReplacements() {
            const uint objectId = 14u;

            var sample = new SampleNetwork();
            var types = new[] { typeof(SampleNetwork) };
            var arguments = new[] { sample };

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredObjectId(sample)).Returns(objectId);

            var sut = new NetworkPayloadType(types);
            var output = sut.CreateInstanceFromArguments(registry.Object, arguments).Arguments().FirstOrDefault();

            Assert.NotNull(output);
            Assert.AreEqual(objectId, (ulong)output);
        }

        [Test]
        public void ShouldReturnCorrectNumberOfArguments() {
            const int parameterCount = 2;

            var types = Enumerable.Repeat(typeof(string), parameterCount).ToArray();
            var payload = new NetworkPayload<string, string, string, string>();

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);
            var output = sut.RecoverArgumentsFromInstance(registry.Object, payload);

            Assert.AreEqual(parameterCount, output.Length);
        }

        [Test]
        public void ShouldRetrieveArgumentsFromPayloadInstance() {
            var types = Enumerable.Repeat(typeof(string), MaxParameterCount).ToArray();
            var arguments = new[] { "first", "second", "third", "fourth" };
            var payload = new NetworkPayload<string, string, string, string>(arguments[0], arguments[1], arguments[2], arguments[3]);

            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);
            var output = sut.RecoverArgumentsFromInstance(registry.Object, payload);

            Assert.True(output.SequenceEqual(arguments));
        }

        [Test]
        public void ShouldReturnNullForNullPayloadInstance() {
            var types = Enumerable.Repeat(typeof(string), MaxParameterCount).ToArray();
            var registry = new Mock<NetworkFunctionRegistry>();

            var sut = new NetworkPayloadType(types);
            var output = sut.RecoverArgumentsFromInstance(registry.Object, null);

            Assert.IsNull(output);
        }

        [Test]
        public void ShouldRetrieveParametersFromPayloadInstanceWithReplacement() {
            const uint objectId = 14u;

            var types = new[] { typeof(SampleNetwork) };
            var payload = new NetworkPayload<ulong, string, string, string>(objectId);
            var sample = new SampleNetwork();

            var registry = new Mock<NetworkFunctionRegistry>();

            registry.Setup(x => x.GetRegisteredObject(objectId)).Returns(sample);

            var sut = new NetworkPayloadType(types);
            var output = sut.RecoverArgumentsFromInstance(registry.Object, payload);

            Assert.NotZero(output.Length);
            Assert.AreEqual(sample, output[0]);
        }
    }
}
