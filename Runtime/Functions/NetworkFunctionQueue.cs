using System;
using System.Collections.Generic;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Payloads;

namespace MoonriseGames.CloudsAhoyConnect.Functions
{
    internal class NetworkFunctionQueue
    {
        private NetworkFunctionRegistry Registry { get; }
        private Queue<(QueueElement element, Roles role, bool isSender)> FunctionDelegateQueue { get; } = new();

        public virtual bool IsEmpty => FunctionDelegateQueue.Count == 0;

        private interface QueueElement
        {
            Transmission Transmission { get; }
            void RetrieveDelegate(out NetworkFunctionDelegate functionDelegate, out INetworkPayload payload);
        }

        private class QueueElementDelegate : QueueElement
        {
            private NetworkFunctionDelegate FunctionDelegate { get; }
            private INetworkPayload Payload { get; }
            public Transmission Transmission => FunctionDelegate.Data.Function.Transmission;

            public QueueElementDelegate(NetworkFunctionDelegate functionDelegate, INetworkPayload payload)
            {
                FunctionDelegate = functionDelegate.ThrowIfNull();
                Payload = payload;
            }

            public void RetrieveDelegate(out NetworkFunctionDelegate functionDelegate, out INetworkPayload payload)
            {
                functionDelegate = FunctionDelegate;
                payload = Payload;
            }
        }

        private class QueueElementCall : QueueElement
        {
            private NetworkFunctionCall Call { get; }
            private NetworkFunctionRegistry Registry { get; }
            public Transmission Transmission => Call.Transmission;

            public QueueElementCall(NetworkFunctionCall call, NetworkFunctionRegistry registry)
            {
                Call = call.ThrowIfNull();
                Registry = registry.ThrowIfNull();
            }

            public void RetrieveDelegate(out NetworkFunctionDelegate functionDelegate, out INetworkPayload payload)
            {
                functionDelegate = Registry.GetRegisteredFunctionDelegate(Call.ObjectId, Call.FunctionId);
                payload = functionDelegate != null ? Call.DecodedPayload(functionDelegate.Data.PayloadType) : null;
            }
        }

        public NetworkFunctionQueue(NetworkFunctionRegistry registry) => Registry = registry;

        public virtual void EnqueueCall(NetworkFunctionCall call, Roles role, bool isSender)
        {
            var element = new QueueElementCall(call, Registry);
            FunctionDelegateQueue.Enqueue((element, role, isSender));
        }

        public virtual void EnqueueDelegate(NetworkFunctionDelegate functionDelegate, INetworkPayload payload, Roles role, bool isSender)
        {
            var element = new QueueElementDelegate(functionDelegate, payload);

            if (!functionDelegate.Data.Function.IsDeferred && isSender)
            {
                ProcessQueueElement(element, role, true);
                return;
            }

            FunctionDelegateQueue.Enqueue((element, role, isSender));
        }

        public virtual void ProcessQueuedElements()
        {
            while (!IsEmpty)
            {
                var (element, role, isSender) = FunctionDelegateQueue.Dequeue();
                ProcessQueueElement(element, role, isSender);
            }
        }

        private void ProcessQueueElement(QueueElement element, Roles role, bool isSender)
        {
            try
            {
                element.RetrieveDelegate(out var functionDelegate, out var payload);
                if (functionDelegate?.Data.Function.Recipients.Contains(role, isSender) != true)
                    return;

                var arguments = functionDelegate.Data.PayloadType.RecoverArgumentsFromInstance(Registry, payload);
                functionDelegate.Invoke(arguments);
            }
            catch (ArgumentException)
            {
                if (element.Transmission != Transmission.Unreliable || isSender)
                    throw;
            }
        }
    }
}
