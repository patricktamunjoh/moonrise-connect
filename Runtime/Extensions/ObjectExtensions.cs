using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoonriseGames.Connect.Extensions
{
    internal static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T target)
            where T : class
        {
            if (target == null)
                throw new ArgumentNullException();
            return target;
        }

        public static IEnumerable<GameObject> ChildGameObjects(this GameObject target) => target.GetComponentsInChildren<Transform>().Skip(1).Select(x => x.gameObject);

        public static IEnumerable<MonoBehaviour> MonoBehaviours(this GameObject target) => target.GetComponents<MonoBehaviour>();

        public static string FullName(this Transform target)
        {
            var names = new List<string>();

            do
            {
                names.Add(target.name);
            } while ((target = target.parent) != null);

            names.Reverse();
            return string.Join(".", names);
        }
    }
}
