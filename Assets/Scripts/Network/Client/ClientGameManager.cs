using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

       //   AUTHENTICATE PLAYER
public class ClientGameManager 
{
    private const string MenuSceneName = "Menu";


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
}
