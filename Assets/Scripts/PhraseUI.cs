using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhraseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text phraseText;    // Text to display the phrase
    [SerializeField] private Button actionButton;    // Button to trigger actions (e.g., add/remove phrase)

    private JournalPhrase phrase; // Reference to the journal phrase this UI represents
    private System.Action<JournalPhrase> onPhraseAction; // Callback for the phrase action (e.g., Add to Journal)

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
    }
}
