using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


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
        // Use a regular expression to identify "BLANK" and preserve parts of contractions
        string pattern = @"BLANK(?=\W|$)|(?<=\W|^)BLANK|BLANK(-\w+)";
        List<string> phrases = new List<string>();

        int lastIndex = 0;

        foreach (Match match in Regex.Matches(phrase, pattern))
        {
            // Add the text before the match to the list
            if (match.Index > lastIndex)
            {
                string textBeforeMatch = phrase.Substring(lastIndex, match.Index - lastIndex).Trim();
                if (!string.IsNullOrEmpty(textBeforeMatch))
                {
                    phrases.AddRange(textBeforeMatch.Split(' '));
                }
            }

            // Add an empty string for "BLANK"
            phrases.Add("");

            // If there's a suffix like "-ish", add it to the list
            if (match.Groups[1].Success)
            {
                phrases.Add(match.Groups[1].Value);
            }

            lastIndex = match.Index + match.Length;
        }

        // Add any remaining text after the last match
        if (lastIndex < phrase.Length)
        {
            string remainingText = phrase.Substring(lastIndex).Trim();
            if (!string.IsNullOrEmpty(remainingText))
            {
                phrases.AddRange(remainingText.Split(' '));
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
        // Debug.Log(GetPhraseText());
        // Debug.Log("Num words: " + phraseList.Count);
        // Debug.Log("Num blank: " + numBlanks);
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
