using System;

namespace MoonriseGames.Connect.Functions
{
    /// <summary>
    /// Attribute for marking functions to be transitively synced over the network.
    /// A function with this attribute should only be called from a <see cref="NetworkFunction"/> or other <see cref="TransitiveNetworkFunction"/>.
    /// Any function that modifies the shared network state should be of one of these two types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransitiveNetworkFunction : Attribute { }
}
