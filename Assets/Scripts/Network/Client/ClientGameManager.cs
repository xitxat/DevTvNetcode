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

       //   AUTHENTICATE PLAYER
public class ClientGameManager 
{

    private JoinAllocation allocation;
    private const string MenuSceneName = "Menu";
    private const string GameSceneName = "Game";



    //  Async runs for as long as it needs
    //  Return bool for Authenticated
    public async Task<bool> InitAsync()
    {
        //  Authentications (Task)
        await UnityServices.InitializeAsync();

        // Await the 5 max tries & return state
        AuthState authState =  await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;

    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    // Called by Menu Button
    public async Task StartClientAsync(string joinCode)
    {
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

        //  Start
        NetworkManager.Singleton.StartClient();

        //  Scene
        //  Only Server changes the scene &
        //  Will handle client scene too.
    }
}
