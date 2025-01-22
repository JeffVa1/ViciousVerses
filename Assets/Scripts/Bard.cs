using UnityEngine;
using static Dictionary;
using static DeckObj;

public class Bard
{
    private DeckObj deck;
    private Dictionary dict;
    private int ego;
    private int money = 0;

    public Bard(Dictionary d, DeckObj new_deck, int e)
    {
        dict = d;
        deck = new_deck;
        ego = e;
    }

    public Bard(Dictionary d, DeckObj new_deck)
    {
        dict = d;
        deck = new_deck;
        ego = 100;
    }

    public Bard(Dictionary d)
    {
        dict = d;
        deck = new DeckObj();
        ego = 100;
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
