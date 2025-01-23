using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static JournalPhrase;

public class Journal
{
    private List<JournalPhrase> availablePhrases;
    private List<JournalPhrase> usedPhrases;
    private JournalPhrase currentPhrase;


    public Journal(List<JournalPhrase> phrases)
    {
        availablePhrases = phrases;
        usedPhrases = new List<JournalPhrase> {};
    }

    public List<JournalPhrase> GetAvailablePhrases()
    {
        return availablePhrases;
    }

    public List<JournalPhrase> GetUsedPhrases()
    {
        return usedPhrases;
    }

    public JournalPhrase GetCurrentPhrase()
    {
        return currentPhrase;
    }

    public void LogCurrentPhrase()
    {
        Debug.Log(currentPhrase.GetPhraseText());
        Debug.Log("Num words: " + currentPhrase.GetPhraseList().Count);
        Debug.Log("Num blank: " + currentPhrase.GetNumBlanks());
    }


}
