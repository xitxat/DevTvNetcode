using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{


    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;

    private bool isMatchMaking;
    private bool isCancelling;


    private void Start()
    {
        // Set for Clients only
        if (ClientSingleton.Instance == null) { return;  }

        // Set cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // Clear display text
        queueStatusText.text = string.Empty;
        queueTimerText.text = string.Empty;
    }


    // BUTTON FIND MATCH click
    public async void FindMatchPress()
    {
        if (isCancelling) { return; }

        // Qing
        if (isMatchMaking) {
            queueStatusText.text = "Cancelling ...";
            isCancelling = true;

            // Cancel Matchmaking
            isCancelling = false;
            isMatchMaking = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;

            return; 
        }

        // while not alrewady in Q
        // Start Q MatchMaking
        // Pass in Event when method is called
        ClientSingleton.Instance.GameManager.MatchMakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching ...";
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
        await HostSingleton.Instance.GameManager.StartHostAsync(); 

     
    }    
    
    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text); 

     
    }


}
