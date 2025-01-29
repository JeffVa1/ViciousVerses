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

    public Card(string text, int multiplier, int addition, string pos, int e, int audience, bool inslt, List<string> category)
    {
        Text = text;
        pointMultiplier = multiplier;
        pointAddition = addition;
        partOfSpeech = pos;
        egoDmg = e;
        audienceValue = audience;
        insult = inslt;
        categories = category;
    }

    public bool IsInsult()
    {
        return insult;
    }

    public void SetInsult(bool i)
    {
        insult = i;
    }

    public List<string> GetCategories()
    {
        return categories;
    }

    public void SetCategories(List<string> c)
    {
        categories = c;
    }

    public string GetText()
    {
        return Text;
    }

    public int GetMultiplier()
    {
        return pointMultiplier;
    }

    public int GetAddition()
    {
        return pointAddition;
    }

    public string GetPartOfSpeech()
    {
        return partOfSpeech;
    }

    public int GetEgoDamage()
    {
        return egoDmg;
    }

    public int GetAudienceValue()
    {
        return audienceValue;
    }

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
            Debug.Log("");
        }
        else
        {
            Debug.Log($"Card Text: {Text}");
        }
    }
}
