using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer LocalPlayer;

    [SyncVar] public string playerId;
    [SyncVar] public int playerIndex;
    private void Start()
    {
        if (isLocalPlayer)
            LocalPlayer = this;
    }
    [ClientRpc]
    public void RpcReceiveJsonMessage(string json)
    {
        HandleJsonFromServer(json);
    }
    private void HandleJsonFromServer(string json)
    {
        Debug.Log("[NetworkPlayer] JSON from server: " + json);
        var wrapper = JsonUtility.FromJson<GenericMessageWrapper>(json);
        if (wrapper == null || string.IsNullOrEmpty(wrapper.action))
            return;
        switch (wrapper.action)
        {
            case "gameStart":
                HandleGameStart(JsonUtility.FromJson<GameStartMessage>(json));
                break;
            case "state":
                HandleState(JsonUtility.FromJson<StateMessage>(json));
                break;
            case "gameEnd":
                HandleGameEnd(JsonUtility.FromJson<GameEndMessage>(json));
                break;
        }
    }
    private void HandleGameStart(GameStartMessage msg)
    {
        Debug.Log($"[NetworkPlayer] GameStart: totalTurns={msg.totalTurns}");
    }
    private void HandleState(StateMessage msg)
    {
        // timer / turn / cost
        GameEvents.OnTurnStart?.Invoke(msg.turn, msg.timeRemaining, msg.cost);
        // scores
        GameEvents.OnScoreUpdated?.Invoke(msg.p0Score, msg.p1Score);
        // hand update
        if (playerIndex == 0)
            GameEvents.OnHandChanged?.Invoke(0, new List<int>(msg.p0Hand));
        else
            GameEvents.OnHandChanged?.Invoke(1, new List<int>(msg.p1Hand));
        GameEvents.OnBoardChanged?.Invoke(
            0,
            new List<int>(msg.p0Board)
        );
        GameEvents.OnBoardChanged?.Invoke(
            1,
            new List<int>(msg.p1Board)
        );
    }
    private void HandleGameEnd(GameEndMessage msg)
    {
        Debug.Log("[NetworkPlayer] GAME END! Winner: " + msg.winnerIndex);
        GameEvents.OnGameEnd?.Invoke(msg.winnerIndex);
    }
    public void LocalRequestEndTurn(List<int> selected)
    {
        if (!isLocalPlayer) return;
        var msg = new EndTurnMessage
        {
            action = "endTurn",
            playerId = playerId,
            cardIds = selected.ToArray()
        };
        string json = JsonUtility.ToJson(msg);
        CmdSendJsonToServer(json);
    }
    [Command]
    private void CmdSendJsonToServer(string json)
    {
        CardGameNetworkManager.Instance.HandleHostSideMessage(this, json);
    }
}
