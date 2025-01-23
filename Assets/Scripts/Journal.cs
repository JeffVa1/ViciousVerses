using UnityEngine;

public class Journal
{
    private string phraseText;
    private List[string] phraseList;
    private int numBlanks;

    public Journal(string phrase, int b)
    {
        phraseText = phrase;
        numBlanks = b;
        phraseList = ParsePhrase();
    }

    private ParsePhrase(string phrase)
    {
        List<string> phrases = new List<string>(phrase.Split(' '));

        for (int i = 0; i < phrases.Length; i++)
        {
            if (phrases[i] == "BLANK")
            {
                phrases[i] = "";
            }
        }

        return phrases;
    }

    public int GetNumBlanks() 
    {
        return numBlanks;
    }

    public string GetPhraseText() 
    {
        return phraseText.Replace("BLANK", "_____");
    }

    public List<string> GetPhraseList()
    {
        return phraseList;
    }


}
