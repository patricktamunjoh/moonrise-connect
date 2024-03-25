using System;

namespace MoonriseGames.Connect
{
    public static class Invocation
    {
        /// <summary>Calls a network function without parameters, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Call(Action function) => Session.Instance?.Emitter.Call(function);

        /// <summary>Calls a network function with one parameter, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        /// <param name="p1">The argument values the function is invoked with.</param>
        public static void Call<T1>(Action<T1> function, T1 p1) => Session.Instance?.Emitter.Call(function, p1);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        /// <param name="p1">The first argument values the function is invoked with.</param>
        /// <param name="p2">The second argument values the function is invoked with.</param>
        public static void Call<T1, T2>(Action<T1, T2> function, T1 p1, T2 p2) => Session.Instance?.Emitter.Call(function, p1, p2);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        /// <param name="p1">The first argument values the function is invoked with.</param>
        /// <param name="p2">The second argument values the function is invoked with.</param>
        /// <param name="p3">The third argument values the function is invoked with.</param>
        public static void Call<T1, T2, T3>(Action<T1, T2, T3> function, T1 p1, T2 p2, T3 p3) => Session.Instance?.Emitter.Call(function, p1, p2, p3);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        /// <param name="p1">The first argument values the function is invoked with.</param>
        /// <param name="p2">The second argument values the function is invoked with.</param>
        /// <param name="p3">The third argument values the function is invoked with.</param>
        /// <param name="p4">The fourth argument values the function is invoked with.</param>
        public static void Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> function, T1 p1, T2 p2, T3 p3, T4 p4) => Session.Instance?.Emitter.Call(function, p1, p2, p3, p4);
    }
}
