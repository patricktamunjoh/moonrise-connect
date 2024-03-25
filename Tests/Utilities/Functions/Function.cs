using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoonriseGames.Connect.Tests.Utilities.Functions
{
    internal static class Function
    {
        public static T ExecuteInCollectableScope<T>(Func<T> constructor) => constructor.Invoke();

        public static void ClearScene()
        {
            foreach (var transform in Object.FindObjectsOfType<Transform>(true))
                Object.DestroyImmediate(transform.gameObject);
        }
    }
}
