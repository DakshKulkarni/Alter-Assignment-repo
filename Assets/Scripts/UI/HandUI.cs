using System.Collections.Generic;
using UnityEngine;
public class HandUI : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform handContainer;
    public CardView cardPrefab;
    [Header("Layout")]
    public float cardWidth = 100f;
    public float spacing = 20f;
    private readonly List<int> _currentHandIds = new List<int>();
    private readonly HashSet<int> _selectedIds = new HashSet<int>();
    public void SetHand(List<int> cardIds)
    {
        Debug.Log($"[HandUI] SetHand called, count = {cardIds.Count}");
        // clear old cards visuals
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }
        _currentHandIds.Clear();
        _selectedIds.Clear();

        if (cardIds == null || cardIds.Count == 0)
            return;
        _currentHandIds.AddRange(cardIds);
        for (int i = 0; i < cardIds.Count; i++)
        {
            int id = cardIds[i];
            var def = CardDatabase.Instance.GetCard(id);
            if (def == null)
            {
                Debug.LogWarning($"[HandUI] No CardDefinition for id={id}");
                continue;
            }
            CardView view = Instantiate(cardPrefab, handContainer);
            var rt = view.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.localScale = Vector3.one;
            float x = i * (cardWidth + spacing);
            rt.anchoredPosition = new Vector2(x, 0f);
            view.Init(def, id, OnCardClicked);

        }
    }
    private void OnCardClicked(int cardId)
    {
        if (_selectedIds.Contains(cardId))
            _selectedIds.Remove(cardId);
        else
            _selectedIds.Add(cardId);
        Debug.Log($"[HandUI] Clicked card {cardId}. Selected count = {_selectedIds.Count}");
    }
    public List<int> GetSelectedCards()
    {
        return new List<int>(_selectedIds);
    }
}
