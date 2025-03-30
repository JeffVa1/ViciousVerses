using UnityEngine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine.Scripting;


using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance { get; private set; }

    private List<Card> playerCards;
    private List<JournalPhrase> playerPhrases;
    private List<Card> opponentCards;
    private List<JournalPhrase> opponentPhrases;
    private List<Card> shop_cards;
    private List<JournalPhrase> shop_phrases;

    public bool IsLoading { get; private set; }

    public List<Card> PlayerCards
    {
        get { return playerCards; }
        set { playerCards = value; }
    }

    public List<JournalPhrase> PlayerPhrases
    {
        get { return playerPhrases; }
        set { playerPhrases = value; }
    }

    public List<Card> OpponentCards
    {
        get { return opponentCards; }
        set { opponentCards = value; }
    }

    public List<JournalPhrase> OpponentPhrases
    {
        get { return opponentPhrases; }
        set { opponentPhrases = value; }
    }

    public List<Card> ShopCards
    {
        get { return shop_cards; }
        set { shop_cards = value; }
    }

    public List<JournalPhrase> ShopPhrases
    {
        get { return shop_phrases; }
        set { shop_phrases = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        IsLoading = true;
        Debug.Log("Loading Player Data...");
        await LoadPlayerData();
        Debug.Log("Loading Opponent Data...");
        await LoadOpponentData();
        Debug.Log("Loading Shop Data...");
        await LoadShopData();
        Debug.Log("Data loaded successfully!");
        IsLoading = false;
    }

    private async Task LoadPlayerData()
    {
        playerCards = await PlayerCardDataLoader();
        playerPhrases = await PlayerPhraseDataLoader();
    }

    private async Task LoadOpponentData()
    {
        opponentCards = await OpponentCardDataLoader();
        opponentPhrases = await OpponentPhraseDataLoader();
    }

    private async Task LoadShopData()
    {
        shop_cards = await ShopCardDataLoader();
        shop_phrases = await ShopPhraseDataLoader();
    }

    private async Task<List<Card>> PlayerCardDataLoader()
    {
        List<Card> nouns = await ParseCardsFromJson("defaultNouns.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("defaultVerbs.json", "verb");
        List<Card> final_list = new List<Card>(nouns).Concat(verbs).ToList();
        return final_list;
    }

    private async Task<List<JournalPhrase>> PlayerPhraseDataLoader()
    {
        List<JournalPhrase> playerPhrases = await ParsePhrasesFromJson("playerPhrases.json");
        return playerPhrases;
    }

    private async Task<List<Card>> OpponentCardDataLoader()
    {
        List<Card> nouns = await ParseCardsFromJson("opponentNouns.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("opponentVerbs.json", "verb");
        List<Card> final_list = new List<Card>(nouns).Concat(verbs).ToList();
        return final_list;    
    }

    private async Task<List<JournalPhrase>> OpponentPhraseDataLoader()
    {
        List<JournalPhrase> opponentPhrases = await ParsePhrasesFromJson("genericOpponentPhrases.json");
        return opponentPhrases;
    }

    private async Task<List<Card>> ShopCardDataLoader()
    {
        shop_cards = new List<Card>();
        List<Card> nouns = await ParseCardsFromJson("shopVerbs.json", "noun");
        List<Card> verbs = await ParseCardsFromJson("shopNouns.json", "verb");
        shop_cards = nouns.Concat(verbs).ToList();
        return shop_cards;
    }

    private async Task<List<JournalPhrase>> ShopPhraseDataLoader()
    {
        shop_phrases = new List<JournalPhrase>();
        shop_phrases = await ParsePhrasesFromJson("shopPhrases.json");
        return shop_phrases;
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
