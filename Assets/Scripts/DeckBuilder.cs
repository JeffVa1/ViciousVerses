using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static Dictionary;
using static DeckObj;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private Transform dictionaryContainer; // Parent object for dictionary cards
    [SerializeField] private Transform deckContainer;       // Parent object for deck cards
    [SerializeField] private GameObject cardPrefab;         // Prefab for displaying cards
    [SerializeField] private int maxDeckSize = 24;          // Maximum number of cards in the deck

    [SerializeField] private TextMeshProUGUI pageNumberText;
    private int currentPage = 1;
    private int cardsPerPage = 21;
    private int maxPages = 1;

    private Dictionary<Card, float> cardOpacityStates = new Dictionary<Card, float>();

    private Bard playerBard;
    private Dictionary dictionary;
    private DeckObj deck;


    private void Start()
    {
        playerBard = GameManager.Instance?.PlayerBard;
        if (playerBard == null)
        {
            Debug.LogError("PlayerBard is null! Make sure GameManager and PlayerBard are initialized.");
            return;
        }

        dictionary = playerBard.GetDictionary();
        if (dictionary == null)
        {
            Debug.LogError("Dictionary is null! Make sure the Bard has a dictionary assigned.");
            return;
        }

        deck = playerBard.GetDeck();
        if (deck == null)
        {
            Debug.LogError("Deck is null! Make sure the Bard has a deck assigned.");
            return;
        }

        StartCoroutine(InitializeUI());
    }

    private System.Collections.IEnumerator InitializeUI()
    {
        yield return null; // Wait for one frame
        CalculateMaxPages();
        PopulateDictionaryUI();
        PopulateDeckUI();
        UpdatePageNumberText();
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

            // Apply stored opacity state (if it exists)
            if (cardOpacityStates.ContainsKey(allCards[i]))
            {
                float opacity = cardOpacityStates[allCards[i]];
                cardUI.SetCardOpacity(opacity);
            }
        }
    }


    private void UpdatePageNumberText()
    {
        pageNumberText.text = $"{currentPage} / {maxPages}";
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
            Debug.Log("This card is already in the deck!");
            return;
        }
        if (deck.GetLibrary().Count >= 30)
        {
            Debug.Log(deck.GetLibrary().Count);
            Debug.Log("Deck is full!");
            return;
        }

        deck.AddCardToLibrary(card);
        RefreshDeckUI();
        ChangeCardOpacity(card, 0.5f);
    }

    private void ChangeCardOpacity(Card card, float opacity)
    {
        // Iterate through all cards in the dictionary UI and change the opacity
        foreach (Transform child in dictionaryContainer)
        {
            CardUI cardUI = child.GetComponent<CardUI>();
            if (cardUI != null && cardUI.GetCard() == card)
            {
                // Change the opacity and store the state
                cardUI.SetCardOpacity(opacity);
                // Save the opacity value for this card
                if (cardOpacityStates.ContainsKey(card))
                {
                    cardOpacityStates[card] = opacity;
                }
                else
                {
                    cardOpacityStates.Add(card, opacity);
                }
                break;
            }
        }
    }

    private void RemoveCardFromDeck(Card card)
    {
        deck.RemoveCardFromLibrary(card);
        ChangeCardOpacity(card, 1f);
        RefreshDeckUI();
    }

    private void RefreshDeckUI()
    {
        // Clear existing UI
        for (int i = deckContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(deckContainer.GetChild(i).gameObject);
        }

        PopulateDeckUI();
    }

    public void ConfirmDeck()
    {
        playerBard.SetDeck(deck);
        Debug.Log("Deck confirmed!");
        GameManager.Instance.ChangeState(GameManager.GameState.Battle);
    }
}
