using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JournalPhrase
{
    private string phraseText;
    private List<string> phraseList;
    private int numBlanks;

    public JournalPhrase(string phrase, int b)
    {
        phraseText = phrase;
        numBlanks = b;
        phraseList = ParsePhrase();
    }

    private List<string> ParsePhrase(string phrase)
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

    public void LogPhrase()
    {
        Debug.Log(GetPhraseText());
        Debug.Log("Num words: " + phraseList.Count);
        Debug.Log("Num blank: " + numBlanks);
    }

    

}
