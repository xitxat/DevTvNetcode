using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{


    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle teamToggle; //MatchMakeAsync(teamToggle.isOn, OnMatchMade)
    [SerializeField] private Toggle privateToggle;

    private float timeInQueue; // Display
    private bool isMatchMaking;
    private bool isBusy; // no access to other buttons
    private bool isCancelling;


    private void Start()
    {
        // Set for Client / Host only
        if (ClientSingleton.Instance == null) { return;  }

        // Set cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // Clear display text
        queueStatusText.text = string.Empty;
        queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchMaking)
        {
            timeInQueue += Time.deltaTime;
            // Conversion to HMS
            TimeSpan ts = TimeSpan.FromSeconds(timeInQueue);
            // Format: {0} ~ first index, how many decimal places
            queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
    }



    // BUTTON FIND MATCH click (set ref in Canvas)
    public async void FindMatchPressed()
    {
        if (isCancelling) { return; }

        // Qing
        if (isMatchMaking) 
        {
            queueStatusText.text = "Cancelling ...";
            isCancelling = true;

            // Cancel Matchmaking
            await ClientSingleton.Instance.GameManager.CancelMatchMaking();

            isCancelling = false;
            isMatchMaking = false;
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;

            return; 
        }

        if (isBusy) { return; }

        // while not alrewady in Q
        // Start Q MatchMaking
        // Pass in Event when method is called
        ClientSingleton.Instance.GameManager.MatchMakeAsync(teamToggle.isOn, OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching ...";
        timeInQueue = 0f;
        isMatchMaking = true;
    }

    // Pass enum result in on Match
    private void OnMatchMade(MatchmakerPollingResult result)
    {


        switch (result) 
        {
            case MatchmakerPollingResult.Success:
                // as we change scene and connect
                queueStatusText.text = "Connecting ...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "TicketCreationError";
                break;            
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "TicketCancellationError";
                break;            
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "MatchAssignmentError";
                break;




        }
    }

    //  ALWAYS Access class Host/ Client  GameManagers  via Singleton.
    public async void StartHost()
    {
        if (isBusy) { return; }

        isBusy = true;

        await HostSingleton.Instance.GameManager.StartHostAsync(privateToggle.isOn);

        // incase of Error & still onMain Menu
        isBusy = false;
     
    }    
    
    public async void StartClient()
    {
        if (isBusy) { return; }

        isBusy = true;

        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text); 

        isBusy = false;

    }

    public async void JoinAsync(Lobby lobby)
    {
        // Automatically assin the Join Code
        try
        {
            //  Handle Join Button spam/mash
            if (isBusy) { return; }
            isBusy = true;


            // joiningLobby contains the JoinCode from -> (lobby.Id)
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            // Extract Join Code
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            // Pass in JCode to client
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);


        }
        catch (LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
        }

        isBusy = false;
    }


}
