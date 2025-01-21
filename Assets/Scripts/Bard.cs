using UnityEngine;
using static Dictionary;
using static DeckObj;

public class Bard
{
    private DeckObj deck;
    private Dictionary dict;
    private int ego;

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

    



}
