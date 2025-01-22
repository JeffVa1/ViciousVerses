using UnityEngine;

public class Card
{
    private string Text = "";
    private int pointMultiplier = 1;
    private int pointAddition = 0;
    private string partOfSpeech = "";
    private int egoDmg = 0;
    private int audienceValue = 0;

    public Card(string text, int multiplier, int addition, string pos, int e, int audience)
    {
        Text = text;
        pointMultiplier = multiplier;
        pointAddition = addition;
        partOfSpeech = pos;
        egoDmg = e;
        audienceValue = audience;
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
            Debug.Log("CARD");
            Debug.Log("Text: " + Text);
            Debug.Log("Mult: " + pointMultiplier);
            Debug.Log("Add : " + pointAddition);
            Debug.Log("");
        } else {
            Debug.Log(Text);
        }
    }
}
