using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Payloads
{
    [Serializable]
    internal class NetworkPayloadType
    {
        private int ArgumentCount { get; }
        private int ParameterCount => typeof(NetworkPayload<,,,>).GetTypeInfo().GenericTypeParameters.Length;

        private Type PayloadType { get; }
        private bool[] ObjectReplacementPositions { get; }

        public NetworkPayloadType(params Type[] parameterTypes)
        {
            ArgumentCount = parameterTypes.Length;
            PayloadType = RemotablePayloadType(parameterTypes, out var replacedPositions);
            ObjectReplacementPositions = replacedPositions;
        }

        public INetworkPayload CreateInstanceFromArguments(NetworkFunctionRegistry registry, params object[] arguments)
        {
            if (arguments.Length != ArgumentCount)
            {
                var message = $@"The number of supplied arguments does not match the expected count of {ArgumentCount}.";
                throw new ArgumentException(message.TrimIndents());
            }

            var replacedArguments = arguments
                .Zip(ObjectReplacementPositions, (parameter, isReplaced) => (parameter, isReplaced))
                .Select(x => x.isReplaced ? registry.GetRegisteredObjectId(x.parameter) : x.parameter);

            var payloadArguments = FillMissingValues(replacedArguments, null, ParameterCount - arguments.Length).ToArray();

            return Activator.CreateInstance(PayloadType, payloadArguments) as INetworkPayload;
        }

        public object[] RecoverArgumentsFromInstance(NetworkFunctionRegistry registry, INetworkPayload payload) =>
            payload
                ?.Arguments()
                .Take(ArgumentCount)
                .Zip(ObjectReplacementPositions, (arg, isReplaced) => (arg, isReplaced))
                .Select(x => x.isReplaced ? registry.GetRegisteredObject((ulong)x.arg) : x.arg)
                .ToArray();

        private Type RemotablePayloadType(IReadOnlyCollection<Type> parameterTypes, out bool[] replacedPositions)
        {
            if (parameterTypes.Count > ParameterCount)
            {
                var message =
                    $@"The number of supplied type parameters exceeds the maximum value of {ParameterCount}.
                    Make sure all network function have {ParameterCount} or less input parameters.";

                throw new ArgumentException(message.TrimIndents());
            }

            var replacedTypes = parameterTypes.Select(x => (Type: RemotableParameterType(x, out var isReplaced), isReplaced)).ToList();

            replacedPositions = replacedTypes.Select(x => x.isReplaced).ToArray();

            var remotableTypes = replacedTypes.Select(x => x.Type);
            var finalTypes = FillMissingValues(remotableTypes, typeof(object), ParameterCount - parameterTypes.Count).ToArray();

            return typeof(NetworkPayload<,,,>).MakeGenericType(finalTypes);
        }

        private Type RemotableParameterType(Type type, out bool isReplaced)
        {
            isReplaced = type.IsDefined(typeof(NetworkObject));

            if (!isReplaced && !IsSerializable(type))
            {
                var message =
                    $@"Objects of type {type} cannot be used as arguments to network functions.
                    Only {nameof(NetworkObject)}, serializable classes or structs, and primitive data types are supported.";

                throw new ArgumentException(message.TrimIndents());
            }

            return isReplaced ? typeof(ulong) : type;
        }

        private bool IsSerializable(Type type)
        {
            if (type.IsDefined(typeof(SerializableAttribute)))
                return true;
            return IsPrimitiveType(type) || IsSerializableBuiltInType(type) || IsSerializableCollectionType(type);
        }

        private bool IsPrimitiveType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(string))
                return true;
            return false;
        }

        private bool IsSerializableBuiltInType(Type type)
        {
            if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) || type == typeof(Quaternion))
                return true;
            if (type == typeof(Vector2Int) || type == typeof(Vector3Int))
                return true;
            if (type == typeof(Matrix4x4) || type == typeof(Color) || type == typeof(Rect) || type == typeof(LayerMask))
                return true;
            return false;
        }

        private bool IsSerializableCollectionType(Type type)
        {
            if (type.IsArray && IsSerializable(type.GetElementType()))
                return true;
            if (type == typeof(List<>) && IsSerializable(type.GetGenericArguments().Single()))
                return true;
            return false;
        }

        private IEnumerable<T> FillMissingValues<T>(IEnumerable<T> types, T value, int size) => types.Concat(Enumerable.Repeat(value, size));

        public static implicit operator Type(NetworkPayloadType payloadType) => payloadType.PayloadType;
    }
}
