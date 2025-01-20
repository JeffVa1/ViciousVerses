using UnityEngine;
using static Dictionary;
using static Card;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Dictionary testDict = new Dictionary();
        Card c1 = new Card("cTest1", 1, 10);
        c1.LogCard(true);
        Card c2 = new Card("bTest2", 2, 20);
        Card c3 =  new Card("aTest3", 3, 30);
        testDict.AddCard(c1);
        testDict.AddCard(c2);
        testDict.AddCard(c3);

        testDict.LogCards(true);
        testDict.Alphabetize();
        testDict.LogCards(true);
    }

    void Update()
    {
        
    }
}
