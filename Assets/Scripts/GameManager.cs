using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class GameManager : MonoBehaviour
{

    public int round_number = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Read card json and add starter cards to (Player)Bards dictionary.
        // Read journal json and add phrases to (Player)Bards journal.
        // Set (Player)Bards starter deck.

        // Read card json and add cards to 3 (Opponent)Bards decks.
        // Read journal json and add phrases to 3 (Opponent)Bards journals.
        

        // Display welcome message.
    }

    // Method to parse the JSON string into an array of Card objects
    private Card[] ParseCardsFromJson(string json)
    {
        var cardDict = JsonConvert.DeserializeObject<Dictionary<string, List<CardData>>>(json);
        List<Card> cards = new List<Card>();

        foreach (var entry in cardDict)
        {
            foreach (var cardData in entry.Value)
            {
                Card card = new Card(
                    text: entry.Key,
                    multiplier: (int)cardData.ptMultiplier,
                    addition: cardData.ptValue,
                    pos: "N/A", // Placeholder, adjust if necessary
                    e: cardData.egoDmg,
                    audience: cardData.audienceValue
                );
                cards.Add(card);
            }
        }

        return cards.ToArray();
    }

    void StartMatch() 
    {
        // Run code to start a match against an opponent.
    }

    void DisplayWelcomeModal()
    {
        // Displays the welcome message.
    }

    void DisplayShopModal()
    {
        // Displays the shop menu.
    }
    

    void DisplayGameOverModal()
    {
        // Displays game over message and gives player option to restart.
    }


}

// Helper class for deserialization
public class CardData
{
    public int ptValue { get; set; }
    public float ptMultiplier { get; set; }
    public int egoDmg { get; set; }
    public int audienceValue { get; set; }
}