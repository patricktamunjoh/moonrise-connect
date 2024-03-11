using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Assertions;
using NUnit.Framework;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples
{
    public class SampleBehaviourTest
    {
        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations()
        {
            var sut = new GameObject().AddComponent<SampleBehaviour>();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBehaviour.NetworkFunction));
        }
    }
}
