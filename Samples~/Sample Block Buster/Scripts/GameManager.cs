using MoonriseGames.CloudsAhoyConnect;
using MoonriseGames.CloudsAhoyConnect.Connection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Steam;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

[NetworkObject]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _connectionTimeout;

    [SerializeField]
    [Space]
    private Text _networkStatusText;

    [SerializeField]
    private InputField _steamFriendText;

    [SerializeField]
    private GameObject _connectionSettingsContainer;

    [SerializeField]
    private Character _characterPrefab;

    private CloudsAhoyConnect CloudsAhoyConnect { get; set; }

    private void Awake()
    {
        // Using the builder a new instance of the library is created
        var builder = new CloudsAhoyConnect.Builder();

        // The library is configured for Steamworks
        CloudsAhoyConnect = builder.ForSteam().Build();

        // Network events can be observed by subscribing to this event
        CloudsAhoyConnect.OnNetworkConnectionChanged += OnNetworkConnectionChanged;
    }

    private void Update()
    {
        _networkStatusText.text = $"Network Status: {CloudsAhoyConnect.Connectivity}";
    }

    private void LateUpdate()
    {
        // Every frame incoming messages are collected and processed
        CloudsAhoyConnect.PollConnection();
        CloudsAhoyConnect.ProcessQueuedNetworkFunctionCalls();
    }

    private void InitializeGameSession()
    {
        // When initializing the game session the characters are spawned
        // Because SpawnCharacter has authority host, this function has no effect on the client
        // This is important, because on client instances only the connection to the host has to be established
        // However, on the host ALL clients have to be connected
        // Therefore, it should always be the host instance that kicks off the game session
        true.Send(SpawnCharacter);
        if (CloudsAhoyConnect.ConnectedClientsCount > 0)
            false.Send(SpawnCharacter);
    }

    private void CleanupGameSession()
    {
        // After each session, ensure the initial state is restored
        foreach (var character in FindObjectsOfType<Character>())
            Destroy(character.gameObject);
    }

    [NetworkFunction(Groups.Host, Recipients.All)]
    private void SpawnCharacter(bool isHostCharacter)
    {
        var spawnPosition = isHostCharacter ? Vector2.left : Vector2.right;
        var character = Instantiate(_characterPrefab, spawnPosition, Quaternion.identity);

        // Network object have to be registers to communicate
        // Here, only the character instance is registered, because the game object contains no other Network Objects
        // If the game objects or its children have multiple Network Object behaviours attached, RegisterGameObject should be used
        character.RegisterInstance();
        character.IsControlledLocally = isHostCharacter == (CloudsAhoyConnect.Role == Roles.Host);
    }

    private void OnNetworkConnectionChanged(object sender, NetworkConnectionEventArgs args)
    {
        switch (args.Type)
        {
            case NetworkConnectionEventArgs.Types.ConnectionLost:
                // On the host instance this is independent from the connection to individual client instances
                // This allows the host instance to continue the session, even if all clients disconnect
                SetConnectionSettingsVisibility(true);
                CleanupGameSession();
                break;

            case NetworkConnectionEventArgs.Types.ConnectionEstablished:
                // On the client instance this is called when connected to the host
                // On the host instance this is called when connected to ALL client instances
                SetConnectionSettingsVisibility(false);
                InitializeGameSession();
                break;

            case NetworkConnectionEventArgs.Types.ConnectionEstablishmentFailed:
                SetConnectionSettingsVisibility(true);
                break;

            case NetworkConnectionEventArgs.Types.ConnectionToClientLost:
                // This is only called on the host instance
                CloudsAhoyConnect.DropConnection();
                break;
        }
    }

    public void PlaySolo()
    {
        // For a solo session, the config should be set as host but without providing any client identities
        var builder = new SteamNetworkConnectionConfig.Builder();
        var config = builder.WithConnectionEstablishmentTimeout(_connectionTimeout).AsHost().Build();

        EstablishConnection(config);
    }

    public void HostSession()
    {
        var friend = FindSteamFriendWithName();
        if (friend == CSteamID.Nil)
            return;

        // To open a session as host, all the expected client identities must be configured beforehand
        // This is necessary to determine when everyone is connected and the session can be started
        // For Steamworks, clients can be identified using their SteamID
        var builder = new SteamNetworkConnectionConfig.Builder();
        var config = builder.WithConnectionEstablishmentTimeout(_connectionTimeout).AsHost(friend).Build();

        EstablishConnection(config);
    }

    public void JoinSession()
    {
        var friend = FindSteamFriendWithName();
        if (friend == CSteamID.Nil)
            return;

        // To connect to a session as client, only the identity of the host must be configured
        // It is important that the session already exists, before any client attempts to join
        var builder = new SteamNetworkConnectionConfig.Builder();
        var config = builder.WithConnectionEstablishmentTimeout(_connectionTimeout).AsClient(friend).Build();

        EstablishConnection(config);
    }

    private void EstablishConnection(SteamNetworkConnectionConfig config)
    {
        SetConnectionSettingsVisibility(false);

        // Before establishing a new connection, all registered objects must be cleared
        // Failing to do so will prevent function calls to be delivered to the correct objects
        CloudsAhoyConnect.Reset();

        // Here, all Network Objects in the scene are registered
        // This ensures that all game instances have the same initial state
        // This also allows one of the pre-existing Network Objects to initialize the game
        CloudsAhoyConnect.RegisterAllGameObjects();

        CloudsAhoyConnect.EstablishConnection(config);
    }

    private void SetConnectionSettingsVisibility(bool isVisible)
    {
        if (_connectionSettingsContainer.activeSelf == isVisible)
            return;
        _connectionSettingsContainer.SetActive(isVisible);
    }

    private CSteamID FindSteamFriendWithName()
    {
        var friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
        var expectedName = _steamFriendText.text.ToLower();

        for (var i = 0; i < friendCount; i++)
        {
            var friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
            var friendName = SteamFriends.GetFriendPersonaName(friend);

            if (string.IsNullOrEmpty(friendName))
                continue;
            if (friendName.ToLower().Equals(expectedName))
                return friend;
        }

        if (_steamFriendText.placeholder is Text placeholder)
            placeholder.text = "Not found...";
        _steamFriendText.text = string.Empty;
        return CSteamID.Nil;
    }
}
