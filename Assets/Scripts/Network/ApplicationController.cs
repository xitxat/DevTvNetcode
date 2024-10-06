using System;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Headless server (bool)
        LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

    }

    private void LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {

        }
    }
}
