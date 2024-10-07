using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager 
{

    private Allocation allocation;
    private string joinCode;
    private const int MaxConnections = 20;
    private const string GameSceneName  = "Game";

    public async Task StartHostAsync()
    {
        try
          {
            //  CREATE, Assign & Store the allocation with ID# (Join Code)
           allocation =  await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            //   Join Code
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"<color=teal>Join Code: {joinCode}</color>");

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
        NetworkManager.Singleton.StartHost();

        //  Scene
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }
}
