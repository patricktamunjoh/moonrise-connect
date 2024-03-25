using System;
using System.Collections.Generic;
using MoonriseGames.Connect.Collections;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Collections
{
    public class BidirectionalMapTest
    {
        [Test]
        public void ShouldCorrectlyStoreValuesFromHead()
        {
            var sut = new BidirectionalMap<string, int> { ["example"] = 12 };

            Assert.AreEqual(12, sut["example"]);
            Assert.AreEqual("example", sut[12]);
        }

        [Test]
        public void ShouldCorrectlyStoreValuesFromTail()
        {
            var sut = new BidirectionalMap<string, int> { [12] = "example" };

            Assert.AreEqual(12, sut["example"]);
            Assert.AreEqual("example", sut[12]);
        }

        [Test]
        public void ShouldRemovePreviousConnectionIfKeyIsReused()
        {
            var sut = new BidirectionalMap<string, int> { [12] = "example", [12] = "testing" };

            Assert.False((bool)sut.Contains("example"));
            Assert.AreEqual(12, sut["testing"]);
            Assert.AreEqual("testing", sut[12]);

            sut["example"] = 12;

            Assert.False((bool)sut.Contains("testing"));
            Assert.AreEqual(12, sut["example"]);
            Assert.AreEqual("example", sut[12]);

            sut["example"] = 42;

            Assert.False((bool)sut.Contains(12));
            Assert.AreEqual(42, sut["example"]);
            Assert.AreEqual("example", sut[42]);
        }

        [Test]
        public void ShouldProvideCorrectCount()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.AreEqual(0, sut.Count);

            sut["first"] = 1;
            Assert.AreEqual(1, sut.Count);

            sut["second"] = 2;
            Assert.AreEqual(2, sut.Count);

            sut.Remove("second");
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public void ShouldNotRemoveMissingValues()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.False((bool)sut.Remove("example"));
            Assert.False((bool)sut.Remove(12));
        }

        [Test]
        public void ShouldCorrectlyRemoveValuesFromHead()
        {
            var sut = new BidirectionalMap<string, int> { ["example"] = 12 };

            Assert.True((bool)sut.Remove("example"));
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public void ShouldCorrectlyRemoveValuesFromTail()
        {
            var sut = new BidirectionalMap<string, int> { [12] = "example" };

            Assert.True((bool)sut.Remove(12));
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public void ShouldCorrectlyClearAllValues()
        {
            var sut = new BidirectionalMap<string, int> { [12] = "example" };

            sut.Clear();
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public void ShouldContainStoredValues()
        {
            var sut = new BidirectionalMap<string, int> { [12] = "example" };

            Assert.True((bool)sut.Contains("example"));
            Assert.True((bool)sut.Contains(12));
        }

        [Test]
        public void ShouldNotContainMissingValues()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.False((bool)sut.Contains("example"));
            Assert.False((bool)sut.Contains(12));
        }

        [Test]
        public void ShouldThrowWhenReadingMissingValue()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["example"];
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut[12];
            });
        }

        [Test]
        public void ShouldThrowWhenReadingNullKey()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = sut[null];
            });
        }

        [Test]
        public void ShouldThrowWhenStoringNullKey()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.Throws<ArgumentNullException>(() => sut[null] = 12);
        }

        [Test]
        public void ShouldThrowWhenCheckingNullKey()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.Throws<ArgumentNullException>(() => sut.Contains(null));
        }

        [Test]
        public void ShouldThrowWhenRemovingNullKey()
        {
            var sut = new BidirectionalMap<string, int>();

            Assert.Throws<ArgumentNullException>(() => sut.Remove(null));
        }
    }
}
