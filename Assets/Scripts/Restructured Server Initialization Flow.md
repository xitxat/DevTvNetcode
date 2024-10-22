Issue:
- `ServerSingleton.Instance.GameManager` is null during the execution of `OnNetworkSpawn` on the server side.
- This prevents proper server initialization and causes networked operations like player spawning to fail.

Root Cause:
- The server initialization and scene loading were not properly synchronized. Specifically, the `ServerSingleton` and `GameManager` were being initialized after the game scene was loaded, leading to network operations trying to access the `GameManager` before it was fully set up.

Steps to Solve the Issue:

1. Restructured Server Initialization Flow:
   - Moved the `CreateServer` call to before the scene load in `ApplicationController.cs`, ensuring that the `ServerGameManager` and all server-related components are initialized before any networked objects are spawned in the scene.

2. Ensured `GameManager` is Not Null Before Proceeding:
   - Added explicit checks to confirm that `GameManager` is properly initialized after calling `CreateServer`.
   - If `GameManager` is null after initialization, we log an error and stop further execution, preventing potential runtime failures.

3. Enhanced Error Logging for Server Creation:
   - In `CreateServer`, added more detailed error handling to catch any issues that may prevent the `ServerGameManager` from being initialized. This ensures that failures during server setup are clearly visible in the logs.

4. Adjusted the Flow of Scene Loading:
   - Modified the flow to:
     1. Instantiate the `ServerSingleton`.
     2. Initialize the server by calling `CreateServer`.
     3. Ensure that `GameManager` is properly initialized.
     4. Only then, load the game scene and start the server.

5. Added Detailed Execution Flow for "Find Match" Button:
   - Documented the full execution order from the "Find Match" button press, through server instantiation, scene loading, and network server startup.

Outcome:
- With these changes, we ensure that `ServerSingleton.Instance.GameManager` is properly initialized before any network operations, preventing `null` reference errors during player spawning.
