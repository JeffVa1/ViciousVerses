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
        phraseList = ParsePhrase(phraseText);
    }

    private List<string> ParsePhrase(string phrase)
    {
        List<string> phrases = new List<string>(phrase.Split(' '));

        for (int i = 0; i < phrases.Count; i++)
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

    public string GetDisplayText(List<Card> selectedCards)
    {
        List<string> displayList = new List<string>(phraseList); // Create a copy of the phrase list
        int selectedIndex = 0;

        for (int i = 0; i < displayList.Count; i++)
        {
            if (displayList[i] == "" && selectedIndex < selectedCards.Count)
            {
                // Replace blank with the text of the selected card
                displayList[i] = selectedCards[selectedIndex].GetText();
                selectedIndex++;
            }
            else if (displayList[i] == "")
            {
                // Keep remaining blanks as underscores
                displayList[i] = "_____";
            }
        }

        return string.Join(" ", displayList);
    }




}
