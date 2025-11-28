using System.Collections.Generic;
using UnityEngine;
public class BoardUI : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform myBoardContainer;
    public RectTransform oppBoardContainer;
    public CardView cardPrefab;
    [Header("Layout")]
    public float cardWidth = 100f;
    public float spacing = 20f;
    private void OnEnable()
    {
        GameEvents.OnBoardChanged += HandleBoardChanged;
    }
    private void OnDisable()
    {
        GameEvents.OnBoardChanged -= HandleBoardChanged;
    }
    private void HandleBoardChanged(int playerIndex, List<int> boardIds)
    {
        if (NetworkPlayer.LocalPlayer == null) return;

        bool isMe = (playerIndex == NetworkPlayer.LocalPlayer.playerIndex);
        RectTransform container = isMe ? myBoardContainer : oppBoardContainer;
        if (container == null)
        {
            Debug.LogError("[BoardUI] Container is null for " + (isMe ? "me" : "opponent"));
            return;
        }
        // Clearing the previous old cards
        foreach (Transform child in container)
            Destroy(child.gameObject);

        if (boardIds == null || boardIds.Count == 0)
            return;

        // Spawning cards for the player in the centre
        for (int i = 0; i < boardIds.Count; i++)
        {
            int id = boardIds[i];
            var def = CardDatabase.Instance.GetCard(id);
            if (def == null)
            {
                Debug.LogWarning($"[BoardUI] No CardDefinition for id={id}");
                continue;
            }
            CardView view = Instantiate(cardPrefab, container);
            var rt = view.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;
            float x = (i - (boardIds.Count - 1) * 0.5f) * (cardWidth + spacing);
            rt.anchoredPosition = new Vector2(x, 0f);
            view.Init(def, id, null);
        }
    }
}
