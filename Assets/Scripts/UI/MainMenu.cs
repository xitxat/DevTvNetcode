using UnityEngine;

public class MainMenu : MonoBehaviour
{

    //  Access class HostGameManager  via Singleton.
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync(); 

     
    }
}
