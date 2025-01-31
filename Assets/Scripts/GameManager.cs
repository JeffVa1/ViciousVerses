using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Scripting;

using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Menu, Opening, DeckBuilder, Battle, Results, Shop, Scene_2, Scene_3}
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

    public GameState nextScene = GameState.Opening;

    public int prev_audience_score = 0;
    public int prev_gold_earned = 0;
    public bool WonLastMatch = false;



    private async void Awake()
    {
        AotHelper.EnsureType<JournalPhraseData>();
        AotHelper.EnsureType<BlankData>();
        AotHelper.EnsureType<BlankAttributes>();
        var tempPhrase = new JournalPhraseData();
        var tempBlank = new BlankData();
        var tempAttributes = new BlankAttributes();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // This prevents destruction on scene changes
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event

        await LoadPlayerData();
        await LoadOpponentData();
        await LoadShopData();

        // Initialize game state
        CurrentState = GameState.Menu;
    }

    private async Task LoadPlayerData()
    {
        List<Card> nouns = await ParseCardsFromJson("defaultNouns.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("defaultVerbs.json", "verb");
        Dictionary playerDictionary = new Dictionary(new List<Card>(nouns).Concat(verbs).ToList());
        List<JournalPhrase> playerPhrases = await ParsePhrasesFromJson("playerPhrases.json");
        PlayerBard = new Bard(playerDictionary, new Journal(playerPhrases));
        PlayerBard.SetRandomDeck();
    }

    private async Task LoadOpponentData()
    {
        List<Card> nouns = await ParseCardsFromJson("opponentNouns.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("opponentVerbs.json", "verb");
        Dictionary opponentDictionary = new Dictionary(new List<Card>(nouns).Concat(verbs).ToList());
        List<JournalPhrase> opponentPhrases = await ParsePhrasesFromJson("genericOpponentPhrases.json");
        OpponentBard1 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard2 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard3 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard1.SetRandomDeck();
        OpponentBard2.SetRandomDeck();
        OpponentBard3.SetRandomDeck();
    }

    private async Task LoadShopData()
    {
        shop_cards = new List<Card>();
        List<Card> nouns = await ParseCardsFromJson("shopVerbs.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("shopNouns.json", "verb");
        shop_cards = nouns.Concat(verbs).ToList();
        shop_phrases = await ParsePhrasesFromJson("shopPhrases.json");
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
                nextScene = GameState.Battle;
                break;
            case GameState.DeckBuilder:
                LoadScene("DeckBuilder");
                if (currentBattle == 2) {
                    nextScene = GameState.Scene_2;
                } else if (currentBattle == 3) {
                    nextScene = GameState.Scene_3;
                }
                break;
            case GameState.Battle:
                LoadScene("Battle");
                nextScene = GameState.Results;
                break;
            case GameState.Results:
                LoadScene("Results");
                PlayerBard.ResetDeck();
                OpponentBard1.ResetDeck();
                OpponentBard2.ResetDeck();
                OpponentBard3.ResetDeck();
                if (WonLastMatch)
                {
                    IncrementCurrentBattle();
                    nextScene = GameState.Shop;
                } else {
                    nextScene = GameState.Opening;
                    currentBattle = 1;
                }
                break;
            case GameState.Shop:
                LoadScene("Shop");
                nextScene = GameState.DeckBuilder;
                break;
            case GameState.Scene_2:
                LoadScene("Scene_2");
                break;
            case GameState.Scene_3:
                LoadScene("Scene_3");
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

    public void GoToOpening()
    {
        ChangeState(GameState.Opening);
    }

    public void GoToNextScene()
    {
 

        ChangeState(nextScene);
    }

    public void StartNextBattle()
    {
        Debug.Log("CURRENT BATTLE: " + currentBattle);
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

private async Task<List<JournalPhrase>> ParsePhrasesFromJson(string filename)
{
    var journalPhrases = new List<JournalPhrase>();
    var tcs = new TaskCompletionSource<List<JournalPhrase>>();

    JsonLoader.LoadJson(filename, (jsonData) =>
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            Debug.Log($"[WebGL Debug] Raw JSON from {filename}: {jsonData}"); // 🔥 Logs JSON

            try
            {
                // 🔹 Try deserializing JSON as a dictionary
                Dictionary<string, List<JournalPhraseData>> phraseDict = 
                    JsonConvert.DeserializeObject<Dictionary<string, List<JournalPhraseData>>>(jsonData);

                if (phraseDict != null)
                {
                    foreach (var entry in phraseDict)
                    {
                        foreach (var phraseData in entry.Value)
                        {
                            string blankInfoJson = JsonConvert.SerializeObject(phraseData.blank_info ?? new List<BlankData>());
                            JournalPhrase journalPhrase = new JournalPhrase(phraseData.phrase, phraseData.blanks, blankInfoJson);
                            journalPhrases.Add(journalPhrase);
                        }
                    }
                }

                Debug.Log($"[WebGL Debug] Successfully loaded {journalPhrases.Count} phrases from {filename}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WebGL ERROR] Failed to deserialize JSON from {filename}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"[WebGL ERROR] JSON file {filename} is empty or null.");
        }

        tcs.SetResult(journalPhrases);
    });

    return await tcs.Task;
}





private async Task<List<Card>> ParseCardsFromJson(string filename, string partOfSpeech)
{
    var cards = new List<Card>();
    var tcs = new TaskCompletionSource<List<Card>>();

    var settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.None,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

    JsonLoader.LoadJson(filename, (jsonData) =>
    {
        if (!string.IsNullOrEmpty(jsonData))
        {
            try
            {
                Debug.Log($"Attempting to deserialize JSON from {filename}");
                
                var cardDict = JsonConvert.DeserializeObject<Dictionary<string, List<CardData>>>(jsonData, settings);

                if (cardDict == null)
                {
                    Debug.LogError($"Deserialization returned NULL for {filename}");
                    return;
                }

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
                            inslt: cardData.insult ?? false,
                            category: cardData.categories ?? new List<string>(),
                            tenseDict: cardData.tenses ?? new Dictionary<string, string>()
                        );
                        cards.Add(card);
                    }
                }

                Debug.Log($"Successfully loaded {cards.Count} cards from {filename}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to deserialize JSON from {filename}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"JSON file {filename} is empty or null.");
        }

        tcs.SetResult(cards);
    });

    return await tcs.Task;
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

[Preserve] // Prevents IL2CPP stripping
public class CardData
{
    [Preserve] // Prevents stripping
    public CardData() {} // Parameterless constructor

    [Preserve] // Ensures constructor isn't removed
    [JsonConstructor]
    public CardData(
        int ptValue,
        float ptMultiplier,
        int egoDmg,
        int audienceValue,
        bool? insult,  // Nullable since not always present
        List<string> categories,  // Nullable since not always present
        Dictionary<string, string> tenses  // Nullable since not always present
    )
    {
        this.ptValue = ptValue;
        this.ptMultiplier = ptMultiplier;
        this.egoDmg = egoDmg;
        this.audienceValue = audienceValue;
        this.insult = insult ?? false;
        this.categories = categories ?? new List<string>();
        this.tenses = tenses ?? new Dictionary<string, string>();
    }

    [Preserve] [JsonProperty("ptValue")]
    public int ptValue { get; set; }

    [Preserve] [JsonProperty("ptMultiplier")]
    public float ptMultiplier { get; set; }

    [Preserve] [JsonProperty("egoDmg")]
    public int egoDmg { get; set; }

    [Preserve] [JsonProperty("audienceValue")]
    public int audienceValue { get; set; }

    [Preserve] [JsonProperty("insult", NullValueHandling = NullValueHandling.Ignore)]
    public bool? insult { get; set; }

    [Preserve] [JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> categories { get; set; }

    [Preserve] [JsonProperty("tenses", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string> tenses { get; set; }
}

[System.Serializable]
[Preserve] // Prevent IL2CPP stripping
public class JournalPhraseData
{
    [Preserve]

    public JournalPhraseData() 
    {
        this.phrase = "";
        this.blanks = 0;
        this.blank_info = new List<BlankData>(); // Prevent null references
    }

    [Preserve]
    [JsonConstructor]
    public JournalPhraseData(string phrase, int blanks, List<BlankData> blank_info)
    {
        this.phrase = phrase ?? ""; // Default to empty string
        this.blanks = blanks;
        this.blank_info = blank_info ?? new List<BlankData>(); // Ensure no null reference
    }

    [Preserve] [JsonProperty("phrase", NullValueHandling = NullValueHandling.Include)]
    public string phrase { get; set; }

    [Preserve] [JsonProperty("blanks", NullValueHandling = NullValueHandling.Include)]
    public int blanks { get; set; }

    [Preserve] [JsonProperty("blank_info", NullValueHandling = NullValueHandling.Include)]
    public List<BlankData> blank_info { get; set; }
}



[System.Serializable]
[Preserve] // Prevent IL2CPP stripping
[JsonObject(MemberSerialization.OptIn)]
public class BlankData
{
    [Preserve]

    public BlankData()
    {
        this.blank_id = 0;
        this.blank_attributes = new BlankAttributes();
    }

    [Preserve]
    [JsonConstructor]
    public BlankData(int blank_id, BlankAttributes blank_attributes)
    {
        this.blank_id = blank_id;
        this.blank_attributes = blank_attributes ?? new BlankAttributes();
    }

    [Preserve] [JsonProperty("blank_id", NullValueHandling = NullValueHandling.Include)]
    public int blank_id { get; set; }

    [Preserve] [JsonProperty("blank_attributes", NullValueHandling = NullValueHandling.Include)]
    public BlankAttributes blank_attributes { get; set; }
}




[System.Serializable]
[Preserve] // Prevent IL2CPP stripping
public class BlankAttributes
{
    [Preserve]

    public BlankAttributes()
    {
        this.word = "";
        this.PreferredPOS = "";
        this.PreferredCAT = "";
        this.Insult = false;
        this.Tense = "";
    }

    [Preserve]
    [JsonConstructor]
    public BlankAttributes(string word, string PreferredPOS, string PreferredCAT, bool? Insult, string Tense)
    {
        this.word = word ?? "";
        this.PreferredPOS = PreferredPOS ?? "";
        this.PreferredCAT = PreferredCAT ?? "";
        this.Insult = Insult ?? false;
        this.Tense = Tense ?? "";
    }

    [Preserve] [JsonProperty("word", NullValueHandling = NullValueHandling.Include)]
    public string word { get; set; }

    [Preserve] [JsonProperty("PreferredPOS", NullValueHandling = NullValueHandling.Include)]
    public string PreferredPOS { get; set; }

    [Preserve] [JsonProperty("PreferredCAT", NullValueHandling = NullValueHandling.Include)]
    public string PreferredCAT { get; set; }

    [Preserve] [JsonProperty("Insult", NullValueHandling = NullValueHandling.Include)]
    public bool Insult { get; set; }

    [Preserve] [JsonProperty("Tense", NullValueHandling = NullValueHandling.Include)]
    public string Tense { get; set; }
}





