using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Payloads {

    internal interface INetworkPayload {

        public IEnumerable<object> Arguments();
    }

    [Serializable]
    internal class NetworkPayload<T1, T2, T3, T4> : INetworkPayload {

        [SerializeField] private T1 _v1;
        [SerializeField] private T2 _v2;
        [SerializeField] private T3 _v3;
        [SerializeField] private T4 _v4;

        public NetworkPayload(T1 v1 = default, T2 v2 = default, T3 v3 = default, T4 v4 = default) {
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
            _v4 = v4;
        }

        IEnumerable<object> INetworkPayload.Arguments() {
            yield return _v1;
            yield return _v2;
            yield return _v3;
            yield return _v4;
        }
    }
}
