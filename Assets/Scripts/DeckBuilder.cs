using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static Dictionary;
using static DeckObj;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private Transform dictionaryContainer; // Parent object for dictionary cards
    [SerializeField] private Transform deckContainer;       // Parent object for deck cards
    [SerializeField] private GameObject cardPrefab;         // Prefab for displaying cards
    [SerializeField] private int maxDeckSize = 10;          // Maximum number of cards in the deck

    private Bard playerBard;
    private Dictionary dictionary;
    private DeckObj deck;

    private void Start()
    {
        playerBard = GameManager.Instance.PlayerBard;
        dictionary = playerBard.Dictionary;
        deck = playerBard.Deck;

        PopulateDictionaryUI();
        PopulateDeckUI();
    }

    private void PopulateDictionaryUI()
    {
        List<Card> allCards = dictionary.GetAllCards();
        foreach (Card card in allCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, dictionaryContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card, AddCardToDeck);
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
        if (deck.GetLibrary().Count >= maxDeckSize)
        {
            Debug.Log("Deck is full!");
            return;
        }

        deck.AddCardToLibrary(card);
        RefreshDeckUI();
    }

    private void RemoveCardFromDeck(Card card)
    {
        deck.RemoveCardFromLibrary(card);
        RefreshDeckUI();
    }

    private void RefreshDeckUI()
    {
        // Clear existing UI
        foreach (Transform child in deckContainer)
        {
            Destroy(child.gameObject);
        }

        PopulateDeckUI();
    }

    public void ConfirmDeck()
    {
        playerBard.Deck.SetLibrary(deck.GetLibrary());
        GameManager.Instance.ChangeState(GameManager.GameState.Battle);
    }
}
