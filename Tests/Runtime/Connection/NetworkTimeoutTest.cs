using System;
using System.Threading.Tasks;
using MoonriseGames.CloudsAhoyConnect.Connection;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Connection {
    public class NetworkTimeoutTest {

        [Test]
        public async Task ShouldInvokeTimeoutAfterDelay() {
            const int delay = 2;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();

            await Task.Delay(100);
            Assert.True(isTimeout);
        }

        [Test]
        public void ShouldNotImmediatelyInvokeTimeout() {
            const int delay = 100;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();

            Assert.False(isTimeout);
        }

        [Test]
        public async Task ShouldNotInvokeTimeoutBeforeDelay() {
            const int delay = 100;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();

            await Task.Delay(2);
            Assert.False(isTimeout);
        }

        [Test]
        public async Task ShouldNeverInvokeIfDelayIsNegative() {
            const int delay = -1;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();

            await Task.Delay(100);
            Assert.False(isTimeout);
        }

        [Test]
        public void ShouldThrowIfActionIsNull() {
            const int delay = 10;

            Assert.Throws<ArgumentNullException>(() => new NetworkTimeout(delay, null));
        }

        [Test]
        public async Task ShouldNotInvokeTimeoutAfterCancel() {
            const int delay = 2;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();
            sut.Cancel();

            await Task.Delay(100);
            Assert.False(isTimeout);
        }

        [Test]
        public async Task ShouldNotInvokeTimeoutAfterLateCancel() {
            const int delay = 100;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();
            await Task.Delay(10);

            sut.Cancel();
            await Task.Delay(delay);

            Assert.False(isTimeout);
        }

        [Test]
        public async Task ShouldRestartTimeoutAfterCancel() {
            const int delay = 2;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();
            sut.Cancel();
            sut.Start();

            await Task.Delay(100);
            Assert.True(isTimeout);
        }

        [Test]
        public async Task ShouldRestartTimeoutWhenStartedMultipleTimes() {
            const int delay = 100;
            var isTimeout = false;

            var sut = new NetworkTimeout(delay, () => isTimeout = true);

            sut.Start();
            await Task.Delay(delay / 2);

            sut.Start();

            await Task.Delay(delay / 2);
            Assert.False(isTimeout);

            await Task.Delay(delay);
            Assert.True(isTimeout);
        }
    }
}
