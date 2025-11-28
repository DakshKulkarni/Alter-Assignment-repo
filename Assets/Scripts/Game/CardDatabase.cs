using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AbilityData
{
    public string type;
    public int value;
}
[Serializable]
public class CardDefinition
{
    public int id;
    public string name;
    public int cost;
    public int power;
    public AbilityData ability;
}
[Serializable]
public class CardDatabaseData
{
    public List<CardDefinition> cards;
}
public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }
    private Dictionary<int, CardDefinition> _cardsById = new Dictionary<int, CardDefinition>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCards();
    }
    private void LoadCards()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("Data/cards");
        if (jsonAsset == null)
        {
            Debug.LogError("cards.json not found in Resources/Data/");
            return;
        }
        CardDatabaseData db = JsonUtility.FromJson<CardDatabaseData>(jsonAsset.text);
        foreach (var card in db.cards)
        {
            _cardsById[card.id] = card;
        }//loading cards after getting the values from cards.json
        Debug.Log($"Loaded {_cardsById.Count} card definitions.");
    }
    public CardDefinition GetCard(int id)
    {
        if (_cardsById.TryGetValue(id, out var card))
            return card;
        Debug.LogError($"Card id {id} not found.");
        return null;
    }
    public List<CardDefinition> GetAllCards()
    {
        return new List<CardDefinition>(_cardsById.Values);
    }
}
