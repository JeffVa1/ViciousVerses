using System.Collections.Generic;
using UnityEngine;

public class DeckObj
{
    public List<Card> library = new List<Card>();
    public List<Card> playerHand = new List<Card>();
    public List<Card> graveyard = new List<Card>();

    private int maxCardsInHand = 10;
    private int maxDrawNewHand = 5;

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

    public void PlayCardToPhrase(Card card)
    {

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

}

