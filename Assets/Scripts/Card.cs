using UnityEngine;

public class Card : MonoBehaviour
{
    private string Text = "";
    private int pointMultiplier = 1;
    private int pointAddition = 0;

    public Card(string text, int multiplier, int addition)
    {
        Text = text;
        pointMultiplier = multiplier;
        pointAddition = addition;
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
