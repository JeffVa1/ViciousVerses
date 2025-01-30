using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static Dictionary;
using static DeckObj;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private Transform dictionaryContainer;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private Transform journalContainer;
    [SerializeField] private ScrollRect JournalScrollView;
    [SerializeField] private ScrollRect DeckScrollView;
    [SerializeField] private Button togglePhraseDisplayButton;
    [SerializeField] private TextMeshProUGUI togglePhraseDisplayButtonText;
    [SerializeField] private GameObject journalPhrasePrefab;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int maxDeckSize = 30;
    [SerializeField] private int currentDeckCount = 0;

    [SerializeField] private TextMeshProUGUI pageNumberText;
    [SerializeField] private TextMeshProUGUI deckCounterText;
    private int currentPage = 1;
    private int cardsPerPage = 18;
    private int maxPages = 1;
    private bool isJournalViewActive = false;

    private Dictionary<Card, float> cardOpacityStates = new Dictionary<Card, float>();

    private Bard playerBard;
    private Dictionary dictionary;
    private DeckObj deck;


    private void Start()
    {
        playerBard = GameManager.Instance?.PlayerBard;
        if (playerBard == null)
        {
            return;
        }

        dictionary = playerBard.GetDictionary();
        if (dictionary == null)
        {
            return;
        }

        deck = playerBard.GetDeck();
        if (deck == null)
        {
            return;
        }

        InitializeCardOpacityStates();
        StartCoroutine(InitializeUI());
    }

    private void InitializeCardOpacityStates()
    {
        foreach (Card card in deck.GetLibrary())
        {
            if (!cardOpacityStates.ContainsKey(card))
            {
                cardOpacityStates[card] = 0.5f; // Set opacity to 0.5 for cards already in the deck
            }
        }
    }

    private System.Collections.IEnumerator InitializeUI()
    {
        yield return null;
        CalculateMaxPages();
        PopulateDictionaryUI();
        PopulateDeckUI();
        UpdatePageNumberText();
        UpdateDeckCounterText();
    }

    private void PopulateDictionaryUI()
    {
        // Clear the existing cards in the UI
        foreach (Transform child in dictionaryContainer)
        {
            Destroy(child.gameObject);
        }

        // Get cards for the current page
        List<Card> allCards = dictionary.GetAllCards();
        int startIndex = (currentPage - 1) * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, allCards.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, dictionaryContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(allCards[i], AddCardToDeck);
            // Apply stored opacity state (default to 1 if not present in the dictionary)
            float opacity = cardOpacityStates.ContainsKey(allCards[i]) ? cardOpacityStates[allCards[i]] : 1f;
            cardUI.SetCardOpacity(opacity);
        }
    }

    private int GetCurrentDeckCount()
    {
        return deck.GetLibrary().Count;
    }

    private void UpdatePageNumberText()
    {
        pageNumberText.text = $"{currentPage} / {maxPages}";
    }

    private void UpdateDeckCounterText()
    {
        deckCounterText.text = $"{GetCurrentDeckCount()} / {maxDeckSize}";
    }

    private void CalculateMaxPages()
    {
        int totalCards = dictionary.GetAllCards().Count;
        maxPages = Mathf.CeilToInt((float)totalCards / cardsPerPage);
    }

    public void NextPage()
    {
        if (currentPage < maxPages)
        {
            currentPage++;
            PopulateDictionaryUI();
            UpdatePageNumberText();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            PopulateDictionaryUI();
            UpdatePageNumberText();
        }
    }

    private void PopulateDeckUI()
    {
        List<Card> library = deck.GetLibrary();
        foreach (Card card in library)
        {
            GameObject cardObj = Instantiate(cardPrefab, deckContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card, RemoveCardFromDeck);
        }
    }

    private void AddCardToDeck(Card card)
    {
        if (deck.GetLibrary().Contains(card))
        {
            return;
        }
        if (deck.GetLibrary().Count >= maxDeckSize)
        {
            return;
        }

        deck.AddCardToLibrary(card);
        currentDeckCount = GetCurrentDeckCount();
        RefreshDeckUI();
        UpdateDeckCounterText();
        ChangeCardOpacity(card, 0.5f);
    }

    private void ChangeCardOpacity(Card card, float opacity)
    {
        // Update the opacity state in cardOpacityStates
        if (cardOpacityStates.ContainsKey(card))
        {
            cardOpacityStates[card] = opacity;
        }
        else
        {
            cardOpacityStates.Add(card, opacity);
        }

        PopulateDictionaryUI();
    }

    private void RemoveCardFromDeck(Card card)
    {
        deck.RemoveCardFromLibrary(card);
        currentDeckCount = GetCurrentDeckCount();
        UpdateDeckCounterText();
        ChangeCardOpacity(card, 1f);
        RefreshDeckUI();
    }

    private void RefreshDeckUI()
    {
        for (int i = deckContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(deckContainer.GetChild(i).gameObject);
        }

        PopulateDeckUI();
    }

    public void TogglePhraseDisplay()
    {
        isJournalViewActive = !isJournalViewActive;

        DeckScrollView.gameObject.SetActive(!isJournalViewActive);
        JournalScrollView.gameObject.SetActive(isJournalViewActive);

        togglePhraseDisplayButtonText.text = isJournalViewActive ? "Deck" : "Journal";

        ToggleChildren(DeckScrollView.content, !isJournalViewActive);
        ToggleChildren(JournalScrollView.content, isJournalViewActive);

        if (isJournalViewActive)
        {
            PopulateJournalUI();
        }

        SetAllJournalPhrasesActive();
    }

    private void ToggleChildren(Transform parent, bool isActive)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    private void SetAllJournalPhrasesActive()
    {
        foreach (Transform child in journalContainer)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void PopulateJournalUI()
    {
        foreach (Transform child in journalContainer)
        {
            Destroy(child.gameObject);
        }

        List<JournalPhrase> journalPhrases = playerBard.GetJournal().GetAvailablePhrases();

        foreach (JournalPhrase phrase in journalPhrases)
        {
            GameObject phraseObj = Instantiate(journalPhrasePrefab, journalContainer);
            PhraseUI phraseUI = phraseObj.GetComponent<PhraseUI>();
            phraseUI.Setup(phrase, null); 
            phraseUI.GetButton().gameObject.SetActive(false);
        }
    }


    public void ConfirmDeck()
    {
        playerBard.SetDeck(deck);
        GameManager.Instance.ChangeState(GameManager.GameState.Battle);
    }
}
