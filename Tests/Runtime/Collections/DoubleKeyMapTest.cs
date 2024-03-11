using System;
using System.Collections.Generic;
using MoonriseGames.CloudsAhoyConnect.Collections;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Collections
{
    public class DoubleKeyMapTest
    {
        [Test]
        public void ShouldCorrectlyStoreValues()
        {
            var sut = new DoubleKeyMap<string, string, int> { ["first", "second"] = 12 };

            Assert.AreEqual(12, sut["first", "second"]);
        }

        [Test]
        public void ShouldNotRemoveMissingValues()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.False(sut.Remove("first", "second"));
            Assert.False(sut.Remove("first"));
        }

        [Test]
        public void ShouldCorrectlyRemoveValues()
        {
            var sut = new DoubleKeyMap<string, string, int> { ["first", "second"] = 12 };

            Assert.True(sut.Remove("first", "second"));

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["first", "second"];
            });
        }

        [Test]
        public void ShouldCorrectlyRemoveValueGroups()
        {
            var sut = new DoubleKeyMap<string, string, int> { ["first", "second"] = 12, ["first", "third"] = 42 };

            Assert.True(sut.Remove("first"));

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["first", "second"];
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["first", "third"];
            });
        }

        [Test]
        public void ShouldCorrectlyClearAllValues()
        {
            var sut = new DoubleKeyMap<string, string, int> { ["first", "second"] = 12 };

            sut.Clear();

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["first", "second"];
            });
        }

        [Test]
        public void ShouldContainStoredValues()
        {
            var sut = new DoubleKeyMap<string, string, int> { ["first", "second"] = 12 };

            Assert.True(sut.Contains("first", "second"));
            Assert.True(sut.Contains("first"));
        }

        [Test]
        public void ShouldNotContainMissingValues()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.False(sut.Contains("first", "second"));
            Assert.False(sut.Contains("first"));
        }

        [Test]
        public void ShouldThrowWhenReadingMissingValue()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = sut["first", "second"];
            });
        }

        [Test]
        public void ShouldThrowWhenReadingNullKey()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = sut[null, "second"];
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = sut["first", null];
            });
        }

        [Test]
        public void ShouldThrowWhenStoringNullKey()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.Throws<ArgumentNullException>(() => sut[null, "second"] = 12);
            Assert.Throws<ArgumentNullException>(() => sut["first", null] = 12);
        }

        [Test]
        public void ShouldThrowWhenCheckingNullKey()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.Throws<ArgumentNullException>(() => sut.Contains(null, "second"));
            Assert.Throws<ArgumentNullException>(() => sut.Contains("first", null));
            Assert.Throws<ArgumentNullException>(() => sut.Contains(null));
        }

        [Test]
        public void ShouldThrowWhenRemovingNullKey()
        {
            var sut = new DoubleKeyMap<string, string, int>();

            Assert.Throws<ArgumentNullException>(() => sut.Remove(null, "second"));
            Assert.Throws<ArgumentNullException>(() => sut.Remove("first", null));
            Assert.Throws<ArgumentNullException>(() => sut.Remove(null));
        }
    }
}
