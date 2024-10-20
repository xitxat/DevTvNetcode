using System;



public enum Map
{
    Default
}

public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo,
    Team
}


[Serializable]
public class UserData
{

    public string userName;
    public string userAuthId;

    // What the useer wants to play
    public GameInfo userGamePreferences = new GameInfo();
}


[Serializable]
public class GameInfo 
{
    public Map map;
    public GameMode gameMode;
    public GameQueue gameQueue;



    //  UGS / MATCHMAKER / Queue
    public string ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueue.Solo => "solo-queue",
            GameQueue.Team => "team-queue",

            // _ (default)
            _ => "solo-queue"
        };
    }
}

