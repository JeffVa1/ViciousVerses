using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Card;

public class Dictionary : MonoBehaviour
{
    private List<Card> Cards;
    private CardCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Dictionary(List<Card> c)
    {
        Cards = c;
        CardCount = Cards.Length;
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
        CardCount += 1;
    }

    public void Alphabetize()
    {
        Cards = Cards.OrderBy(card => card.GetText()).ToList();
    }

    public void LogCards(bool full_info)
    {
        foreach (Card c in Cards) 
        {
            if (full_info)
            {
                c.LogCard();
            }
            else
            {
                Debug.Log(c.GetText());
            }
            
        }
    }

    public int GetCardCount()
    {
        return CardCount;
    }

    public List<Card> GetAllCards()
    {
        return Cards;
    }
}
