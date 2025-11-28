using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CardView : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI costText;
    public Button button;
    private int _cardId;
    private System.Action<int> _onClicked;
    public void Init(CardDefinition def, int cardId, System.Action<int> onClicked)
    {//initialising card details 
        _cardId = cardId;
        _onClicked = onClicked;
        if (nameText != null)
            nameText.text = def.name;
        if (powerText != null)
            powerText.text = $"Power: {def.power}";
        if (costText != null)
            costText.text = $"Cost: {def.cost}";
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _onClicked?.Invoke(_cardId);
            });
        }
    }
}
