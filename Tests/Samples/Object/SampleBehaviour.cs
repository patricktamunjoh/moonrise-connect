using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object
{
    [NetworkObject]
    public class SampleBehaviour : MonoBehaviour
    {
        public InvocationCounter InvocationCounter { get; } = new();

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));
    }
}
