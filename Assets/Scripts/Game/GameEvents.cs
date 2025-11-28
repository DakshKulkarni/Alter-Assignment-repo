using System;
using System.Collections.Generic;

public static class GameEvents
{
    // turnNumber, timeRemaining, cost
    public static Action<int, float, int> OnTurnStart;
    // p0Score, p1Score
    public static Action<int, int> OnScoreUpdated;
    // winnerIndex 
    public static Action<int> OnGameEnd;
    // playerIndex, full hand cardIds
    public static Action<int, List<int>> OnHandChanged;
    // playerIndex, full board cardIds
    public static Action<int, List<int>> OnBoardChanged;
}
