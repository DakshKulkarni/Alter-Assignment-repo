using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public GameState State { get; private set; }
    [Header("Turn Settings")]
    public float turnDuration = 30f;
    private float _turnTimer;
    private bool _turnRunning;
    private int _playersEndedTurn;
    public List<int> p0SelectedThisTurn = new List<int>();
    public List<int> p1SelectedThisTurn = new List<int>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void StartGame(List<int> deckP0, List<int> deckP1)
    {
        State = new GameState();
        State.player0.deck = new List<int>(deckP0);
        State.player1.deck = new List<int>(deckP1);

        // first draw by default hands 3 cards to players
        DrawCards(State.player0, 3);
        DrawCards(State.player1, 3);

        //we ensure both have one cost cards in the first hand
        EnsureCheapInHand(State.player0);
        EnsureCheapInHand(State.player1);

        // ensuring different 3 cards for the first time
        MakeStartingHandsDifferent(State.player0, State.player1);
        State.currentTurn = 1;
        StartTurn();
    }
    private void EnsureCheapInHand(PlayerState player)
    {
        bool hasCheap = false;

        foreach (int id in player.hand)
        {
            var def = CardDatabase.Instance.GetCard(id);
            if (def != null && def.cost == 1)
            {
                hasCheap = true;
                break;
            }
        }
        if (hasCheap) return;
        for (int i = 0; i < player.deck.Count; i++)
        {
            var def = CardDatabase.Instance.GetCard(player.deck[i]);
            if (def != null && def.cost == 1)
            {
                int cheapId = player.deck[i];
                player.deck.RemoveAt(i);
                player.hand.Add(cheapId);
                Debug.Log("[GameController] Injected cheap card into starting hand.");
                return;
            }
        }
    }
    private void MakeStartingHandsDifferent(PlayerState p0, PlayerState p1)
    {
        if (p0.hand.Count != p1.hand.Count) return;

        bool identical = true;
        for (int i = 0; i < p0.hand.Count; i++)
        {
            if (p0.hand[i] != p1.hand[i])
            {
                identical = false;
                break;
            }
        }
        if (!identical) return;
        if (p1.deck.Count == 0) return;
        int swapFromDeck = p1.deck[p1.deck.Count - 1];
        p1.deck.RemoveAt(p1.deck.Count - 1);
        int replaced = p1.hand[p1.hand.Count - 1];
        p1.hand[p1.hand.Count - 1] = swapFromDeck;
        p1.deck.Add(replaced);

        Debug.Log("[GameController] Starting hands were identical – adjusted player1's hand.");
    }
    private void StartTurn()
    {
        if (State.currentTurn > GameState.MaxTurns)
        {
            EndGame();
            return;
        }
        _playersEndedTurn = 0;
        p0SelectedThisTurn.Clear();
        p1SelectedThisTurn.Clear();
        if (State.currentTurn > 1)
        {
            DrawCards(State.player0, 1);
            DrawCards(State.player1, 1);
        }
        _turnTimer = turnDuration;
        _turnRunning = true;
        BroadcastFullState();
    }
    private void Update()
    {
        if (!NetworkServer.active) return;
        if (!_turnRunning || State == null) return;
        _turnTimer -= Time.deltaTime;
        if (_turnTimer <= 0f)
        {
            _turnRunning = false;
            HandleBothPlayersEndedTurn();
        }
    }
    private void DrawCards(PlayerState player, int count)
    {
        for (int i = 0; i < count && player.deck.Count > 0; i++)
        {
            int index = Random.Range(0, player.deck.Count);
            int cardId = player.deck[index];
            player.deck.RemoveAt(index);
            player.hand.Add(cardId);
        }
    }
    public bool CanPlayCards(PlayerState player, List<int> selectedIds)
    {
        int totalCost = 0;
        foreach (int id in selectedIds)
        {
            var card = CardDatabase.Instance.GetCard(id);
            if (card == null) return false;
            totalCost += card.cost;
        }
        return totalCost <= State.currentTurn;
    }
    public void PlayerEndedTurn(int playerIndex, List<int> selectedCardIds)
    {
        if (!_turnRunning || State == null) return;

        PlayerState ps = (playerIndex == 0) ? State.player0 : State.player1;

        if (!CanPlayCards(ps, selectedCardIds))
        {
            Debug.LogWarning($"Player {playerIndex} tried to play over cost.");
            return;
        }
        foreach (int id in selectedCardIds)
        {
            if (ps.hand.Contains(id))
            {
                ps.hand.Remove(id);
                ps.cardsInPlay.Add(id);
            }
        }
        if (playerIndex == 0)
            p0SelectedThisTurn = new List<int>(selectedCardIds);
        else
            p1SelectedThisTurn = new List<int>(selectedCardIds);
        _playersEndedTurn++;
        if (_playersEndedTurn >= 2)
        {
            _turnRunning = false;
            HandleBothPlayersEndedTurn();
        }
    }
    private void HandleBothPlayersEndedTurn()
    {
        AbilityResolver.ResolveTurn(State, p0SelectedThisTurn, p1SelectedThisTurn);
        BroadcastFullState();
        State.currentTurn++;
        StartTurn();
    }
    private void EndGame()
    {
        int winner = -1;
        if (State.player0.score > State.player1.score) winner = 0;
        else if (State.player1.score > State.player0.score) winner = 1;
        var msg = new GameEndMessage
        {
            action = "gameEnd",
            winnerIndex = winner,
            p0Score = State.player0.score,
            p1Score = State.player1.score
        };

        CardGameNetworkManager.Instance.BroadcastGameEnd(msg);
    }
    private void BroadcastFullState()
    {
        var msg = new StateMessage
        {
            action = "state",
            turn = State.currentTurn,
            timeRemaining = _turnTimer,
            cost = State.currentTurn,
            p0Hand = State.player0.hand.ToArray(),
            p1Hand = State.player1.hand.ToArray(),
            p0Board = State.player0.cardsInPlay.ToArray(),
            p1Board = State.player1.cardsInPlay.ToArray(),
            p0Score = State.player0.score,
            p1Score = State.player1.score
        };
        CardGameNetworkManager.Instance.BroadcastState(msg);
    }
}
