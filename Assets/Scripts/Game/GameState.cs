using System.Collections.Generic;
public class PlayerState
{
    public List<int> deck = new List<int>();
    public List<int> hand = new List<int>();
    public List<int> cardsInPlay = new List<int>();
    public int score = 0;
    public int playerIndex;
    public PlayerState(int index)
    {
        playerIndex = index;
    }
}
public class GameState
{
    public const int MaxTurns = 6;
    public int currentTurn = 1;
    public int currentPlayerCost => currentTurn;
    public PlayerState player0;
    public PlayerState player1;
    public GameState()
    {
        player0 = new PlayerState(0);
        player1 = new PlayerState(1);
    }
}
