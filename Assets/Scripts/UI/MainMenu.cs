using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{


    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;


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
