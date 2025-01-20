using UnityEngine;
using static Dictionary;
using static Card;
using static DeckObj;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Dictionary testDict = new Dictionary();
        Card c1 = new Card("cTest1", 1, 10);
        c1.LogCard(true);
        Card c2 = new Card("bTest2", 2, 20);
        Card c3 =  new Card("aTest3", 3, 30);
        Card c4 = new Card("aTest4", 4, 40);
        Card c5 = new Card("aTest5", 5, 50);
        testDict.AddCard(c1);
        testDict.AddCard(c2);
        testDict.AddCard(c3);

        testDict.LogCards(true);
        testDict.Alphabetize();
        testDict.LogCards(true);

        List<Card> newDeck1 = new List<Card> { c1, c2 };
        List<Card> newDeck2 = new List<Card> { c1, c2, c3, c4, c5};
        DeckObj deckObj1 = new DeckObj(newDeck1);
        DeckObj deckObj2 = new DeckObj();
        Debug.Log("constructor 1 test - library1 size: " + deckObj1.GetLibraryCount());
        deckObj2.SetLibrary(newDeck2);
        Debug.Log("constructor 2 test - library2 size: " + deckObj2.GetLibraryCount());

        Debug.Log("Add to library test - library1 size: " + deckObj1.GetLibraryCount());
        deckObj1.AddCardToLibrary(c3);
        Debug.Log("Add to library test - library1 size: " + deckObj1.GetLibraryCount());

        Debug.Log("discard test post call - library1 size: " + deckObj1.GetLibraryCount());
        deckObj1.RemoveCardFromLibrary(c3);
        Debug.Log("discard test post call - library1 size: " + deckObj1.GetLibraryCount());

        Debug.Log("draw player hand pre - library2 size: " + deckObj2.GetLibraryCount());
        Debug.Log("draw player hand pre - playerHand size: " + deckObj2.GetHandCount());
        Debug.Log("draw player hand pre - graveyard size: " + deckObj2.GetGraveyardCount());

        deckObj2.DrawMaxPlayerHandFromLibrary(newDeck1);

        Debug.Log("draw player hand post - library2 size: " + deckObj2.GetLibraryCount());
        Debug.Log("draw player hand post- playerHand size: " + deckObj2.GetHandCount());
        Debug.Log("draw player hand post - graveyard size: " + deckObj2.GetGraveyardCount());
    }

    void Update()
    {
        
    }
}
