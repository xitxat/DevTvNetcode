# Execution Flow

1. **MainMenu.cs**:
   - **Method**: `FindMatchPress()`
     - User clicks the "Find Match" button.
     - Calls: `ClientSingleton.Instance.GameManager.MatchMakeAsync(teamToggle.isOn, OnMatchMade)`
     - This sends a request to matchmake the client, and the matchmaking process starts.
     - If the matchmaking is successful, the server is initialized (on the server-side), and the game scene is loaded for the client.

2. **Server Initialization**: (Dedicated Server Mode)
   - **Class**: `ApplicationController.cs`
   - **Method**: `Start()`
     - Called when the server starts, checks if the server is running in headless mode (dedicated server) via:
       - `await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)`

3. **ApplicationController.cs**:
   - **Method**: `LaunchInMode(isDedicatedServer)`
     - This determines whether the server is running in dedicated mode or host mode.
     - If the system is in **dedicated server mode**:
       - Instantiate `ServerSingleton`: `ServerSingleton serverSingleton = Instantiate(serverPrefab);`
       - Call: `StartCoroutine(LoadGameSceneAsync(serverSingleton))`

4. **ApplicationController.cs**:
   - **Coroutine**: `LoadGameSceneAsync(serverSingleton)`
     - Load the "Game" scene asynchronously using:
       - `AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName)`
     - Wait until the scene is fully loaded (yield until `asyncOperation` is done).

     - After the scene is loaded:
       - Initialize the server by calling:
         - `Task createServerTask = serverSingleton.CreateServer(playerPrefab)`
         - Wait until `CreateServer` completes:
           - `yield return new WaitUntil(() => createServerTask.IsCompleted)`

     - Check if `GameManager` is initialized:
       - If `serverSingleton.GameManager == null`:
         - Log error: "GameManager is null after CreateServer."

     - Start the game server after scene load and server initialization:
       - `Task startServerTask = serverSingleton.GameManager.StartGameServerAsync()`
       - Wait until the server starts:
         - `yield return new WaitUntil(() => startServerTask.IsCompleted)`

5. **ServerSingleton.cs**:
   - **Method**: `CreateServer(NetworkObject playerPrefab)`
     - Initializes Unity Game Services (UGS).
     - Initializes `NetworkManager`, checks if it's not null.
     - Creates a new instance of `ServerGameManager`:
       - `GameManager = new ServerGameManager(...)`
     - Log: "ServerGameManager created successfully."

6. **ServerGameManager.cs**:
   - **Constructor**: `ServerGameManager(...)`
     - Initializes the network server with parameters like IP, port, and `playerPrefab`.
     - Sets up the network server for handling incoming connections and player spawning.

7. **NetworkServer.cs**:
   - **Method**: `OpenConnection()`
     - Opens the network server connection on the specified IP and port.
     - Logs if the server successfully starts or fails.
     - Starts handling matchmaking requests from clients.

8. **MainMenu.cs**:
   - **Method**: `OnMatchMade(MatchmakerPollingResult result)`
     - Called once matchmaking is completed (success or failure).
     - On success:
       - Update UI: `queueStatusText.text = "Connecting ..."`
       - Transition the client to the game scene and connect to the server.
