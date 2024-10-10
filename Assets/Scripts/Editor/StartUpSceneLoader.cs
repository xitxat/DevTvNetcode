using System;
using UnityEditor;
using UnityEditor.SceneManagement ;


[InitializeOnLoad]
public static class StartUpSceneLoader 
{
    // Called Automatically on Scene Load
    static StartUpSceneLoader()
    {
        // On Change to Editors Play mode
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange state)
    {
        // Change editor scene or start playing
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            // Force Popup
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        // Entering Play
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // !if Bootstrap scene[0] load Bootstrap
            {
                if(EditorSceneManager.GetActiveScene().buildIndex != 0)
                {
                    EditorSceneManager.LoadScene(0);
                }
            }
        }
    }
}
