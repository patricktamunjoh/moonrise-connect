using System;
using System.Linq;
using MoonriseGames.Connect.Extensions;

namespace MoonriseGames.Connect.Functions
{
    internal class NetworkFunctionDelegate
    {
        public NetworkFunctionData Data { get; }

        private WeakReference<object> TargetReference { get; }
        public bool HasTarget => TargetReference.TryGetTarget(out _);

        public NetworkFunctionDelegate(object target, NetworkFunctionData data)
        {
            Data = data;
            TargetReference = new WeakReference<object>(target);
        }

        public virtual void Invoke(object[] arguments)
        {
            if (!TargetReference.TryGetTarget(out var target))
                return;

            if (arguments?.Length == 0)
            {
                const string message =
                    @"The provided array of arguments is empty. 
                    If the delegate is niladic, make sure to provide null for the arguments.
                    In all other cases, the array should contain one or more argument values.";

                throw new ArgumentException(message.TrimIndents());
            }

            if ((arguments?.Length ?? 0) != Data.ParameterTypes.Length)
            {
                const string message =
                    @"The provided array of arguments does not have the expected length. 
                    Make sure to provide the exact amount of arguments that matches the delegated function.";

                throw new ArgumentException(message.TrimIndents());
            }

            Data.MethodInfo.Invoke(target, arguments);
        }

        public virtual string ToString(object[] arguments)
        {
            if (!TargetReference.TryGetTarget(out var target))
                return string.Empty;

            var argumentsString = arguments != null ? string.Join(", ", arguments.Select(x => x.ToString())) : " ";
            return $@"Invoking {target}@{Data.MethodInfo.Name}({argumentsString})";
        }
    }
}
