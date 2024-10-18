using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using Unity.Services.Authentication;

//   AUTHENTICATE PLAYER
//  IDisposable: gain access to monobehaviours
public class ClientGameManager : IDisposable
{

    private JoinAllocation allocation;
    private NetworkClient networkClient;
    private MatchplayMatchmaker matchmaker; //>Services
    private UserData userData;

    private const string MenuSceneName = "Menu";
    private const string GameSceneName = "Game";



    //  Async runs for as long as it needs
    //  Return bool for Authenticated
    public async Task<bool> InitAsync()
    {
        //  Authentications (Task)
        await UnityServices.InitializeAsync();

        //  Inst when becomming a Client &
        //  listen for NetworkClient Disconnect event
        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        // Await the 5 max tries & return state
        //  Returns AuthID
        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            // Set Data from Network Server ApprovalCheck()
            // user prefs like solo, team
            userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "NoNameO"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };

            return true;
        }

        return false;

    }


    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }



    //  START CLIENT    
    // Called by Menu Button
    // jOINsERVER VIA Lobby and Relay
    public async Task StartClientAsync(string joinCode)
    {
        // Relay Specific
        try
        {
            //  JOIN allocation with ID# (Join Code)
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        //  Unity Transport switch to RELAY MODE
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //  Set connection type ("udp" User Data Protocol = RELAY) [IP,Port]
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);




    }

    private void ConnectClient()
    {
        // Generic Server Connection
        // Repackage Package: JSON <=> Byte Array
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        // Send over network on Connecting to Server
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        //  Start
        NetworkManager.Singleton.StartClient();

        //  Scene
        //  Only Server changes the scene &
        //  Will handle client scene too.
    }

    private void StartClient()
    {

    }


    //  MATCHMAKE
    // br 5.07, 08
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);

        if(matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            // Connect to Server

        }

        return matchmakingResult.result;

    }

    // Client Disconnect
    internal void Disconnect()
    {
        networkClient.Disconnect();    }


    public void Dispose()
    {
        // Pass down the chain
        networkClient?.Dispose();

    }


}