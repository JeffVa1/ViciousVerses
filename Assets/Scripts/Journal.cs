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

    public Journal()
    {
    
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

    public void ShuffleAvailable() {
		var count = availablePhrases.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i) {
			var r = Random.Range(i, count);
			var tmp = availablePhrases[i];
			availablePhrases[i] = availablePhrases[r];
			availablePhrases[r] = tmp;
		}
	}

    public void LogAllPhrases()
    {
        foreach (JournalPhrase p in availablePhrases)
        {
            LogPhrase(p);
        }
        foreach (JournalPhrase p in usedPhrases)
        {
            LogPhrase(p);
        }
        LogCurrentPhrase();
    }

    public void LogPhrase(JournalPhrase phrase)
    {
        Debug.Log(phrase.GetPhraseText());
        Debug.Log("Num words: " + phrase.GetPhraseList().Count);
        Debug.Log("Num blank: " + phrase.GetNumBlanks());
    }

    public void LogCurrentPhrase()
    {
        if (currentPhrase != null)
        {
            LogPhrase(currentPhrase);
        }
    }

    public void AddNewPhrase(JournalPhrase new_phrase)
    {
        availablePhrases.Add(new_phrase);
    }

    public bool SelectNewPhrase()
    {
        if (availablePhrases.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePhrases.Count);
            JournalPhrase random_phrase = availablePhrases[randomIndex];
            availablePhrases.RemoveAt(randomIndex);
            UpdateCurrentPhrase(random_phrase);
        }
        else
        {
            Debug.Log("ERROR - no more phrases available in journal");
            return false;
        }

        return true;
    }

    public void UpdateCurrentPhrase(JournalPhrase new_phrase)
    {
        if (currentPhrase == null)
        {
            currentPhrase = new_phrase;
        } 
        else 
        {
            usedPhrases.Add(currentPhrase);
            currentPhrase = new_phrase;
        }
    }

    public void ResetPhrases()
    {
        foreach (JournalPhrase p in usedPhrases)
        {
            availablePhrases.Add(p);
        }

        if (currentPhrase != null)
        {
            availablePhrases.Add(currentPhrase);
            currentPhrase = null;
        }
    }


}
