using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Dictionary;
using static DeckObj;
using static Journal;

public class Bard
{
    private DeckObj deck;
    private Dictionary dict;
    private Journal journal;
    private int ego;
    private int money = 500;

    public Bard(Dictionary d, DeckObj new_deck, int e, Journal j)
    {
        dict = d;
        deck = new_deck;
        ego = e;
        journal = j;
    }

    public Bard(Dictionary d, DeckObj new_deck, Journal j)
    {
        dict = d;
        deck = new_deck;
        ego = 100;
        journal = j;
    }

    public Bard(Dictionary d, Journal j)
    {
        dict = d;
        deck = new DeckObj();
        ego = 100;
        journal = j;
    }

    public Bard(Dictionary d)
    {
        dict = d;
        deck = new DeckObj();
        ego = 100;
        journal = new Journal();
    }

    public void SetJournal(Journal j)
    {
        journal = j;
    }

    public Journal GetJournal()
    {
        return journal;
    }

    public void AddPhraseToJournal(JournalPhrase j)
    {
        journal.AddNewPhrase(j);
    }

    public Dictionary GetDictionary()
    {
        return dict;
    }

    public void SetDictionary(Dictionary d)
    {
        dict = d;
    }

    public void SetDeck(DeckObj d)
    {
        deck = d;
    }

    public void SetDeck(List<Card> d)
    {
        deck = new DeckObj(d);
    }

    public DeckObj GetDeck()
    {
        return deck;
    }

    public int GetEgo() 
    {
        return ego;
    } 

    public void SetEgo(int e) 
    {
        ego = e;
    }

    public void AddEgo(int delta) 
    {
        ego += delta;
    }

    public void AddCardToDict(Card c) 
    {
        dict.AddCard(c);
    }

    public int GetMoneyBalance()
    {
        return money;
    }

    public void AddOrRemoveMoney(int delta)
    {
        money += delta;
    }

    public void SetMoney(int m)
    {
        money = m;
    }

}
