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
    [SerializeField] private TMP_Text bookCostText;       // Text to display the cost of the book
    [SerializeField] private Button tripleScrollButton;   // Button for purchasing a triple scroll
    [SerializeField] private TMP_Text tripleScrollCostText; // Text to display the cost of the triple scroll

    [Header("Item Costs")]
    [SerializeField] private int wordCardCost = 30;       // Cost of a word card
    [SerializeField] private int scrollCost = 50;         // Cost of a scroll
    [SerializeField] private int bookCost = 150;           // Cost of a book
    [SerializeField] private int tripleScrollCost = 150;   // Cost of a triple scroll

    private Bard bard; // Reference to the Bard class (which manages the player's cards and journal)
    private GameManager gameManager; // Reference to the GameManager (which stores available cards and journal phrases)

    [Header("Item Prefabs")]
    [SerializeField] private CardUI cardUIPrefab;   // Prefab for displaying card UI
    [SerializeField] private PhraseUI phraseUIPrefab; // Prefab for displaying phrase UI

    private bool bookPurchased = false; // Track if the book has been purchased
    private bool tripleScrollPurchased = false; // Track if the triple scroll has been purchased

    void Start()
    {
        bard = GameManager.Instance?.PlayerBard;
        gameManager = GameManager.Instance;

        // Update the UI initially
        UpdateCurrencyUI();

        // Set initial cost text for the book and triple scroll
        bookCostText.text = $"Cost: {bookCost}";
        tripleScrollCostText.text = $"Cost: {tripleScrollCost}";

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

    // Display available cards and phrases in the shop
    void DisplayAvailableItems()
    {
        // Display 3 available cards (if there are enough)
        List<Card> availableCards = gameManager.GetAllShopCards();
        ShuffleList(availableCards);
        for (int i = 0; i < 3; i++)
        {
            if (i < availableCards.Count)
            {
                if (i < cardContainer.childCount)
                {
                    CardUI cardUI = cardContainer.GetChild(i).GetComponent<CardUI>();
                    if (cardUI.GetCard() == availableCards[i] && cardUI.IsPurchased())
                    {
                        cardUI.SetCardOpacity(0.5f); // Reduce opacity if purchased
                    }
                }
                else
                {
                    Card card = availableCards[i];
                    CardUI cardUI = Instantiate(cardUIPrefab, cardContainer);
                    cardUI.Setup(card, PurchaseWordCard);

                    // Add cost text to the card UI
                    TMP_Text costText = cardUI.GetCostText();
                    if (costText != null)
                    {
                        costText.text = $"Cost: {wordCardCost}";
                    }
                }
            }
        }

        // Display 2 available phrases (if there are enough)
        List<JournalPhrase> availablePhrases = gameManager.GetAllShopPhrases();
        ShuffleList(availablePhrases);
        for (int i = 0; i < 2; i++)
        {
            if (i < availablePhrases.Count)
            {
                if (i < phraseContainer.childCount)
                {
                    PhraseUI phraseUI = phraseContainer.GetChild(i).GetComponent<PhraseUI>();
                    if (phraseUI.GetPhrase() == availablePhrases[i] && phraseUI.IsPurchased())
                    {
                        phraseUI.SetPhraseOpacity(0.5f); // Reduce opacity if purchased
                    }
                }
                else
                {
                    JournalPhrase phrase = availablePhrases[i];
                    PhraseUI phraseUI = Instantiate(phraseUIPrefab, phraseContainer);
                    phraseUI.Setup(phrase, PurchaseScroll);

                    // Add cost text to the phrase UI
                    TMP_Text costText = phraseUI.GetCostText();
                    if (costText != null)
                    {
                        costText.text = $"Cost: {scrollCost}";
                    }
                }
            }
        }

        // Update button opacities if items are purchased
        if (bookPurchased)
        {
            SetButtonOpacity(bookButton, 0.5f);
        }

        if (tripleScrollPurchased)
        {
            SetButtonOpacity(tripleScrollButton, 0.5f);
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

            // Mark the card as purchased and disable its button
            foreach (Transform child in cardContainer)
            {
                CardUI cardUI = child.GetComponent<CardUI>();
                if (cardUI.GetCard() == card)
                {
                    cardUI.SetPurchased(true); // Set the card as purchased
                    DisableButton(cardUI.GetButton()); // Disable the button and reduce opacity
                }
            }

            Debug.Log("Purchased a word card: " + card.GetText());
            UpdateCurrencyUI(); // Update the currency display
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

            // Mark the phrase as purchased and disable its button
            foreach (Transform child in phraseContainer)
            {
                PhraseUI phraseUI = child.GetComponent<PhraseUI>();
                if (phraseUI.GetPhrase() == phrase)
                {
                    phraseUI.SetPhraseOpacity(0.5f); // Reduce opacity of the purchased phrase
                    DisableButton(phraseUI.GetButton()); // Disable the button
                }
            }

            Debug.Log("Purchased a scroll: " + phrase.GetPhraseText());
            UpdateCurrencyUI(); // Update the currency display
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

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

            // Disable and dim the book button
            DisableButton(bookButton);
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

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

            // Disable and dim the triple scroll button
            DisableButton(tripleScrollButton);
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    // Helper function to disable a button and reduce its opacity
    void DisableButton(Button button)
    {
        button.interactable = false;

        // Set opacity to 50%
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 0.5f; // Set alpha to 50%
            buttonImage.color = color;
        }
    }


    // Helper to set button opacity
    void SetButtonOpacity(Button button, float opacity)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, opacity);
        button.colors = colors;
    }

    private void ShuffleList<T>(List<T> list)
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

    public void ExitShop()
    {
        GameManager.Instance.GoToNextScene("Shop");
    }
}
