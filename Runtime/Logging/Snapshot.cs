using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;

namespace MoonriseGames.CloudsAhoyConnect.Logging
{
    /// <summary>
    /// Collection of network event data through a certain period of time. This class can be used to collect information to debug
    /// networking or synchronisation issue.
    /// </summary>
    public class Snapshot
    {
        private List<(ulong, Type, string)> Registrations { get; } = new();
        private List<(ulong?, Type, string, string)> Unregistrations { get; } = new();

        private List<(string, ulong, string, Transmission)> OutgoingCalls { get; } = new();
        private List<(string, ulong, string, Transmission)> IncomingCalls { get; } = new();

        private List<(NetworkConnectionEventArgs.Types, string)> Events { get; } = new();

        /// <summary>
        /// Collection of object registrations containing the id with which the object was registered, the type of the object and its string
        /// representation.
        /// </summary>
        public ReadOnlyCollection<(ulong id, Type type, string target)> ObjectRegistrations => Registrations.AsReadOnly();

        /// <summary>
        /// Collection of object unregistrations containing the id with which the object was registered, if available, the type of the object
        /// and its string representation. Additionally an info message can be supplied for events in which more than one object were unregistered or
        /// object data is no longer available.
        /// </summary>
        public ReadOnlyCollection<(ulong? id, Type type, string target, string info)> ObjectUnregistrations => Unregistrations.AsReadOnly();

        /// <summary>
        /// Collection of outgoing network calls containing the name of the recipient, the id of the targeted object, the encoded hash of the
        /// target function and the transmission type.
        /// </summary>
        public ReadOnlyCollection<(string recipient, ulong id, string function, Transmission transmission)> OutgoingNetworkCalls => OutgoingCalls.AsReadOnly();

        /// <summary>
        /// Collection of incoming network calls containing the name of the sender, the id of the targeted object, the encoded hash of the
        /// target function and the transmission type.
        /// </summary>
        public ReadOnlyCollection<(string sender, ulong id, string function, Transmission transmission)> IncomingNetworkCalls => IncomingCalls.AsReadOnly();

        /// <summary>
        /// Collection of network event containing the event type and optionally the name of the involved game instance. These events match
        /// the events raised by <see cref="Session.OnNetworkConnectionChanged" />
        /// </summary>
        public ReadOnlyCollection<(NetworkConnectionEventArgs.Types type, string target)> NetworkEvents => Events.AsReadOnly();

        internal virtual void RecordObjectRegistration(ulong id, object target) => Registrations.Add((id, target.ThrowIfNull().GetType(), target.ThrowIfNull().ToString()));

        internal virtual void RecordObjectUnregistration(ulong? id, object target, string info = null) => Unregistrations.Add((id, target?.GetType(), target?.ToString(), info));

        internal virtual void RecordOutgoingNetworkCall(NetworkIdentity recipient, NetworkFunctionCall call) =>
            OutgoingCalls.Add((recipient.DisplayName, call.ObjectId, call.FunctionId.ToBase64(), call.Transmission));

        internal virtual void RecordIncomingNetworkCall(NetworkIdentity sender, NetworkFunctionCall call) =>
            IncomingCalls.Add((sender.DisplayName, call.ObjectId, call.FunctionId.ToBase64(), call.Transmission));

        internal virtual void RecordNetworkEvent(NetworkConnectionEventArgs args) => Events.Add((args.Type, args.Target?.DisplayName));
    }
}
