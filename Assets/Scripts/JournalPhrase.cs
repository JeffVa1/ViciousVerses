using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine.Scripting; // Required for [Preserve]


public class JournalPhrase
{
    private string phraseText;
    private List<string> phraseList;
    private int numBlanks;
    private List<Blank> blanks_info;

    public JournalPhrase(string phrase, int blanks, string blankInfoJson)
    {
        phraseText = phrase;
        numBlanks = blanks;
        phraseList = ParsePhrase(phraseText);
        ParseBlanksInfo(blankInfoJson);
    }

    private List<string> ParsePhrase(string phrase)
    {
        // Your existing ParsePhrase implementation
        string pattern = @"BLANK(?=\W|$)|(?<=\W|^)BLANK|BLANK(-\w+)";
        List<string> phrases = new List<string>();

        int lastIndex = 0;

        foreach (Match match in Regex.Matches(phrase, pattern))
        {
            if (match.Index > lastIndex)
            {
                string textBeforeMatch = phrase.Substring(lastIndex, match.Index - lastIndex).Trim();
                if (!string.IsNullOrEmpty(textBeforeMatch))
                {
                    phrases.AddRange(textBeforeMatch.Split(' '));
                }
            }

            phrases.Add("");
            if (match.Groups[1].Success)
            {
                phrases.Add(match.Groups[1].Value);
            }

            lastIndex = match.Index + match.Length;
        }

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

    private void ParseBlanksInfo(string blankInfoJson)
    {
        blanks_info = JsonConvert.DeserializeObject<List<Blank>>(blankInfoJson);
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

    public List<Blank> GetBlanksInfo()
    {
        return blanks_info;
    }

    public void LogPhrase(bool fullInfo)
    {
        Debug.Log("PHRASE INFO:");

        // Log phrase text and number of blanks
        string format = "{0,-15}: {1}";
        Debug.Log(string.Format(format, "Phrase Text", phraseText));
        Debug.Log(string.Format(format, "Num Blanks", numBlanks));

        // Log phrase list
        Debug.Log(string.Format(format, "Phrase List", string.Join(" | ", phraseList)));

        if (fullInfo && blanks_info != null)
        {
            Debug.Log("BLANKS INFO:");
            foreach (var blank in blanks_info)
            {
                Debug.Log($"  Blank ID      : {blank.BlankID}");
                Debug.Log($"  Word          : {blank.Word}");
                Debug.Log($"  Preferred POS : {blank.PreferredPOS}");
                Debug.Log($"  Preferred CAT : {blank.PreferredCAT}");
                Debug.Log($"  Insult        : {blank.Insult}");
                Debug.Log($"  Tense         : {blank.Tense}");
                Debug.Log("");
            }
        }
    }


    public string GetDisplayText(List<Card> selectedCards)
    {
        List<string> displayList = new List<string>(phraseList); // Create a copy of the phrase list
        int selectedIndex = 0;

        for (int i = 0; i < displayList.Count; i++)
        {
            if (displayList[i] == "" && selectedIndex < selectedCards.Count)
            {
                Card selectedCard = selectedCards[selectedIndex];
                Blank correspondingBlank = blanks_info[selectedIndex];

                string finalWord = selectedCard.GetText();

                // Check if the blank has a tense requirement
                if (!string.IsNullOrEmpty(correspondingBlank.Tense))
                {
                    Dictionary<string, string> tenses = selectedCard.GetTenses();
                    if (tenses != null && tenses.ContainsKey(correspondingBlank.Tense))
                    {
                        finalWord = tenses[correspondingBlank.Tense];
                    }
                }

                displayList[i] = finalWord;
                selectedIndex++;
            }
            else if (displayList[i] == "")
            {
                displayList[i] = "_____";
            }
        }

        return string.Join(" ", displayList);
    }

}





[System.Serializable]
[Preserve] // Prevent IL2CPP stripping
[JsonObject(MemberSerialization.OptIn)] // Ensure correct JSON mapping
public class Blank
{
    [Preserve] [JsonProperty("blank_id")]
    public int BlankID { get; private set; }

    [Preserve] [JsonProperty("word")]
    public string Word { get; private set; }

    [Preserve] [JsonProperty("PreferredPOS")]
    public string PreferredPOS { get; private set; }

    [Preserve] [JsonProperty("PreferredCAT")]
    public string PreferredCAT { get; private set; }

    [Preserve] [JsonProperty("Insult")]
    public bool Insult { get; private set; }

    [Preserve] [JsonProperty("Tense")]
    public string Tense { get; private set; }

    [Preserve]
    public Blank()
    {
        BlankID = 0;
        Word = "";
        PreferredPOS = "";
        PreferredCAT = "";
        Insult = false;
        Tense = "";
    }

    [Preserve]
    [JsonConstructor]
    public Blank(int blankID, string word, string preferredPOS, string preferredCAT, bool insult, string tense)
    {
        BlankID = blankID;
        Word = word ?? "";
        PreferredPOS = preferredPOS ?? "";
        PreferredCAT = preferredCAT ?? "";
        Insult = insult;
        Tense = tense ?? "";
    }
}

