using System;
using System.Collections.Generic;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Payloads;

namespace MoonriseGames.CloudsAhoyConnect.Functions {
    internal class NetworkFunctionQueue {

        private NetworkFunctionRegistry Registry { get; }
        private Queue<(NetworkFunctionDelegate, INetworkPayload)> FunctionDelegateQueue { get; } = new();

        public virtual bool IsEmpty => FunctionDelegateQueue.Count == 0;

        public NetworkFunctionQueue(NetworkFunctionRegistry registry) => Registry = registry;

        public virtual void EnqueueCall(NetworkFunctionCall call, Roles role, bool isSender) {
            call.ThrowIfNull();

            try {
                var functionDelegate = Registry.GetRegisteredFunctionDelegate(call.ObjectId, call.FunctionId);
                if (functionDelegate == null) return;

                var payload = call.DecodedPayload(functionDelegate.Data.PayloadType);

                EnqueueDelegate(functionDelegate, payload, role, isSender);
            }
            catch (ArgumentException) {
                if (call.Transmission != Transmission.Unreliable || isSender) throw;
            }
        }

        public virtual void EnqueueDelegate(NetworkFunctionDelegate functionDelegate, INetworkPayload payload, Roles role, bool isSender) {
            functionDelegate.ThrowIfNull();

            if (!functionDelegate.Data.Function.Recipients.Contains(role, isSender)) return;

            if (!functionDelegate.Data.Function.IsDeferred && isSender) {
                ProcessCall(functionDelegate, payload);
                return;
            }

            FunctionDelegateQueue.Enqueue((functionDelegate, payload));
        }

        public virtual void ProcessQueuedElements() {
            while (!IsEmpty) {
                var (functionDelegate, payload) = FunctionDelegateQueue.Dequeue();
                ProcessCall(functionDelegate, payload);
            }
        }

        private void ProcessCall(NetworkFunctionDelegate functionDelegate, INetworkPayload payload) {
            var arguments = functionDelegate.Data.PayloadType.RecoverArgumentsFromInstance(Registry, payload);
            functionDelegate.Invoke(arguments);
        }
    }
}
