using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Menu, Opening, DeckBuilder, Battle, Results, Shop, CutScene }
    public GameState CurrentState { get; private set; }

    public Bard PlayerBard { get; private set; }

    public LevelLoader levelLoader;
    public Bard OpponentBard1 { get; private set; }
    public Bard OpponentBard2 { get; private set; }
    public Bard OpponentBard3 { get; private set; }

    public Bard CurrentOpponent { get; private set; }

    public List<Card> shop_cards = new List<Card> { };
    public List<JournalPhrase> shop_phrases = new List<JournalPhrase> { };

    [SerializeField] private BattleManager battleManager;
    public int currentBattle = 1;

    public int round_number = 1;

    public GameState nextScene = GameState.Opening;

    public int prev_audience_score = 0;
    public int prev_gold_earned = 0;
    public bool WonLastMatch = false;

   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // This prevents destruction on scene changes
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event

        LoadPlayerData();
        LoadOpponentData();
        LoadShopData();

        // Initialize game state
        CurrentState = GameState.Menu;
    }

    private void LoadPlayerData()
    {
        List<Card> nouns = ParseCardsFromJson("defaultNouns.json", "noun");
        List<Card> verbs = ParseCardsFromJson("defaultVerbs.json", "verb");
        Dictionary playerDictionary = new Dictionary(new List<Card>(nouns).Concat(verbs).ToList());
        List<JournalPhrase> playerPhrases = ParsePhrasesFromJson("playerPhrases.json");
        PlayerBard = new Bard(playerDictionary, new Journal(playerPhrases));
        PlayerBard.SetRandomDeck();
    }

    private void LoadOpponentData()
    {
        List<Card> nouns = ParseCardsFromJson("opponentNouns.json", "noun");
        List<Card> verbs = ParseCardsFromJson("opponentVerbs.json", "verb");
        Dictionary opponentDictionary = new Dictionary(new List<Card>(nouns).Concat(verbs).ToList());
        List<JournalPhrase> opponentPhrases = ParsePhrasesFromJson("genericOpponentPhrases.json");
        OpponentBard1 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard2 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard3 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard1.SetRandomDeck();
        OpponentBard2.SetRandomDeck();
        OpponentBard3.SetRandomDeck();
    }

    private void LoadShopData()
    {
        shop_cards = new List<Card>(
            ParseCardsFromJson("shopNouns.json", "noun").Concat(ParseCardsFromJson("shopVerbs.json", "verb"))
        );
        shop_phrases = ParsePhrasesFromJson("shopPhrases.json");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene {scene.name} loaded.");

        if (scene.name == "Battle")
        {
            Debug.Log("Battle scene detected. Finding BattleManager...");
            battleManager = FindObjectOfType<BattleManager>();

            if (battleManager != null)
            {
                Debug.Log("****** Initializing battle! ******");
                battleManager.Initialize(PlayerBard, CurrentOpponent);
            }
            else
            {
                Debug.LogError("BattleManager not found in Battle scene!");
            }
        }
    }



    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Opening:
                LoadScene("OpeningScene");
                break;
            case GameState.DeckBuilder:
                LoadScene("DeckBuilder");
                break;
            case GameState.Battle:
                LoadScene("Battle");
                nextScene = GameState.Results;
                break;
            case GameState.Results:
                LoadScene("Results");
                if (WonLastMatch)
                {
                    IncrementCurrentBattle();
                    nextScene = GameState.Shop;
                }
                break;
            case GameState.Shop:
                LoadScene("Shop");
                nextScene = GameState.DeckBuilder;
                break;
            case GameState.CutScene:
                LoadScene("CutScene");
                break;
        }
    }

    private void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Starting async load for {sceneName}...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress}");
            yield return null;
        }

        Debug.Log($"Scene {sceneName} loaded successfully.");
    }


    public void IncrementCurrentBattle()
    {
        currentBattle += 1;
    }

    public void GoToResults()
    {
        ChangeState(GameState.Results);
    }

    public void GoToOpening() {
        ChangeState(GameState.Opening);
    }

    public void GoToNextScene(string current_scene)
    {
        if (current_scene == "Shop") {
            nextScene = GameState.DeckBuilder;
        }
        ChangeState(nextScene);
    }

    public void StartNextBattle()
    {
        if (currentBattle == 1)
        {
            InitializeBattle(PlayerBard, OpponentBard1);
        }
        else if (currentBattle == 2)
        {
            InitializeBattle(PlayerBard, OpponentBard2);
        }
        else if (currentBattle == 3)
        {
            InitializeBattle(PlayerBard, OpponentBard3);
        }
    }

    public void InitializeBattle(Bard player, Bard opponent)
    {
        PlayerBard = player;
        CurrentOpponent = opponent;
        ChangeState(GameState.Battle);
        //StartCoroutine(WaitForBattleSceneLoad());
    }

    private IEnumerator WaitForBattleSceneLoad()
    {
        yield return new WaitUntil(() => FindObjectOfType<BattleManager>() != null);
        battleManager = FindObjectOfType<BattleManager>();
        Debug.Log("******Initializing battle!******");
        battleManager.Initialize(PlayerBard, CurrentOpponent);
    }

    private List<JournalPhrase> ParsePhrasesFromJson(string filename)
    {
        List<JournalPhrase> journalPhrases = new List<JournalPhrase>();
        // string jsonFilePath = Path.Combine(Application.dataPath, "Data", filename);
        string jsonFilePath = "Assets/Data/" + filename;
        string json = File.ReadAllText(jsonFilePath);

        Dictionary<string, List<JournalPhraseData>> data = JsonConvert.DeserializeObject<Dictionary<string, List<JournalPhraseData>>>(json);

        foreach (var entry in data)
        {
            foreach (var phraseData in entry.Value)
            {
                // Pass the phrase, number of blanks, and blank_info JSON directly to the JournalPhrase constructor
                string blankInfoJson = JsonConvert.SerializeObject(phraseData.blank_info);
                JournalPhrase journalPhrase = new JournalPhrase(phraseData.phrase, phraseData.blanks, blankInfoJson);
                journalPhrases.Add(journalPhrase);
            }
        }

        return journalPhrases;
    }


    private List<Card> ParseCardsFromJson(string filename, string partOfSpeech)
    {
        string jsonFilePath = "Assets/Data/" + filename;
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
                    audience: cardData.audienceValue,
                    inslt: cardData.insult,
                    category: cardData.categories,
                    tenseDict: cardData.tenses
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

}

// Helper class for Card data deserialization
public class CardData
{
    public int ptValue { get; set; }
    public float ptMultiplier { get; set; }
    public int egoDmg { get; set; }
    public int audienceValue { get; set; }
    public bool insult { get; set; }
    public List<string> categories { get; set; }
    public Dictionary<string, string> tenses { get; set; }
}


// Helper class for Phrase data deserialization
[System.Serializable]
public class JournalPhraseData
{
    public string phrase;
    public int blanks;
    public List<BlankData> blank_info;
}

[System.Serializable]
public class BlankData
{
    public int blank_id;
    public BlankAttributes blank_attributes;
}

[System.Serializable]
public class BlankAttributes
{
    public string word;
    public string PreferredPOS;
    public string PreferredCAT;
    public bool Insult;
    public string Tense;
}