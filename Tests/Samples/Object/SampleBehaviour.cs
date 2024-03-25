using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Objects;
using MoonriseGames.Connect.Tests.Samples.Collections;
using UnityEngine;

namespace MoonriseGames.Connect.Tests.Samples.Object
{
    [NetworkObject]
    public class SampleBehaviour : MonoBehaviour
    {
        public InvocationCounter InvocationCounter { get; } = new();

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));
    }
}
