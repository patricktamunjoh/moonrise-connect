using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Extensions
{
    public static class RegistrationExtensions
    {
        /// <summary>Registers a game object to send and receive network function calls.</summary>
        /// <param name="target">The game object to be registered.</param>
        /// <param name="doRegisterChildObjects">When true, all child objects in the hierarchy are registered as well.</param>
        public static GameObject Register(this GameObject target, bool doRegisterChildObjects = false)
        {
            Session.Instance?.Registry.RegisterGameObject(target, doRegisterChildObjects);
            return target;
        }

        /// <summary>Registers the game object of a component to send and receive network function calls.</summary>
        /// <param name="target">The component of the game object to be registered.</param>
        /// <param name="doRegisterChildObjects">When true, all child objects in the hierarchy are registered as well.</param>
        public static T RegisterGameObject<T>(this T target, bool doRegisterChildObjects = false)
            where T : Component
        {
            Session.Instance?.Registry.RegisterGameObject(target.gameObject, doRegisterChildObjects);
            return target;
        }

        /// <summary>Registers an object instance to send and receive network function calls.</summary>
        /// <param name="target">The object to be registered.</param>
        public static T RegisterInstance<T>(this T target)
            where T : class
        {
            Session.Instance?.Registry.RegisterObject(target);
            return target;
        }

        /// <summary>Unregisters an object instance from the network.</summary>
        /// <param name="target">The object to be unregistered.</param>
        public static T UnregisterInstance<T>(this T target)
            where T : class
        {
            Session.Instance?.Registry.UnregisterObject(target);
            return target;
        }
    }
}
