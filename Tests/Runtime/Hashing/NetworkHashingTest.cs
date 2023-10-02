using System.Security.Cryptography;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Hashing {
    public class NetworkHashingTest {

        [Test]
        public void ShouldProvideValidHashForString() {
            const string text = "example";

            Assert.True(NetworkHashing.Hash(text).IsValid);
        }

        [Test]
        public void ShouldProvideValidHashForMethodInfo() {
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction));

            Assert.True(NetworkHashing.Hash(methodInfo).IsValid);
        }

        [Test]
        public void ShouldProvideEqualHashForEqualString() {
            var a = NetworkHashing.Hash("example");
            var b = NetworkHashing.Hash("example");

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldProvideDifferentHashForDifferentString() {
            var a = NetworkHashing.Hash("example");
            var b = NetworkHashing.Hash("testing");

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldCorrectlyHandleNonAsciiCharacters() {
            var a = NetworkHashing.Hash("exampleäöüӂӸઔ௵⑮╳🂇");
            var b = NetworkHashing.Hash("example");

            Assert.True(a.IsValid);
            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideEqualHashForEqualMethodInfo() {
            var a = NetworkHashing.Hash(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction)));
            var b = NetworkHashing.Hash(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction)));

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldProvideDifferentHashForDifferentMethodInfo() {
            var a = NetworkHashing.Hash(typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction)));
            var b = NetworkHashing.Hash(typeof(ISample).GetDeclaredMethod(nameof(ISample.MonadicFunction)));

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideDifferentHashForOverwrittenMethod() {
            var a = NetworkHashing.Hash(typeof(Sample).GetDeclaredMethod(nameof(Sample.VirtualFunction)));
            var b = NetworkHashing.Hash(typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.NiladicFunction)));

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideDifferentHashForHiddenMethod() {
            var a = NetworkHashing.Hash(typeof(Sample).GetDeclaredMethod(nameof(Sample.PublicFunction)));
            var b = NetworkHashing.Hash(typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction)));

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideDifferentHashForPrivateHiddenMethod() {
            var a = NetworkHashing.Hash(typeof(Sample).GetDeclaredMethod(Sample.PrivateFunctionName));
            var b = NetworkHashing.Hash(typeof(SampleBase).GetDeclaredMethod(SampleBase.PrivateFunctionName));

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldProvideHashSizeForMD5() {
            var md5 = MD5.Create();

            Assert.AreEqual(md5.HashSize, NetworkHashing.HashSizeBytes * 8);
        }
    }
}
