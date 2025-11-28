using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameHUD : MonoBehaviour
{
    [Header("Refs")]
    public HandUI handUI;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI costText;
    public Button endTurnButton;
    public TextMeshProUGUI enemyScoreText;
    private float _timer;
    private bool _timerRunning;
    private int _currentCost;
    private void OnEnable()
    {
        GameEvents.OnTurnStart += HandleTurnStart;
        GameEvents.OnScoreUpdated += HandleScoreUpdated;
        GameEvents.OnGameEnd += HandleGameEnd;
        GameEvents.OnHandChanged += HandleHandChanged;
    }
    private void OnDisable()
    {
        GameEvents.OnTurnStart -= HandleTurnStart;
        GameEvents.OnScoreUpdated -= HandleScoreUpdated;
        GameEvents.OnGameEnd -= HandleGameEnd;
        GameEvents.OnHandChanged -= HandleHandChanged;
    }
    private void HandleTurnStart(int turnNumber, float timeRemaining, int cost)
    {
        _timer = timeRemaining;
        _timerRunning = true;
        _currentCost = cost;
        turnText.text = $"Turn {turnNumber}/{GameState.MaxTurns}";
        timerText.text = _timer.ToString("0.0");
        costText.text = $"Cost: {cost}";
        endTurnButton.interactable = true;
    }
    private void HandleScoreUpdated(int p0, int p1)
    {
        if (NetworkPlayer.LocalPlayer == null)
        {
            scoreText.text = $"P0: {p0}  P1: {p1}";
            if (enemyScoreText != null)
                enemyScoreText.text = $"Score: {p1}";
            return;
        }
        int myIndex = NetworkPlayer.LocalPlayer.playerIndex;
        int myScore = myIndex == 0 ? p0 : p1;
        int oppScore = myIndex == 0 ? p1 : p0;

        // displays player score
        scoreText.text = $"Score: {myScore}";

        // displays opp score
        if (enemyScoreText != null)
            enemyScoreText.text = $"Score: {oppScore}";
    }
    private void HandleGameEnd(int winnerIndex)
    {
        _timerRunning = false;
        endTurnButton.interactable = false;
        string msg;
        if (winnerIndex == -1)
        {
            msg = "Draw!";
        }
        else
        {
            int myIndex = NetworkPlayer.LocalPlayer != null
                ? NetworkPlayer.LocalPlayer.playerIndex
                : -1;
            if (myIndex == -1)
            {
                msg = winnerIndex == 0 ? "Player 1 Won" : "Player 2 Won";
            }
            else if (winnerIndex == myIndex)
            {
                msg = "You Won!";
            }
            else
            {
                msg = "You Lost!";
            }
        }
        turnText.text = msg;
    }
    private void HandleHandChanged(int playerIndex, List<int> hand)
    {
        Debug.Log($"[GameHUD] Hand changed for player {playerIndex}, count={hand.Count}");
        if (handUI != null)
        {
            handUI.SetHand(hand);
        }
        else
        {
            Debug.LogError("[GameHUD] handUI is NULL");
        }
    }
    private void Update()
    {
        if (!_timerRunning) return;
        _timer -= Time.deltaTime;
        if (_timer < 0f) _timer = 0f;
        timerText.text = _timer.ToString("0.0");
    }
    public void OnEndTurnClicked()
    {
        if (NetworkPlayer.LocalPlayer == null) return;
        var selected = handUI.GetSelectedCards();
        Debug.Log($"[GameHUD] EndTurn clicked. Selected {selected.Count} cards.");
        NetworkPlayer.LocalPlayer.LocalRequestEndTurn(selected);
        endTurnButton.interactable = false;
    }
}
