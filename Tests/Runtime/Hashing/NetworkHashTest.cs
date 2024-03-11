using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Hashing
{
    public class NetworkHashTest
    {
        [Test]
        public void ShouldRetainHashValue()
        {
            var hash = new[] { (byte)0 };
            var sut = new NetworkHash(hash);

            Assert.AreEqual(hash, sut.Hash);
        }

        [Test]
        public void ShouldEncodeToAndFromString()
        {
            var sut = new NetworkHash("ZXhhbXBsZQ==");

            Assert.AreEqual("ZXhhbXBsZQ==", sut.ToBase64());
        }

        [Test]
        public void ShouldReconstructHashFromString()
        {
            var hash = new[] { (byte)0, (byte)12, (byte)42 };
            var sut = new NetworkHash(new NetworkHash(hash).ToBase64());

            Assert.True(hash.SequenceEqual(sut.Hash));
        }

        [Test]
        public void ShouldEqualEmptyObjects()
        {
            var a = new NetworkHash();
            var b = new NetworkHash();

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldEqualEmptyAndDefaultObject()
        {
            var a = new NetworkHash();
            var b = default(NetworkHash);

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldNotEqualEmptyAndNonEmptyObject()
        {
            var a = new NetworkHash();
            var b = new NetworkHash(new[] { (byte)0 });

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldEqualIdenticalHash()
        {
            var a = new NetworkHash(new[] { (byte)12 });
            var b = new NetworkHash(new[] { (byte)12 });

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldNotEqualDifferentHash()
        {
            var a = new NetworkHash(new[] { (byte)12 });
            var b = new NetworkHash(new[] { (byte)12, (byte)12 });

            Assert.False(a.Equals(b));
        }

        [Test]
        public void ShouldNotEqualNull()
        {
            var a = new NetworkHash(new[] { (byte)12 });

            Assert.False(a.Equals(null));
        }

        [Test]
        public void ShouldProvideEqualHashCodeForEmptyObjects()
        {
            var a = new NetworkHash();
            var b = new NetworkHash();

            Assert.True(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void ShouldProvideEqualHashCodeForIdenticalHash()
        {
            var a = new NetworkHash(new[] { (byte)12 });
            var b = new NetworkHash(new[] { (byte)12 });

            Assert.True(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void ShouldBeValidIfCorrectHashSize()
        {
            var a = new NetworkHash(new byte[NetworkHashing.HashSizeBytes]);

            Assert.True(a.IsValid);
        }

        [Test]
        public void ShouldBeInvalidIfIncorrectHashSize()
        {
            var a = new NetworkHash(new byte[NetworkHashing.HashSizeBytes - 1]);

            Assert.False(a.IsValid);
        }

        [Test]
        public void ShouldBeInvalidIfEmpty()
        {
            var a = new NetworkHash();

            Assert.False(a.IsValid);
        }
    }
}
