using Moq;

namespace MoonriseGames.Connect.Tests.Utilities.Extensions
{
    internal static class MockExtensions
    {
        public static Mock<T> CallingBase<T>(this Mock<T> mock)
            where T : class
        {
            mock.CallBase = true;
            return mock;
        }
    }
}
