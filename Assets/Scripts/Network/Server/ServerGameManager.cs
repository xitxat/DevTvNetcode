using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

// serverPort : game
// serverQPort: analyitics
public class ServerGameManager : IDisposable
{




    public async Task StartGameServerAsync()
    {
        throw new NotImplementedException();
    }


    public void Dispose()
    {

    }
}