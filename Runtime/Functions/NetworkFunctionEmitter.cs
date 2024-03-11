using System;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Hashing;

namespace MoonriseGames.CloudsAhoyConnect.Functions
{
    internal class NetworkFunctionEmitter
    {
        private NetworkFunctionQueue Queue { get; }
        private NetworkFunctionRegistry Registry { get; }
        private NetworkConnection Connection { get; }

        public NetworkFunctionEmitter(NetworkFunctionQueue queue, NetworkFunctionRegistry registry, NetworkConnection connection)
        {
            Queue = queue;
            Registry = registry;
            Connection = connection;
        }

        public virtual void Call(Action function)
        {
            function.ThrowIfNull();
            Call(function.Target, function.Method);
        }

        public virtual void Call<T1>(Action<T1> function, T1 p1)
        {
            function.ThrowIfNull();
            Call(function.Target, function.Method, p1);
        }

        public virtual void Call<T1, T2>(Action<T1, T2> function, T1 p1, T2 p2)
        {
            function.ThrowIfNull();
            Call(function.Target, function.Method, p1, p2);
        }

        public virtual void Call<T1, T2, T3>(Action<T1, T2, T3> function, T1 p1, T2 p2, T3 p3)
        {
            function.ThrowIfNull();
            Call(function.Target, function.Method, p1, p2, p3);
        }

        public virtual void Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> function, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            function.ThrowIfNull();
            Call(function.Target, function.Method, p1, p2, p3, p4);
        }

        private void Call(object target, MethodInfo methodInfo, params object[] args)
        {
            var objectId = Registry.GetRegisteredObjectId(target);
            var functionId = NetworkHashing.Hash(methodInfo);

            var functionDelegate = Registry.GetRegisteredFunctionDelegate(objectId, functionId);
            if (!functionDelegate.Data.Function.Authority.Contains(Connection.Role))
                return;

            var call = new NetworkFunctionCall(objectId, functionId, functionDelegate.Data.Function.Transmission);

            var payload = args.Length > 0 ? functionDelegate.Data.PayloadType.CreateInstanceFromArguments(Registry, args) : null;
            if (payload != null)
                call.EncodePayload(payload);

            Connection.Send(call);
            Queue.EnqueueDelegate(functionDelegate, payload, Connection.Role, true);
        }
    }
}
