using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class BattleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform handArea; // Parent object for card UI elements
    public TMP_Text currentPhraseText; // Displays the current phrase
    public Button endTurnButton; // Button to end the player's turn
    public GameObject cardPrefab; // Prefab for card UI elements
    public TMP_Text enemyPhraseText;

    private Bard playerBard;
    private Bard enemyBard;

    private bool isPlayerTurn;
    private int roundNumber;

    private List<Card> playerSelectedCards = new List<Card>(); // Tracks player-selected cards for the phrase
    private List<Card> enemySelectedCards = new List<Card>(); // Tracks enemy-selected cards for the phrase

    private Dictionary<Card, float> cardOpacityState = new Dictionary<Card, float>(); // Track card opacity state

    private int MoneyEarned = 0;
    private int AudienceScore = 0;

    private CanvasGroup PlayerCanvasGroup;
    private CanvasGroup EnemyCanvasGroup;

    public PlayerBattleDialogue PlayerBattleDialogue;

    public EnemyBattleDialogue EnemyBattleDialogue;

    public PlayerMeters PlayersMeters;





    public void Initialize(Bard player, Bard enemy)
    {
        PlayerCanvasGroup = PlayerBattleDialogue.GetPlayerCanvasGroup();
        PlayersMeters = FindAnyObjectByType<PlayerMeters>();
        PlayersMeters.Initialize();
        

        EnemyCanvasGroup = EnemyBattleDialogue.GetEnemyCanvasGroup();
        
        

        enemyPhraseText.gameObject.SetActive(false);
        EnemyCanvasGroup.alpha = 0;

        currentPhraseText.gameObject.SetActive(true);
        PlayerCanvasGroup.alpha = 1;
        
        playerBard = player;
        if (playerBard.GetJournal().GetCurrentPhrase() == null) {
            playerBard.GetJournal().SelectNewPhrase();
        }

        enemyBard = enemy;
        if (enemyBard.GetJournal().GetCurrentPhrase() == null) {
            enemyBard.GetJournal().SelectNewPhrase();
        }

        isPlayerTurn = true;
        roundNumber = 1;

        //Debug.Log("Battle Initialized!");
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        playerBard.GetDeck().DrawMaxPlayerHandFromLibrary();
        enemyBard.GetDeck().DrawMaxPlayerHandFromLibrary();
        StartBattle();
    }

    private void StartBattle()
    {
        //Debug.Log("Starting the battle...");
        UpdateHandUI();
        UpdatePhraseUI();
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (playerBard.GetEgo() > 0 && enemyBard.GetEgo() > 0)
        {
            //Debug.Log($"Round {roundNumber} begins.");
            //Debug.Log("IS PLAYER TURN: " + isPlayerTurn);
            if (isPlayerTurn)
            {
                
                currentPhraseText.gameObject.SetActive(true);
                EnemyCanvasGroup.alpha = 0;
                PlayerCanvasGroup.alpha = 1;
                //Debug.Log("Player's turn.");
                yield return PlayerTurn(); // Wait for the player's turn to finish
            }
            else
            {
                EnemyCanvasGroup.alpha = 1;
                PlayerCanvasGroup.alpha = 0;
                //Debug.Log("Enemy's turn.");
                yield return EnemyTurn(); // Wait for the enemy's turn to finish
            }

            // Alternate turns
            //isPlayerTurn = !isPlayerTurn;

            // Increment the round number after both players have taken their turns
            if (isPlayerTurn)
            {
                roundNumber++;
            }
        }

        EndBattle();
    }


    private IEnumerator PlayerTurn()
    {
        
        //Debug.Log("Player, complete your roast!");

        // Wait for the player to press the End Turn button.
        yield return new WaitUntil(() => !isPlayerTurn);

        UpdatePhraseUI();
    }

    private IEnumerator EnemyTurn()
    {
        JournalPhrase currentPhrase = enemyBard.GetJournal().GetCurrentPhrase();
        if (currentPhrase == null)
        {
            yield break;
        }

        enemySelectedCards.Clear();

        List<Card> enemyHand = new List<Card>(enemyBard.GetDeck().GetHand());
        int blanksToFill = currentPhrase.GetNumBlanks();

        List<List<Card>> possibleCombinations = GetCombinations(enemyHand, blanksToFill);
        List<(List<Card>, int)> scoredCombinations = possibleCombinations
            .Select(cards => (cards, CalculatePhraseEffect(enemyBard, cards)))
            .OrderByDescending(result => result.Item2)
            .ToList();

        for (int i = 0; i < Mathf.Min(5, scoredCombinations.Count); i++)
        {
            Debug.Log($"Top {i + 1} enemy option: {string.Join(", ", scoredCombinations[i].Item1.Select(c => c.GetText()))} with score {scoredCombinations[i].Item2}");
        }

        int targetIndex = 0;
        float damageModifier = 0f;
        if (GameManager.Instance.currentBattle == 1)
        {
            targetIndex = 4;
            damageModifier = 0.3f;
        } 
        else if (GameManager.Instance.currentBattle == 2)
        {
            targetIndex = 2;
            damageModifier = 0.6f;
        }
        else if (GameManager.Instance.currentBattle == 3)
        {
            targetIndex = 0;
            damageModifier = 1f;
        }

        targetIndex = Mathf.Clamp(targetIndex, 0, scoredCombinations.Count - 1);
        enemySelectedCards = scoredCombinations.Count > targetIndex ? scoredCombinations[targetIndex].Item1 : new List<Card>();

        string enemyDisplayText = currentPhrase.GetDisplayText(enemySelectedCards);
        enemyPhraseText.text = enemyDisplayText;
        enemyPhraseText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        enemyPhraseText.gameObject.SetActive(false);

        int egoDamage =  Mathf.RoundToInt(CalculatePhraseEffect(enemyBard, enemySelectedCards) * damageModifier);

        playerBard.AddEgo(-egoDamage);
        PlayersMeters.Meter.TakeFromBar("hp", egoDamage);
        

        enemyBard.GetJournal().SelectNewPhrase();
        enemyBard.GetDeck().DiscardHandExcept(enemySelectedCards);
        enemyBard.GetDeck().DrawCards(5);

        isPlayerTurn = true;
    }

    private List<List<Card>> GetCombinations(List<Card> hand, int size)
    {
        List<List<Card>> results = new List<List<Card>>();
        GetCombinationsRecursive(hand, new List<Card>(), size, 0, results);
        return results;
    }

    private void GetCombinationsRecursive(List<Card> hand, List<Card> current, int size, int index, List<List<Card>> results)
    {
        if (current.Count == size)
        {
            results.Add(new List<Card>(current));
            return;
        }
        for (int i = index; i < hand.Count; i++)
        {
            current.Add(hand[i]);
            GetCombinationsRecursive(hand, current, size, i + 1, results);
            current.RemoveAt(current.Count - 1);
        }
    }


    private int CalculatePhraseEffect(Bard bard, List<Card> selectedCards)
    {
        JournalPhrase currentPhrase = bard.GetJournal().GetCurrentPhrase();
        if (currentPhrase == null)
        {
            //Debug.LogError("Current phrase is null. Cannot calculate phrase effect.");
            return 0;
        }

        List<Blank> blanksInfo = currentPhrase.GetBlanksInfo();
        if (blanksInfo == null || blanksInfo.Count == 0)
        {
            //Debug.LogError("Blanks info is null or empty. Cannot calculate phrase effect.");
            return 0;
        }

        int totalDamage = 0;
        int audienceReaction = 0;
        int totalMultiplier = 1;

        for (int i = 0; i < blanksInfo.Count; i++)
        {
            if (i >= selectedCards.Count)
            {
                //Debug.LogWarning($"Not enough cards selected to fill all blanks. Skipping blank {i + 1}.");
                continue;
            }

            Blank blank = blanksInfo[i];
            Card card = selectedCards[i];


            bool posMatch = blank.PreferredPOS == null || blank.PreferredPOS == card.GetPartOfSpeech();
            bool categoryMatch = blank.PreferredCAT == null || card.GetCategories().Contains(blank.PreferredCAT);
            bool insultMatch = blank.Insult == null || blank.Insult == card.IsInsult();


            //Debug.Log($"Blank {i + 1}: POS Match: {posMatch}, CAT Match: {categoryMatch}, Insult Match: {insultMatch}");
            if (isPlayerTurn)
            {
                if (posMatch) audienceReaction += 2; // Example reaction score for POS match
                if (categoryMatch) audienceReaction += 4; // Example reaction score for category match
                if (insultMatch) audienceReaction += 8; // Example reaction score for insult match
                AudienceScore += audienceReaction;
            }
           

            // Increase damage - if all matching attributes, add multiplier to cards damage.
            if (posMatch && categoryMatch && insultMatch)
            {
                totalDamage += (card.GetEgoDamage() * card.GetMultiplier());
            } else {
                totalDamage += card.GetEgoDamage();
            }
        }

        //Debug.Log($"Total Audience Reaction: {audienceReaction}");
        //Debug.Log($"Total Ego Damage: {totalDamage}");

        return Mathf.RoundToInt(totalDamage) * 5;
    }

    private void EndPlayerTurn()
    {
        if (playerSelectedCards.Count == playerBard.GetJournal().GetCurrentPhrase().GetNumBlanks())
        {
            // Calculate phrase effect if phrase is full.
            int egoDamage = CalculatePhraseEffect(playerBard, playerSelectedCards);
            enemyBard.AddEgo(-egoDamage);
            //Debug.Log($"Enemy's ego reduced by {egoDamage}. Current ego: {enemyBard.GetEgo()}.");
            playerBard.GetJournal().SelectNewPhrase();
            // Clear selected cards after completing the phrase.
            playerSelectedCards.Clear();
            cardOpacityState.Clear();
        }
        else
        {
            //Debug.Log("Phrase incomplete. Skipping effect calculation.");
        }

        currentPhraseText.gameObject.SetActive(false);
        // Discard remaining cards and draw a new hand.
        playerBard.GetDeck().DiscardHandExcept(playerSelectedCards);
        playerBard.GetDeck().DrawCards(5);
        UpdateHandUI();
        UpdatePhraseUI();

        isPlayerTurn = false; // End the player's turn.
    }

    private void EndBattle()
    {
        if (playerBard.GetEgo() <= 0)
        {
            Debug.Log("The player has been roasted! Enemy wins.");
            playerBard.SetEgo(0);
            GameManager.Instance.prev_audience_score = AudienceScore;
            GameManager.Instance.prev_gold_earned = MoneyEarned;
            GameManager.Instance.PlayerBard.AddOrRemoveMoney(MoneyEarned);
            GameManager.Instance.WonLastMatch = false;
            GameManager.Instance.GoToResults();
        }
        else if (enemyBard.GetEgo() <= 0)
        {
            Debug.Log("The enemy has been roasted! Player wins.");
            GameManager.Instance.prev_audience_score = AudienceScore;
            MoneyEarned = AudienceScore + (int)(Math.Floor((double)(playerBard.GetEgo())));
            GameManager.Instance.prev_gold_earned = MoneyEarned;
            GameManager.Instance.PlayerBard.AddOrRemoveMoney(MoneyEarned);
            GameManager.Instance.WonLastMatch = true;
            GameManager.Instance.GoToResults();
        }
        else
        {
            Debug.Log("Battle ended unexpectedly.");
            GameManager.Instance.prev_audience_score = AudienceScore;
            GameManager.Instance.prev_gold_earned = MoneyEarned;
            GameManager.Instance.PlayerBard.AddOrRemoveMoney(MoneyEarned);
            GameManager.Instance.WonLastMatch = false;
            GameManager.Instance.GoToResults();
        }
    }

    private void UpdateHandUI()
    {
        // Destroy previous card UI elements
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new card UI elements
        foreach (Card card in playerBard.GetDeck().GetHand())
        {
            GameObject cardObj = Instantiate(cardPrefab, handArea);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card, OnCardSelected);

            // Apply saved opacity state to the card
            if (cardOpacityState.ContainsKey(card))
            {
                cardUI.SetCardOpacity(cardOpacityState[card]);
            }
            else
            {
                cardUI.SetCardOpacity(1.0f); // Default to full opacity if not in the dictionary
            }

            // Add EventTrigger to handle hover actions
            EventTrigger eventTrigger = cardObj.AddComponent<EventTrigger>();
            
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => cardUI.OnPointerEnter((PointerEventData)data));
            eventTrigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => cardUI.OnPointerExit((PointerEventData)data));
            eventTrigger.triggers.Add(entryExit);
        }
    }



    private void UpdatePhraseUI()
    {
        if (playerBard == null)
        {
            //Debug.LogError("playerBard is null in UpdatePhraseUI!");
            return;
        }

        Journal journal = playerBard.GetJournal();
        if (journal == null)
        {
            //Debug.LogError("playerBard's Journal is null in UpdatePhraseUI!");
            return;
        }

        JournalPhrase phrase = journal.GetCurrentPhrase();
        if (phrase == null)
        {
            //Debug.LogError("playerBard's Journal does not have a current phrase in UpdatePhraseUI!");
            return;
        }

        string displayText = phrase.GetDisplayText(playerSelectedCards);
        currentPhraseText.text = displayText;
    }

    private void OnCardSelected(Card card)
    {
        if (playerSelectedCards.Contains(card))
        {
            // If the card is already selected, deselect it
            playerSelectedCards.Remove(card);
            cardOpacityState[card] = 1.0f; // Reset opacity to 100%
        }
        else if (playerSelectedCards.Count < playerBard.GetJournal().GetCurrentPhrase().GetNumBlanks())
        {
            // Select the card and change its opacity
            playerSelectedCards.Add(card);
            cardOpacityState[card] = 0.5f; // Set opacity to 50%
        }

        UpdatePhraseUI();
        UpdateHandUI();
    }


    private void UpdateCardOpacity(Card card, float opacity)
    {
        // Find the corresponding card object in the hand UI
        foreach (Transform child in handArea)
        {
            CardUI cardUI = child.GetComponent<CardUI>();
            if (cardUI != null && cardUI.GetCard() == card)
            {
                cardUI.SetCardOpacity(opacity); // Call the CardUI method to update opacity
                break;
            }
        }
    }
}
