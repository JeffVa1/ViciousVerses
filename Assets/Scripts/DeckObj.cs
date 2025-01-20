using System.Collections.Generic;
using UnityEngine;

public class DeckObj : MonoBehaviour
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

    // These are to modify the library/deck
    public void AddCardToLibrary(Card card)
    {
        library.Add(card);
    }

    public void RemoveCardFromLibrary(Card card)
    {
        library.Remove(card);
    }

    // These are to manage players hand in the game loop
    public void AddToPlayerHand(Card card)
    {
        playerHand.Add(card);

    }



    public void DiscardFromPlayerHand(Card card)
    {
        graveyard.Add(card);
        playerHand.Remove(card);
    }

    public void DrawPlayerHand()
    {
        for (int i = 0; i < playerHand.Count; i++)
        {
            DiscardFromPlayerHand(playerHand[i]);
        }
        for (int i = 0; i < maxDrawNewHand; i++)
        {
            AddToPlayerHand(library[0]);
            library.Remove(library[0]);
        }
    }

    public void GraveyardToLibrary()
    {
        for (int i = 0; i < graveyard.Count - 1; i++)
        {
            library.Add(graveyard[i]);
            graveyard.Remove(graveyard[i]);
        }
    }

}
