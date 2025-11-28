using System;
[Serializable]
public class GenericMessageWrapper
{
    public string action;
}
[Serializable]
public class GameStartMessage
{
    public string action;
    public string[] playerIds;
    public int totalTurns;
}
[Serializable]
public class EndTurnMessage
{
    public string action;
    public string playerId;
    public int[] cardIds;
}
[System.Serializable]
public class StateMessage
{
    public string action;
    public int turn;
    public float timeRemaining;
    public int cost;
    public int[] p0Hand;
    public int[] p1Hand;
    public int[] p0Board;
    public int[] p1Board;
    public int p0Score;
    public int p1Score;
}
[Serializable]
public class GameEndMessage
{
    public string action;
    public int winnerIndex;
    public int p0Score;
    public int p1Score;
}
