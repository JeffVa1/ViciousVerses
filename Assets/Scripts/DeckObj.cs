using System.Collections.Generic;
using UnityEngine;
using static Dictionary;

public class DeckObj
{
    public List<Card> library = new List<Card>();
    public List<Card> playerHand = new List<Card>();
    public List<Card> graveyard = new List<Card>();

    private int maxCardsInHand = 10;
    private int maxDrawNewHand = 5;
    private int maxCardsInDeck = 30;

    // Constructor
    public DeckObj(List<Card> cards)
    {
        library = cards;
    }

    public DeckObj()
    {

    }

    // Getters and Setters

    public List<Card> GetLibrary()
    {
        return library;
    }

    public void SetLibrary(List<Card> newDeck)
    {
        library = newDeck;
    }

    public int GetHandCount()
    {
        return playerHand.Count;
    }

    public int GetLibraryCount()
    {
        return library.Count;
    }

    public int GetGraveyardCount()
    {
        return graveyard.Count;
    }

    // Methods

    // Methods to modify the library/deck
    public void AddCardToLibrary(Card card)
    {
        library.Add(card);
    }

    public void RemoveCardFromLibrary(Card card)
    {
        library.Remove(card);
    }

    // Methods to add cards to the player hand
    public void AddOneCardToPlayerHand(Card card)
    {
        playerHand.Add(card);
    }

    public void DrawOneToHandFromLibrary(List<Card> library)
    {
        playerHand.Add(library[0]);
        library.Remove(library[0]);
    }

    public void DrawMaxPlayerHandFromLibrary(List<Card> library)
    {
        for (int i = 0; i < maxDrawNewHand; i++)
        {
            DrawOneToHandFromLibrary(library);
        }
    }

    // Methods to remove cards from the player hand

    public void RemoveOneCardFromPlayerHand(Card card)
    {
        playerHand.Remove(card);
    }

    public void DiscardOneFromHandToGraveyard(Card card)
    {
        graveyard.Add(card);
        playerHand.Remove(card);
    }

    public void DiscardAllFromPlayerHand()
    {
        for (int i = 0; i < playerHand.Count; i++)
        {
            DiscardOneFromHandToGraveyard(playerHand[i]);
        }
    }

    // Mehtod to return Graveyard to Library when Library is empty
    public void GraveyardToLibrary()
    {
        for (int i = 0; i < graveyard.Count - 1; i++)
        {
            library.Add(graveyard[i]);
            graveyard.Remove(graveyard[i]);
        }
    }

    public void DiscardHandExcept(List<Card> savedCards)
    {
        List<Card> cardsToDiscard = new List<Card>();

        // Identify cards to discard
        foreach (Card card in playerHand)
        {
            if (!savedCards.Contains(card))
            {
                cardsToDiscard.Add(card);
            }
        }

        // Move identified cards to the graveyard
        foreach (Card card in cardsToDiscard)
        {
            graveyard.Add(card);
            playerHand.Remove(card);
        }
    }

    public void DrawCards(int numCards)
    {
        for (int i = 0; i < numCards; i++)
        {
            if (playerHand.Count >= maxCardsInHand)
            {
                Debug.Log("Player hand is full.");
                break;
            }

            if (library.Count == 0)
            {
                if (graveyard.Count > 0)
                {
                    Debug.Log("Library is empty. Shuffling graveyard back into library.");
                    GraveyardToLibrary();
                    ShuffleDeck();
                }
                else
                {
                    Debug.Log("No more cards to draw.");
                    break;
                }
            }

            DrawOneToHandFromLibrary(library);
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < library.Count; i++)
        {
            int randomIndex = Random.Range(0, library.Count);
            Card temp = library[i];
            library[i] = library[randomIndex];
            library[randomIndex] = temp;
        }

        Debug.Log("Library shuffled.");
    }

    public void SetRandomLibrary(Dictionary d)
    {
        library.Clear(); // Ensure the library starts empty

        List<Card> allCards = d.GetAllCards(); // Assume Dictionary has a method to get all available cards
        List<Card> tempDeck = new List<Card>(allCards);

        if (tempDeck.Count < 30)
        {
            Debug.LogError("Not enough cards in the dictionary to fill the library.");
            return;
        }

        for (int i = 0; i < 30; i++)
        {
            int randomIndex = Random.Range(0, tempDeck.Count);
            Card selectedCard = tempDeck[randomIndex];
            library.Add(selectedCard);
            tempDeck.RemoveAt(randomIndex); // Remove to avoid duplicates
        }

        Debug.Log("Library set with 30 random cards.");
    }



}

