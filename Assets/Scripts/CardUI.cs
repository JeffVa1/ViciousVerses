using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cardText;   // Text to display the card's text
    [SerializeField] private TMP_Text posText;    // Text to display the card's part of speech
    [SerializeField] private Button actionButton; // Button to trigger actions (e.g., add/remove card)

    private Card card; // Reference to the card this UI represents
    private System.Action<Card> onCardAction; // Callback for the card action (e.g., Add to Deck)

    // Setup the card UI with data
    public void Setup(Card cardData, System.Action<Card> cardAction)
    {
        card = cardData;
        onCardAction = cardAction;

        // Update the UI elements
        cardText.text = card.GetText();
        posText.text = card.GetPartOfSpeech();

        // Attach the action to the button
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onCardAction?.Invoke(card));
    }
}
