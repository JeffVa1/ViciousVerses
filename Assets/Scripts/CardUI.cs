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

    private bool isPurchased = false;

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

        // Update the opacity based on whether it's purchased
        SetCardOpacity(isPurchased ? 0.5f : 1f);
    }

    public Card GetCard()
    {
        return card;
    }

    public bool IsPurchased()
    {
        return isPurchased;
    }

    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        SetCardOpacity(isPurchased ? 0.5f : 1f); // Update opacity based on purchase status
    }

    public void SetCardOpacity(float opacity)
    {
        // Ensure the opacity value is clamped between 0 (fully transparent) and 1 (fully opaque)
        opacity = Mathf.Clamp(opacity, 0f, 1f);

        // Get the button's Image component
        Image buttonImage = actionButton.GetComponent<Image>();

        // Modify the alpha value of the button's Image color
        Color buttonColor = buttonImage.color;
        buttonColor.a = opacity;  // Set the alpha to the desired opacity
        buttonImage.color = buttonColor;
    }

    public Button GetButton()
    {
        return actionButton;
    }
}
