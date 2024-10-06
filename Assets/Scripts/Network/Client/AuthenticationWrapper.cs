using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
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

        //  Only 1 authenticating instance allowed to run
        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("<color=yellow>Already Authenticating. Waiting... ¦|</color>");

            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);



        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        // Force wait for all tries. 
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        { await Task.Delay(250);}

        return AuthState;
    }

    //  Try Authenticate
    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;

        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            // Error Handeling
            try
            {
                //  Straight in, no 3rd party auth, ie Google, FB
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            // Fail to Auth
            catch(AuthenticationException exA)
            {
                Debug.LogError(exA);
                AuthState = AuthState.Error;
            }
            // Fail to Connect
            catch(RequestFailedException exFRE)
            {
                Debug.LogError(exFRE);
                AuthState = AuthState.Error;
            }

            // On Fail
            Debug.LogError("<color=orange>Retrying Authentication Service, ¦|</color>");

            tries++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"<color=orange>Player not signed in succesfully after {tries} tries. ¦|</color>");
            AuthState = AuthState.TimeOut;
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
