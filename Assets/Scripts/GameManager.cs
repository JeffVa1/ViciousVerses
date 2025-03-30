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

    public enum GameState { Menu, Opening, DeckBuilder, Battle, Results, Shop, CutScene, GameOver, Scene_2, Scene_3}
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // This prevents destruction on scene changes
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
        
        StartCoroutine(WaitForDataLoader());

        // Initialize game state
        CurrentState = GameState.Menu;
    }

    private IEnumerator WaitForDataLoader()
    {
        while (DataLoader.Instance == null || DataLoader.Instance.IsLoading)
        {
            yield return null;
        }

        LoadPlayerData();
        LoadOpponentData();
        LoadShopData();
    }

    private void LoadPlayerData()
    {
        List<Card> playerCards = DataLoader.Instance.PlayerCards;
        Dictionary playerDictionary = new Dictionary(playerCards);
        List<JournalPhrase> playerPhrases = DataLoader.Instance.PlayerPhrases;
        PlayerBard = new Bard(playerDictionary, new Journal(playerPhrases));
        PlayerBard.SetRandomDeck();
    }

    private void LoadOpponentData()
    {
        List<Card> opponentCards = DataLoader.Instance.OpponentCards;
        Dictionary opponentDictionary = new Dictionary(opponentCards);
        List<JournalPhrase> opponentPhrases = DataLoader.Instance.OpponentPhrases;
        OpponentBard1 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard2 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard3 = new Bard(opponentDictionary, new Journal(opponentPhrases));
        OpponentBard1.SetRandomDeck();
        OpponentBard2.SetRandomDeck();
        OpponentBard3.SetRandomDeck();
    }

    private void LoadShopData()
    {
        shop_cards = DataLoader.Instance.ShopCards;
        shop_phrases = DataLoader.Instance.ShopPhrases;
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
                    nextScene = GameState.GameOver;
                    currentBattle = 1;
                }
                if (currentBattle == 4)
                {
                    nextScene = GameState.GameOver;
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
            case GameState.GameOver:
                LoadScene("GameOver");
                nextScene = GameState.Opening;
                currentBattle = 1;
                prev_audience_score = 0;
                prev_gold_earned = 0;
                WonLastMatch = false;
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

