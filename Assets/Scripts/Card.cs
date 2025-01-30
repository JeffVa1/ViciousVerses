using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card
{
    private string Text = "";
    private int pointMultiplier = 1;
    private int pointAddition = 0;
    private string partOfSpeech = "";
    private int egoDmg = 0;
    private int audienceValue = 0;
    private bool insult;
    private List<string> categories;
    private Dictionary<string, string> tenses;

    public Card(string text, int multiplier, int addition, string pos, int e, int audience, bool inslt, List<string> category, Dictionary<string, string> tenseDict)
    {
        Text = text;
        pointMultiplier = multiplier;
        pointAddition = addition;
        partOfSpeech = pos;
        egoDmg = e;
        audienceValue = audience;
        insult = inslt;
        categories = category ?? new List<string>();
        tenses = tenseDict ?? new Dictionary<string, string>();
    }

    public bool IsInsult() => insult;
    public void SetInsult(bool i) => insult = i;

    public List<string> GetCategories() => categories;
    public void SetCategories(List<string> c) => categories = c;

    public Dictionary<string, string> GetTenses() => tenses;
    public void SetTenses(Dictionary<string, string> t) => tenses = t;

    public string GetText() => Text;
    public int GetMultiplier() => pointMultiplier;
    public int GetAddition() => pointAddition;
    public string GetPartOfSpeech() => partOfSpeech;
    public int GetEgoDamage() => egoDmg;
    public int GetAudienceValue() => audienceValue;

    public void LogCard(bool full_info)
    {
        if (full_info)
        {
            Debug.Log("CARD INFO:");
            string format = "{0,-15}: {1}";
            Debug.Log(string.Format(format, "Text", Text));
            Debug.Log(string.Format(format, "Point Multiplier", pointMultiplier));
            Debug.Log(string.Format(format, "Point Addition", pointAddition));
            Debug.Log(string.Format(format, "Part of Speech", partOfSpeech));
            Debug.Log(string.Format(format, "Ego Damage", egoDmg));
            Debug.Log(string.Format(format, "Audience Value", audienceValue));
            Debug.Log(string.Format(format, "Insult", insult));
            Debug.Log(string.Format(format, "Categories", categories != null ? string.Join(", ", categories) : "None"));
            Debug.Log(string.Format(format, "Tenses", tenses != null ? string.Join(", ", tenses) : "None"));
            Debug.Log("");
        }
        else
        {
            Debug.Log($"Card Text: {Text}");
        }
    }
}
