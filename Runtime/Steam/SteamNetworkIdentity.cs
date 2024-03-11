using System;
using MoonriseGames.CloudsAhoyConnect.Connection;
using Steamworks;

namespace MoonriseGames.CloudsAhoyConnect.Steam
{
    /// <summary>
    /// Represents a games instance within the network. Each game instance belongs to exactly one Steam user. Create network identities
    /// from the corresponding Steam id of the targeted user. Steam ids can be implicitly cast to a network identities.
    /// </summary>
    public class SteamNetworkIdentity : NetworkIdentity
    {
        private ulong SteamId { get; }

        /// <summary>
        /// The steam display name belonging to this identity. Steam names are not unique and should not be used to identity game instances.
        /// The name might also change if the user updates their Steam display name.
        /// </summary>
        public string DisplayName => SteamProxy.Instance.DisplayName(this);

        /// <summary>Creates a network identity from a Steam user id as unsigned long.</summary>
        /// <param name="steamId">Defines the Steam id of the targeted Steam user.</param>
        public SteamNetworkIdentity(ulong steamId) => SteamId = steamId;

        /// <summary>Creates a network identity from a Steam user id.</summary>
        /// <param name="steamId">Defines the Steam id of the targeted Steam user.</param>
        public SteamNetworkIdentity(CSteamID steamId) => SteamId = steamId.m_SteamID;

        public override bool Equals(object obj) => obj is SteamNetworkIdentity identity && identity.SteamId == SteamId;

        public override int GetHashCode() => HashCode.Combine(SteamId);

        public static implicit operator SteamNetworkingIdentity(SteamNetworkIdentity identity)
        {
            var id = new SteamNetworkingIdentity();
            id.SetSteamID(identity);
            return id;
        }

        public static implicit operator SteamNetworkIdentity(CSteamID steamID) => new(steamID);

        public static implicit operator SteamNetworkIdentity(ulong steamID) => new(steamID);

        public static implicit operator CSteamID(SteamNetworkIdentity identity) => new(identity.SteamId);
    }
}
