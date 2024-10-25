using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// HOOKUP: [requires GO UI/EventSystem
// GO ConnectButton  add GO NameSelector to OnClick NS Connect()
// GO NameInputField add GO NameSelector to OnValueChanged HandleNameChanged() [in TMP component]
public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    public const string PlayerNameKey = "PlayerName"; // ref from ClientGameManager 


    void Start()
    {
        //  If dedicated server [headless(no gfx)]: skip scene
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            // Set scene change dynamically. sc++
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

            // Will check if name is already in PlayerPrefs (registered from previous game)
            // if T, set into txt field
            nameField.text =  PlayerPrefs.GetString(PlayerNameKey, string.Empty);

        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable =
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;
    }

    public void Connect()
    {
        // Set Player Prefs & go to next scene
        //  Get the players name
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        // Set scene change dynamically. sc++
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
