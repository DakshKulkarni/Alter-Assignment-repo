using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class CardGameNetworkManager : NetworkManager
{
    public static CardGameNetworkManager Instance;
    private readonly List<NetworkPlayer> _players = new List<NetworkPlayer>();
    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        var player = conn.identity.GetComponent<NetworkPlayer>();
        _players.Add(player);
        player.playerIndex = _players.Count - 1;
        // gives a string id like P1,P2
        player.playerId = $"P{_players.Count}";
        if (_players.Count == 2)
        {
            StartMatch();
        }
    }
    public IReadOnlyList<NetworkPlayer> Players => _players;
    private void StartMatch()
    {
        var allCards = CardDatabase.Instance.GetAllCards();
        int deckSize = 12;

        // builds two random decks
        List<int> deck0 = BuildRandomDeck(deckSize, allCards);
        List<int> deck1 = BuildRandomDeck(deckSize, allCards);

        // if in case identical, reshuffle P1 a few times
        int safety = 0;
        while (AreDecksIdentical(deck0, deck1) && safety < 5)
        {
            deck1 = BuildRandomDeck(deckSize, allCards);
            safety++;
        }
        GameController.Instance.StartGame(deck0, deck1);
        var gs = new GameStartMessage
        {
            action = "gameStart",
            playerIds = new[]
            {
                _players[0].playerId,
                _players[1].playerId
            },
            totalTurns = GameState.MaxTurns
        };
        string json = JsonUtility.ToJson(gs);
        foreach (var p in _players)
        {
            if (p != null)
                p.RpcReceiveJsonMessage(json);
        }
    }
    private List<int> BuildRandomDeck(int deckSize, List<CardDefinition> allCards)
    {
        var deck = new List<int>();
        if (allCards == null || allCards.Count == 0)
            return deck;
        var pool = new List<CardDefinition>(allCards);
        for (int i = 0; i < pool.Count; i++)
        {
            int j = Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }
        for (int i = 0; i < deckSize && i < pool.Count; i++)
        {
            deck.Add(pool[i].id);
        }
        bool hasCheap = false;
        foreach (int id in deck)
        {
            var def = CardDatabase.Instance.GetCard(id);
            if (def != null && def.cost == 1)
            {
                hasCheap = true;
                break;
            }
        }
        if (!hasCheap)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                var def = pool[i];
                if (def.cost == 1)
                {
                    int replaceIndex = Random.Range(0, deck.Count);
                    deck[replaceIndex] = def.id;
                    break;
                }
            }
        }
        return deck;
    }
    private bool AreDecksIdentical(List<int> a, List<int> b)
    {
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }
    public void HandleHostSideMessage(NetworkPlayer sender, string json)
    {
        var wrapper = JsonUtility.FromJson<GenericMessageWrapper>(json);
        if (wrapper == null || string.IsNullOrEmpty(wrapper.action))
            return;
        if (wrapper.action == "endTurn")
        {
            var msg = JsonUtility.FromJson<EndTurnMessage>(json);
            int playerIndex = sender.playerIndex;
            GameController.Instance.PlayerEndedTurn(
                playerIndex,
                new List<int>(msg.cardIds)
            );
        }
    }
    public void BroadcastState(StateMessage msg)
    {
        string json = JsonUtility.ToJson(msg);
        foreach (var p in _players)
        {
            if (p != null)
                p.RpcReceiveJsonMessage(json);
        }
    }
    public void BroadcastGameEnd(GameEndMessage msg)
    {
        string json = JsonUtility.ToJson(msg);

        foreach (var p in _players)
        {
            if (p != null)
                p.RpcReceiveJsonMessage(json);
        }
    }
}
