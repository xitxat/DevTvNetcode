using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;


// static: call from anywhere
public static class AuthenticationWrapper
{

    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    //  Authenticate Player via UGS & Client Game Manager returns T/F
    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        //  Check if Authenticated
        if(AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);



        return AuthState;
    }

    //  Try Authenticate
    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;

        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            //  Straight in, no 3rd party auth, ie Google, FB
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                AuthState = AuthState.Authenticated;
                break;
            }

            // On Fail
            Debug.LogError("<color=orange>Retrying Authentication Service, �|</color>");

            tries++;
            await Task.Delay(1000);
        }
    }

}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    TimeOut,
    Error


}
