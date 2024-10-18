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
        // Start Q
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching ...";
        isMatchMaking = true;
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
