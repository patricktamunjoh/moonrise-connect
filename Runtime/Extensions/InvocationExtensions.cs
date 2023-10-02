using System;

namespace MoonriseGames.CloudsAhoyConnect.Extensions {
    public static class InvocationExtensions {

        /// <summary>Calls a network function without parameters, invoking it on all required game instances.</summary>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Send(this object _, Action function) => CloudsAhoyConnect.Instance?.Emitter.Call(function);

        /// <summary>Calls a network function with one parameter, invoking it on all required game instances.</summary>
        /// <param name="p1">The argument values the function is invoked with.</param>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Send<T1>(this T1 p1, Action<T1> function) => CloudsAhoyConnect.Instance?.Emitter.Call(function, p1);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="parameters">A pair wrapping the arguments the function is invoked with.</param>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Send<T1, T2>(this (T1, T2) parameters, Action<T1, T2> function) =>
            CloudsAhoyConnect.Instance?.Emitter.Call(function, parameters.Item1, parameters.Item2);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="parameters">A pair wrapping the arguments the function is invoked with.</param>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Send<T1, T2, T3>(this (T1, T2, T3) parameters, Action<T1, T2, T3> function) =>
            CloudsAhoyConnect.Instance?.Emitter.Call(function, parameters.Item1, parameters.Item2, parameters.Item3);

        /// <summary>Calls a network function with two parameters, invoking it on all required game instances.</summary>
        /// <param name="parameters">A pair wrapping the arguments the function is invoked with.</param>
        /// <param name="function">The function to be invoked on the network.</param>
        public static void Send<T1, T2, T3, T4>(this (T1, T2, T3, T4) parameters, Action<T1, T2, T3, T4> function) =>
            CloudsAhoyConnect.Instance?.Emitter.Call(function, parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4);
    }
}
