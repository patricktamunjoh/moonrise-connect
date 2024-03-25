using System;
using System.Text;
using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Extensions;
using MoonriseGames.Connect.Hashing;
using MoonriseGames.Connect.Payloads;
using UnityEngine;

namespace MoonriseGames.Connect.Functions
{
    internal class NetworkFunctionCall
    {
        private int FunctionIdLength => NetworkHashing.HashSizeBytes;
        private int ObjectIdLength => 8;
        private int TransmissionLength => 1;

        private int MessageHeaderLength => FunctionIdLength + ObjectIdLength + TransmissionLength;

        public ulong ObjectId { get; private set; }
        public NetworkHash FunctionId { get; private set; }

        private byte[] Payload { get; set; }

        public Transmission Transmission { get; private set; }

        public NetworkFunctionCall(ulong objectId, NetworkHash functionId, Transmission transmission)
        {
            if (!functionId.IsValid)
            {
                const string message = "The provided function id hash is invalid.";
                throw new InvalidOperationException(message.TrimIndents());
            }

            ObjectId = objectId;
            FunctionId = functionId;
            Transmission = transmission;
        }

        public NetworkFunctionCall(byte[] bytes) => FromBytes(bytes);

        public void EncodePayload(INetworkPayload payload)
        {
            var json = JsonUtility.ToJson(payload);
            Payload = Encoding.UTF8.GetBytes(json);
        }

        public INetworkPayload DecodedPayload(Type type)
        {
            if (Payload == null || Payload.Length == 0)
                return null;
            var json = Encoding.UTF8.GetString(Payload);
            return JsonUtility.FromJson(json, type) as INetworkPayload;
        }

        public byte[] ToBytes()
        {
            var bytes = new byte[MessageHeaderLength + (Payload?.Length ?? 0)];

            bytes[MessageHeaderLength - TransmissionLength] = (byte)Transmission;

            BitConverter.GetBytes(ObjectId).CopyTo(bytes, 0);
            FunctionId.Hash.CopyTo(bytes, ObjectIdLength);
            Payload?.CopyTo(bytes, MessageHeaderLength);

            return bytes;
        }

        private void FromBytes(byte[] bytes)
        {
            if (bytes.Length < MessageHeaderLength)
            {
                const string message =
                    @"The byte array is incomplete and does not meet the required minimum length.
                    This might suggest a fault during network transmission or during handling of the message body.";

                throw new InvalidOperationException(message.TrimIndents());
            }

            ObjectId = BitConverter.ToUInt64(bytes, 0);
            FunctionId = new NetworkHash(bytes[ObjectIdLength..(MessageHeaderLength - TransmissionLength)]);
            Transmission = (Transmission)bytes[MessageHeaderLength - TransmissionLength];

            if (bytes.Length > MessageHeaderLength)
                Payload = bytes[MessageHeaderLength..];
        }
    }
}
