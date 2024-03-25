using MoonriseGames.Connect.Utilities;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Utilities
{
    public class CounterTest
    {
        [Test]
        public void ShouldInitializeToZeroAsDefault()
        {
            var sut = new Counter();

            Assert.AreEqual(0, sut.Value);
        }

        [Test]
        public void ShouldInitializeToStartValue()
        {
            var sut = new Counter(12);

            Assert.AreEqual(12, sut.Value);
        }

        [Test]
        public void ShouldCountUpInSingleSteps()
        {
            var sut = new Counter();

            Assert.AreEqual(0, sut.ReadAndIncrease());
            Assert.AreEqual(1, sut.ReadAndIncrease());
            Assert.AreEqual(2, sut.ReadAndIncrease());
            Assert.AreEqual(3, sut.ReadAndIncrease());
            Assert.AreEqual(4, sut.Value);
        }

        [Test]
        public void ShouldCountUpFromStartValue()
        {
            var sut = new Counter(12);

            Assert.AreEqual(12, sut.ReadAndIncrease());
            Assert.AreEqual(13, sut.ReadAndIncrease());
            Assert.AreEqual(14, sut.Value);
        }

        [Test]
        public void ShouldResetToStartValue()
        {
            var sut = new Counter(12);

            sut.ReadAndIncrease();
            sut.ReadAndIncrease();
            sut.Reset();

            Assert.AreEqual(12, sut.Value);
        }
    }
}
