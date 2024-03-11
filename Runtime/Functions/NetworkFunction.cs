using System;
using MoonriseGames.CloudsAhoyConnect.Enums;

namespace MoonriseGames.CloudsAhoyConnect.Functions
{
    /// <summary>
    /// Attribute for configuring instance functions to be synced over the network. Every function that should be called on other game
    /// instances must have this attribute defined. The properties are used to determine how the function should be invoked. Use this attribute
    /// only on not overloaded instance functions. It has no effect on static functions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NetworkFunction : Attribute
    {
        internal Groups Authority { get; }
        internal Recipients Recipients { get; }
        internal Transmission Transmission { get; }

        internal bool IsDeferred { get; }

        /// <summary>Initializes a new instance of the <see cref="NetworkFunction" /> attribute.</summary>
        /// <param name="authority">
        /// Determines who is permitted to call the function. When the function is called by a game instance not included by
        /// the authority, the call is ignored. This eliminates the need for differentiating between host and client game instances inside the game
        /// logic. Instead, the authority of each function should be set to only permit calls from the correct instances.
        /// </param>
        /// <param name="recipients">
        /// Determines on which game instances the function will be invoked. Function calls are only distributed to game
        /// instances defined as recipients. For the function to be invoked on the sending game instance, it itself must be a valid recipient.
        /// </param>
        /// <param name="type">
        /// Determines how the function messages are transmitted over the network. When set to
        /// <see cref="Transmission.Unreliable" /> calls might not be received by all <see cref="Recipients" />. The default is
        /// <see cref="Transmission.Reliable" />.
        /// </param>
        /// <param name="isDeferred">
        /// Determines how the function is invoked on the sending game instance. When true the function call is queued and
        /// invoked when processing incoming network calls. Otherwise the function is invoked immediately.
        /// </param>
        public NetworkFunction(Groups authority, Recipients recipients, Transmission type, bool isDeferred = false)
        {
            Authority = authority;
            Recipients = recipients;
            Transmission = type;
            IsDeferred = isDeferred;
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkFunction" /> attribute.</summary>
        /// <param name="authority">
        /// Determines who is permitted to call the function. When the function is called by a game instance not included by
        /// the authority, the call is ignored. This eliminates the need for differentiating between host and client game instances inside the game
        /// logic. Instead, the authority of each function should be set to only permit calls from the correct instances.
        /// </param>
        /// <param name="recipients">
        /// Determines on which game instances the function will be invoked. Function calls are only distributed to game
        /// instances defined as recipients. For the function to be invoked on the sending game instance, it itself must be a valid recipient.
        /// </param>
        /// <param name="type">
        /// Determines how the function messages are transmitted over the network. When set to
        /// <see cref="Transmission.Unreliable" /> calls might not be received by all <see cref="Recipients" />. The default is
        /// <see cref="Transmission.Reliable" />.
        /// </param>
        /// <param name="isDeferred">
        /// Determines how the function is invoked on the sending game instance. When true the function call is queued and
        /// invoked when processing incoming network calls. Otherwise the function is invoked immediately.
        /// </param>
        public NetworkFunction(Groups authority, Recipients recipients, bool isDeferred = false)
            : this(authority, recipients, Transmission.Reliable, isDeferred) { }
    }
}
