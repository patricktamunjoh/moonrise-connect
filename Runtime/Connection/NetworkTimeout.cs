using System;
using System.Threading;
using System.Threading.Tasks;
using MoonriseGames.CloudsAhoyConnect.Extensions;

namespace MoonriseGames.CloudsAhoyConnect.Connection
{
    internal class NetworkTimeout
    {
        private int TimeoutDurationMs { get; }
        private bool IsInfinite { get; }
        private Action OnTimeout { get; }

        private CancellationTokenSource TimeoutCts { get; set; }

        public NetworkTimeout(int timeoutDurationMs, Action onTimeout)
        {
            IsInfinite = timeoutDurationMs < 0;
            TimeoutDurationMs = timeoutDurationMs;
            OnTimeout = onTimeout.ThrowIfNull();
        }

        public void Start()
        {
            if (IsInfinite)
                return;
            Cancel();

            TimeoutCts = new CancellationTokenSource();
            TrackTimeout();
        }

        public void Cancel() => TimeoutCts?.Cancel();

        private async void TrackTimeout()
        {
            try
            {
                await Task.Delay(TimeoutDurationMs, TimeoutCts.Token);
                OnTimeout.Invoke();
            }
            catch (OperationCanceledException) { }
        }
    }
}
