using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhraseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text phraseText;    // Text to display the phrase
    [SerializeField] private Button actionButton;    // Button to trigger actions (e.g., add/remove phrase)

    private JournalPhrase phrase; // Reference to the journal phrase this UI represents
    private System.Action<JournalPhrase> onPhraseAction; // Callback for the phrase action (e.g., Add to Journal)

    private bool isPurchased = false;

    // Setup the phrase UI with data
    public void Setup(JournalPhrase phraseData, System.Action<JournalPhrase> phraseAction)
    {
        phrase = phraseData;
        onPhraseAction = phraseAction;

        // Update the UI elements
        phraseText.text = phrase.GetPhraseText();

        // Attach the action to the button
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onPhraseAction?.Invoke(phrase));

        // Update the opacity based on whether it's purchased
        SetPhraseOpacity(isPurchased ? 0.5f : 1f);
    }

    public JournalPhrase GetPhrase()
    {
        return phrase;
    }

    public bool IsPurchased()
    {
        return isPurchased;
    }

    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        SetPhraseOpacity(isPurchased ? 0.5f : 1f); // Update opacity based on purchase status
    }

    public void SetPhraseOpacity(float opacity)
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
