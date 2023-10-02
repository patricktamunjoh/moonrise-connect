using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Collections;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Hashing;
using MoonriseGames.CloudsAhoyConnect.Logging;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoonriseGames.CloudsAhoyConnect.Functions {

    internal class NetworkFunctionRegistry {

        public const ulong NULL_OBJECT_ID = 0;

        public virtual Snapshot Snapshot { get; set; }

        private Counter RegistrationCounter { get; } = new(NULL_OBJECT_ID + 1);

        private DoubleKeyMap<ulong, NetworkHash, NetworkFunctionDelegate> ObjectFunctions { get; } = new();
        private BidirectionalMap<WeakComparableReference<object>, ulong> ObjectReferences { get; } = new();

        private Dictionary<Type, NetworkFunctionData[]> FunctionCache { get; } = new();

        public virtual void RegisterAllGameObjects() {
            var objects = Object.FindObjectsOfType<Transform>(true).Select(x => (x, x.FullName())).OrderBy(x => x.Item2).ToList();
            var registeredObjects = objects.Where(x => RegisterGameObject(x.Item1.gameObject)).Select(x => x.Item2).ToList();

            if (registeredObjects.Distinct().Count() != registeredObjects.Count) {
                var duplicates = registeredObjects.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);
                var duplicateNames = string.Join(", ", duplicates);

                var message = $@"The scene contains multiple networked game objects with the same name: {duplicateNames}. 
                    Make sure each networked game object in the scene has a unique name.
                    This is necessary to ensure a total order and to correctly assign ids to each object";

                throw new InvalidOperationException(message.TrimIndents());
            }
        }

        public virtual void ClearRegistrationsAndResetCounter() {
            RegistrationCounter.Reset();
            ClearRegistrations();
        }

        public virtual void ClearRegistrations() {
            ObjectFunctions.Clear();
            ObjectReferences.Clear();

            Snapshot?.RecordObjectUnregistration(null, null, "CLEARED");
        }

        public virtual bool RegisterGameObject(GameObject target, bool doRegisterChildObjects) {
            var wasRegistered = RegisterGameObject(target.ThrowIfNull());

            if (!doRegisterChildObjects) return wasRegistered;
            return target.ChildGameObjects().Aggregate(false, (current, x) => RegisterGameObject(x) | current) | wasRegistered;
        }

        private bool RegisterGameObject(GameObject target) =>
            target.ThrowIfNull().MonoBehaviours().Aggregate(false, (current, x) => RegisterObject(x) | current);

        public virtual bool RegisterObject(object target) {
            target.ThrowIfNull();

            if (ObjectReferences.Contains(target)) {
                var message = $@"The object {target} is already registered. 
                    Make sure to register objects only once during their lifetime.";

                throw new ArgumentException(message.TrimIndents());
            }

            var objectId = RegistrationCounter.Value;
            var isRegistrationRequired = false;

            foreach (var type in target.GetType().InheritedTypes()) {
                var isSuccessful = RegisterObjectWithType(target, objectId, type);
                if (isSuccessful) isRegistrationRequired = true;
            }

            if (isRegistrationRequired) {
                ObjectReferences[target] = RegistrationCounter.ReadAndIncrease();
                Snapshot?.RecordObjectRegistration(objectId, target);
            }

            return isRegistrationRequired;
        }

        public virtual bool UnregisterObject(object target) {
            target.ThrowIfNull();

            if (!ObjectReferences.Contains(target)) return false;
            Snapshot?.RecordObjectUnregistration(ObjectReferences[target], target);

            ObjectFunctions.Remove(ObjectReferences[target]);
            ObjectReferences.Remove(target);
            return true;
        }

        private bool RegisterObjectWithType(object target, ulong objectId, Type type) {
            var functionData = NetworkFunctionDataForType(type);

            var isNetworkObject = type.IsDefined(typeof(NetworkObject));
            if (functionData == null || functionData.Length == 0) return isNetworkObject;

            foreach (var data in functionData) RegisterObjectFunction(target, objectId, data);
            return true;
        }

        private void RegisterObjectFunction(object target, ulong objectId, NetworkFunctionData functionData) {
            var functionId = NetworkHashing.Hash(functionData.MethodInfo);

            if (ObjectFunctions.Contains(objectId, functionId)) {
                var message = $@"The object {target} contains multiple instances of the {functionData.MethodInfo.Name} network function.
                    Method overloading is not supported for network functions.
                    Make sure to give each overload a unique name or merge them into one function.";

                throw new InvalidOperationException(message.TrimIndents());
            }

            var functionDelegate = new NetworkFunctionDelegate(target, functionData);
            ObjectFunctions[objectId, functionId] = functionDelegate;
        }

        private NetworkFunctionData[] NetworkFunctionDataForType(Type type) {
            if (FunctionCache.TryGetValue(type, out var functions)) return functions;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
            functions = FilteredNetworkFunctionData(type.GetMethods(flags));

            FunctionCache[type] = functions;
            return functions;
        }

        private NetworkFunctionData[] FilteredNetworkFunctionData(IEnumerable<MethodInfo> methodInfo) => methodInfo
            .Select(x => (x, (NetworkFunction)x.GetCustomAttribute(typeof(NetworkFunction)))).Where(pair => pair.Item2 != null)
            .Select(pair => new NetworkFunctionData(pair.Item2, pair.Item1)).ToArray();

        public virtual ulong GetRegisteredObjectId(object target) {
            if (target == null) return NULL_OBJECT_ID;

            if (!ObjectReferences.Contains(target)) {
                var message = $@"The requested object {target} is not registered. 
                    Make sure to register all objects before calling network function on them or passing them as arguments.
                    Use the registration extension functions to register objects.";

                throw new ArgumentException(message.TrimIndents());
            }

            return ObjectReferences[target];
        }

        public virtual object GetRegisteredObject(ulong objectId) {
            if (!ObjectReferences.Contains(objectId)) {
                if (objectId < RegistrationCounter.Value) return null;

                var message = $@"The requested object id {objectId} is not registered. 
                    Make sure to register all objects before calling network function on them or passing them as arguments.
                    Use the registration extension functions to register objects.";

                throw new ArgumentException(message.TrimIndents());
            }

            return ObjectReferences[objectId].Target;
        }

        public virtual NetworkFunctionDelegate GetRegisteredFunctionDelegate(ulong objectId, NetworkHash functionId) {
            if (!ObjectFunctions.Contains(objectId)) {
                if (objectId < RegistrationCounter.Value) return null;

                var message = $@"There are no function registered for the requested object id {objectId}. 
                    Make sure to register all objects before calling network function on them or passing them as arguments.
                    Use the registration extension functions to register objects.";

                throw new ArgumentException(message.TrimIndents());
            }

            if (!ObjectFunctions.Contains(objectId, functionId)) {
                var message = $@"The requested function with hash {functionId.GetHashCode()} was not found for the given object {objectId}. 
                    This can happen when the states of different game instances have diverged.
                    Make sure networked objects exist in all game instances and are always registered in the exact same order.";

                throw new ArgumentException(message.TrimIndents());
            }

            return ObjectFunctions[objectId, functionId];
        }
    }
}
