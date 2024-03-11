using System;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Functions;
using MoonriseGames.CloudsAhoyConnect.Utilities;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities
{
    public class WeakComparableReferenceTest
    {
        [Test]
        public void ShouldRetainTargetObject()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.AreEqual("example", sut.Target);
        }

        [Test]
        public void ShouldRetainTargetObjectAfterCast()
        {
            var sut = (WeakComparableReference<string>)"example";

            Assert.AreEqual("example", sut.Target);
        }

        [Test]
        public void ShouldProvideTargetObjectHashCode()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.AreEqual("example".GetHashCode(), sut.GetHashCode());
        }

        [Test]
        public void ShouldProvideZeroHashCodeWhenEmpty()
        {
            var sut = new WeakComparableReference<string>(null);

            Assert.Zero(sut.GetHashCode());
        }

        [Test]
        public void ShouldNotHoldStrongReference()
        {
            var sut = Function.ExecuteInCollectableScope(() => new WeakComparableReference<Sample>(new Sample()));

            GC.Collect();
            Assert.Null(sut.Target);
        }

        [Test]
        public void ShouldEqualSameReference()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.True(sut.Equals(sut));
        }

        [Test]
        public void ShouldEqualTargetObject()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.True(sut.Equals("example"));
        }

        [Test]
        public void ShouldEqualOtherReferenceWithTargetObject()
        {
            var a = new WeakComparableReference<string>("example");
            var b = new WeakComparableReference<string>("example");

            Assert.True(a.Equals(b));
        }

        [Test]
        public void ShouldNotEqualNull()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.False(sut.Equals(null));
        }

        [Test]
        public void ShouldEqualNullAfterGarbageCollection()
        {
            var sut = Function.ExecuteInCollectableScope(() => new WeakComparableReference<Sample>(new Sample()));

            GC.Collect();
            Assert.True(sut.Equals(null));
        }

        [Test]
        public void ShouldImplicitlyCastToTargetObject()
        {
            var sut = new WeakComparableReference<string>("example");

            Assert.DoesNotThrow(() => _ = (string)sut);
        }

        [Test]
        public void ShouldImplicitlyCastToReference()
        {
            const string sut = "example";

            Assert.DoesNotThrow(() => _ = (WeakComparableReference<string>)sut);
        }
    }
}
