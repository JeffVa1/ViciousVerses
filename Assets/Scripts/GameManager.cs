using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Intro, DeckBuilding, Battle, Results }
    public GameState CurrentState { get; private set; }
    
    public Bard PlayerBard { get; private set; }
    public Bard OpponentBard1 { get; private set; }
    public Bard OpponentBard2 { get; private set; }
    public Bard OpponentBard3 { get; private set; }

    public Bard CurrentOpponent { get; private set; }
    
    public int round_number = 1;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("LOADING NOUNS");
        string noun_filename = "defaultNouns.json";
        List<Card> nouns = ParseCardsFromJson(noun_filename, "noun");
        Debug.Log("LOADING VERBS");
        string verb_filename = "defaultVerbs.json";
        List<Card> verbs = ParseCardsFromJson(verb_filename, "verb");
        List<Card> playerCardList = new List<Card>();
        playerCardList.AddRange(nouns);
        playerCardList.AddRange(verbs);
        Dictionary player_dictionary = new Dictionary(playerCardList);
        player_dictionary.LogCards(true);
        //TODO - CREATE PLAYER DECK

        Debug.Log("LOADING PLAYER JOURNAL");
        string player_phrase_filename = "playerPhrases.json";
        List<JournalPhrase> player_phrases = ParsePhrasesFromJson(player_phrase_filename);
        Journal player_journal = new Journal(player_phrases);
        player_journal.LogAllPhrases();
        Debug.Log("SHUFFLING PHRASES");
        player_journal.ShuffleAvailable();
        player_journal.LogAllPhrases();
        //TODO - ASSIGN JOURNAL TO PLAYER BARD INSTANCE

        PlayerBard = new Bard(player_dictionary, player_journal);

        // Initialize game state
        CurrentState = GameState.Intro;
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Intro:
                LoadScene("Intro");
                break;
            case GameState.DeckBuilding:
                LoadScene("DeckBuilding");
                break;
            case GameState.Battle:
                LoadScene("Battle");
                break;
            case GameState.Results:
                LoadScene("Results");
                break;
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void InitializeBattle(Bard player, Bard opponent)
    {
        PlayerBard = player;
        CurrentOpponent = opponent;
        ChangeState(GameState.Battle);
    }


    private List<JournalPhrase> ParsePhrasesFromJson(string filename)
    {
        List<JournalPhrase> journalPhrases = new List<JournalPhrase>();
        string jsonFilePath = Path.Combine(Application.dataPath, "Data", filename);
        string json = File.ReadAllText(jsonFilePath);

        Dictionary<string, List<JournalPhraseData>> data = JsonConvert.DeserializeObject<Dictionary<string, List<JournalPhraseData>>>(json);

        foreach (var entry in data)
        {
            foreach (var phraseData in entry.Value)
            {
                JournalPhrase journalPhrase = new JournalPhrase(phraseData.phrase, phraseData.blanks);
                journalPhrases.Add(journalPhrase);
            }
        }


        return journalPhrases;
    }

    private List<Card> ParseCardsFromJson(string filename, string partOfSpeech)
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "Data", filename);
        string json = File.ReadAllText(jsonFilePath);
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
                    pos: partOfSpeech,
                    e: cardData.egoDmg,
                    audience: cardData.audienceValue
                );
                cards.Add(card);
            }
        }

        return cards;
    }

    void LogCardArray(List<Card> cards)
    {
        foreach (var card in cards)
        {
            card.LogCard(true);
        }
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

// Helper class for Card data deserialization
public class CardData
{
    public int ptValue { get; set; }
    public float ptMultiplier { get; set; }
    public int egoDmg { get; set; }
    public int audienceValue { get; set; }
}

// Helper class for Phrase data deserialization
[System.Serializable]
public class JournalPhraseData
{
    public string phrase;
    public int blanks;
}