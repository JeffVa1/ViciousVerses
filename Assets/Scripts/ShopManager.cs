using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Shop UI Elements")]
    [SerializeField] private TMP_Text playerCurrencyText; // Display player currency
    [SerializeField] private Transform cardContainer;     // Container for card UI objects
    [SerializeField] private Transform phraseContainer;   // Container for phrase UI objects
    [SerializeField] private Button bookButton;           // Button for purchasing a book
    [SerializeField] private Button tripleScrollButton;   // Button for purchasing a triple scroll

    [Header("Item Costs")]
    [SerializeField] private int wordCardCost = 10;       // Cost of a word card
    [SerializeField] private int scrollCost = 20;          // Cost of a scroll
    [SerializeField] private int bookCost = 50;            // Cost of a book
    [SerializeField] private int tripleScrollCost = 60;    // Cost of a triple scroll

    private Bard bard; // Reference to the Bard class (which manages the player's cards and journal)
    private GameManager gameManager; // Reference to the GameManager (which stores available cards and journal phrases)

    [Header("Item Prefabs")]
    [SerializeField] private CardUI cardUIPrefab;   // Prefab for displaying card UI
    [SerializeField] private PhraseUI phraseUIPrefab; // Prefab for displaying phrase UI

    void Start()
    {
        bard = GameManager.Instance?.PlayerBard;
        gameManager = GameManager.Instance;

        // Update the UI initially
        UpdateCurrencyUI();

        // Button listeners
        bookButton.onClick.AddListener(PurchaseBook);
        tripleScrollButton.onClick.AddListener(PurchaseTripleScroll);

        // Display available cards and phrases
        DisplayAvailableItems();
    }

    // Updates the player's currency text
    void UpdateCurrencyUI()
    {
        playerCurrencyText.text = "Currency: " + bard.GetMoneyBalance();
    }

    void DisplayAvailableItems()
    {
        // Clear previous items
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in phraseContainer)
        {
            Destroy(child.gameObject);
        }

        // Get all available cards and phrases
        List<Card> availableCards = gameManager.GetAllShopCards();
        List<JournalPhrase> availablePhrases = gameManager.GetAllShopPhrases();

        // Randomly shuffle the available cards and phrases
        ShuffleList(availableCards);
        ShuffleList(availablePhrases);

        // Display 3 available cards (if there are enough)
        for (int i = 0; i < 3; i++)
        {
            if (i < availableCards.Count)
            {
                // Instantiate CardUI for each card
                Card card = availableCards[i];
                CardUI cardUI = Instantiate(cardUIPrefab, cardContainer);
                cardUI.Setup(card, PurchaseWordCard);
            }
        }

        // Display 2 available phrases (if there are enough)
        for (int i = 0; i < 2; i++)
        {
            if (i < availablePhrases.Count)
            {
                // Instantiate PhraseUI for each phrase
                JournalPhrase phrase = availablePhrases[i];
                PhraseUI phraseUI = Instantiate(phraseUIPrefab, phraseContainer);
                phraseUI.Setup(phrase, PurchaseScroll);
            }
        }
    }

    // Helper function to shuffle a list randomly
    void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Purchase a single word card and add it to the Bard's dictionary
    void PurchaseWordCard(Card card)
    {
        if (bard.GetMoneyBalance() >= wordCardCost)
        {
            bard.AddOrRemoveMoney(-wordCardCost);

            // Add the selected word card to the Bard's dictionary
            bard.AddCardToDict(card);
            gameManager.RemoveCardFromShop(card); // Remove the card from the shop's list

            Debug.Log("Purchased a word card: " + card.GetText());
            UpdateCurrencyUI(); // Update the currency display

            DisplayAvailableItems(); // Refresh the shop display
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    // Purchase a scroll and add a JournalPhrase to the Bard's journal
    void PurchaseScroll(JournalPhrase phrase)
    {
        if (bard.GetMoneyBalance() >= scrollCost)
        {
            bard.AddOrRemoveMoney(-scrollCost);

            // Add the selected phrase to the Bard's journal
            bard.AddPhraseToJournal(phrase);
            gameManager.RemovePhraseFromShop(phrase); // Remove the phrase from the shop's list

            Debug.Log("Purchased a scroll: " + phrase.GetPhraseText());
            UpdateCurrencyUI(); // Update the currency display

            DisplayAvailableItems(); // Refresh the shop display
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    // Purchase a book and add 5 random word cards to the Bard's dictionary
    void PurchaseBook()
    {
        if (bard.GetMoneyBalance() >= bookCost)
        {
            bard.AddOrRemoveMoney(-bookCost);

            // Add 5 random word cards from the available cards
            for (int i = 0; i < 5; i++)
            {
                Card newCard = gameManager.GetRandomShopCard();
                bard.AddCardToDict(newCard);
                gameManager.RemoveCardFromShop(newCard); // Remove the card from the shop's list
                Debug.Log("Purchased a word card: " + newCard.GetText());
            }

            UpdateCurrencyUI();
            DisplayAvailableItems(); // Refresh the shop display
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    // Purchase a triple scroll and add 3 random JournalPhrases to the Bard's journal
    void PurchaseTripleScroll()
    {
        if (bard.GetMoneyBalance() >= tripleScrollCost)
        {
            bard.AddOrRemoveMoney(-tripleScrollCost);

            // Add 3 random journal phrases from the available phrases
            for (int i = 0; i < 3; i++)
            {
                JournalPhrase newPhrase = gameManager.GetRandomShopJournalPhrase();
                bard.AddPhraseToJournal(newPhrase);
                gameManager.RemovePhraseFromShop(newPhrase); // Remove the phrase from the shop's list
                Debug.Log("Purchased a scroll: " + newPhrase.GetPhraseText());
            }

            UpdateCurrencyUI();
            DisplayAvailableItems(); // Refresh the shop display
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }
}
