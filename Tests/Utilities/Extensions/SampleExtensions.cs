using System;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions
{
    internal static class SampleExtensions
    {
        public static MethodInfo GetDeclaredMethod(this Type type, string name) => type.GetMethod(name, Instance | DeclaredOnly | Public | NonPublic);

        public static MethodInfo GetDeclaredMethod(this Type type, string name, Type[] parameters) =>
            type.GetMethod(name, 0, Instance | DeclaredOnly | Public | NonPublic, null, parameters ?? Type.EmptyTypes, null);
    }
}
