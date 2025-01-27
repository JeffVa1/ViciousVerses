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

    public List<Card> shop_cards = new List<Card> {};
    public List<JournalPhrase> shop_phrases = new List<JournalPhrase> {};
    
    public int round_number = 1;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // LOADING PLAYER DATA
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
        Debug.Log("LOADING PLAYER JOURNAL");
        string player_phrase_filename = "playerPhrases.json";
        List<JournalPhrase> player_phrases = ParsePhrasesFromJson(player_phrase_filename);
        Journal player_journal = new Journal(player_phrases);
        player_journal.LogAllPhrases();
        PlayerBard = new Bard(player_dictionary, player_journal);
        PlayerBard.SetRandomDeck();


        // LOADING ENEMY DATA
        Debug.Log("");
        Debug.Log("");
        Debug.Log("LOADING OPPONENT DATA");
        Debug.Log("LOADING NOUNS");
        noun_filename = "opponentNouns.json";
        nouns = ParseCardsFromJson(noun_filename, "noun");
        Debug.Log("LOADING VERBS");
        verb_filename = "opponentVerbs.json";
        verbs = ParseCardsFromJson(verb_filename, "verb");
        List<Card> opponentCardList = new List<Card>();
        opponentCardList.AddRange(nouns);
        opponentCardList.AddRange(verbs);
        Dictionary opponent_dictionary = new Dictionary(opponentCardList);
        opponent_dictionary.LogCards(true);
        Debug.Log("LOADING OPPONENT JOURNAL");
        string opponent_phrase_filename = "genericOpponentPhrases.json";
        List<JournalPhrase> opponent_phrases = ParsePhrasesFromJson(opponent_phrase_filename);
        Journal opponent_journal = new Journal(opponent_phrases);
        opponent_journal.LogAllPhrases();
        OpponentBard1 = new Bard(opponent_dictionary, opponent_journal);
        OpponentBard1.SetRandomDeck();

        // LOADING SHOP CARDS
        string shop_noun_filename = "shopNouns.json";
        string shop_verb_filename = "shopVerbs.json";
        List<Card> shop_nouns = ParseCardsFromJson(shop_noun_filename, "noun");
        List<Card> shop_verbs = ParseCardsFromJson(shop_verb_filename, "noun");
        List<Card> all_shop_cards = new List<Card>();
        all_shop_cards.AddRange(shop_nouns);
        all_shop_cards.AddRange(shop_verbs);
        shop_cards = all_shop_cards;

        // LOADING SHOP PHRASES
        string shop_phrase_filename = "shopPhrases.json";
        List<JournalPhrase> new_shop_phrases = ParsePhrasesFromJson(shop_phrase_filename);
        shop_phrases = new_shop_phrases;

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

    public List<Card> GetAllShopCards()
    {
        return shop_cards;
    }

    public List<JournalPhrase> GetAllShopPhrases()
    {
        return shop_phrases;
    }

    public Card GetRandomShopCard()
    {
        int randomIndex = Random.Range(0, shop_cards.Count);
        return shop_cards[randomIndex];
    }

    // Get a random journal phrase from the available phrases
    public JournalPhrase GetRandomShopJournalPhrase()
    {
        int randomIndex = Random.Range(0, shop_phrases.Count);
        return shop_phrases[randomIndex];
    }

    public void RemoveCardFromShop(Card c)
    {
        shop_cards.Remove(c);
    }

    public void RemovePhraseFromShop(JournalPhrase j)
    {
        shop_phrases.Remove(j);
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