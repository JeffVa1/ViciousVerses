using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform handArea; // Parent object for card UI elements
    public TMP_Text currentPhraseText; // Displays the current phrase
    public Button endTurnButton; // Button to end the player's turn
    public GameObject cardPrefab; // Prefab for card UI elements

    private Bard playerBard;
    private Bard enemyBard;

    private bool isPlayerTurn;
    private int roundNumber;

    private List<Card> playerSelectedCards = new List<Card>(); // Tracks player-selected cards for the phrase
    private List<Card> enemySelectedCards = new List<Card>(); // Tracks enemy-selected cards for the phrase

    public void Initialize(Bard player, Bard enemy)
    {
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

        Debug.Log("Battle Initialized!");
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        playerBard.GetDeck().DrawMaxPlayerHandFromLibrary();
        StartBattle();
    }

    private void StartBattle()
    {
        Debug.Log("Starting the battle...");
        UpdateHandUI();
        UpdatePhraseUI();
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (playerBard.GetEgo() > 0 && enemyBard.GetEgo() > 0)
        {
            Debug.Log($"Round {roundNumber} begins.");

            if (isPlayerTurn)
            {
                Debug.Log("Player's turn.");
                yield return PlayerTurn();
            }
            else
            {
                Debug.Log("Enemy's turn.");
                yield return EnemyTurn();
            }

            isPlayerTurn = !isPlayerTurn;
            roundNumber++;
        }

        EndBattle();
    }

    private IEnumerator PlayerTurn()
    {
        
        Debug.Log("Player, complete your roast!");

        // Wait for the player to press the End Turn button.
        yield return new WaitUntil(() => !isPlayerTurn);

        UpdatePhraseUI();
    }

    private IEnumerator EnemyTurn()
    {
        // Simulate enemy AI selecting cards and completing a phrase.
        Debug.Log("Enemy is roasting...");
        yield return new WaitForSeconds(3f);

        // Example: Randomly generate ego damage.
        int egoDamage = Random.Range(5, 15);
        playerBard.AddEgo(-egoDamage);

        Debug.Log($"Player's ego reduced by {egoDamage}. Current ego: {playerBard.GetEgo()}.");
    }

    private int CalculatePhraseEffect(Bard bard, List<Card> selectedCards)
    {
        JournalPhrase currentPhrase = bard.GetJournal().GetCurrentPhrase();
        List<string> phraseList = currentPhrase.GetPhraseList();
        int totalDamage = 0;

        foreach (string word in phraseList)
        {
            if (!string.IsNullOrEmpty(word))
            {
                Card card = selectedCards.Find(c => c.GetText() == word);
                if (card != null)
                {
                    totalDamage += card.GetEgoDamage();
                }
            }
        }

        return totalDamage;
    }

    private void EndPlayerTurn()
    {
        if (playerSelectedCards.Count == playerBard.GetJournal().GetCurrentPhrase().GetNumBlanks())
        {
            // Calculate phrase effect if phrase is full.
            int egoDamage = CalculatePhraseEffect(playerBard, playerSelectedCards);
            enemyBard.AddEgo(-egoDamage);
            Debug.Log($"Enemy's ego reduced by {egoDamage}. Current ego: {enemyBard.GetEgo()}.");
            playerBard.GetJournal().SelectNewPhrase();
            // Clear selected cards after completing the phrase.
            playerSelectedCards.Clear();
        }
        else
        {
            Debug.Log("Phrase incomplete. Skipping effect calculation.");
        }

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
        }
        else if (enemyBard.GetEgo() <= 0)
        {
            Debug.Log("The enemy has been roasted! Player wins.");
        }
        else
        {
            Debug.Log("Battle ended unexpectedly.");
        }

        Debug.Log("Battle over.");
    }

    private void UpdateHandUI()
    {
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in playerBard.GetDeck().GetHand())
        {
            GameObject cardObj = Instantiate(cardPrefab, handArea);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card, OnCardSelected);
            //cardObj.GetComponentInChildren<Text>().text = card.GetText();
            //cardObj.GetComponent<Button>().onClick.AddListener(() => OnCardSelected(card));
        }
    }

    private void UpdatePhraseUI()
    {
        if (playerBard == null)
        {
            Debug.LogError("playerBard is null in UpdatePhraseUI!");
            return;
        }

        Journal journal = playerBard.GetJournal();
        if (journal == null)
        {
            Debug.LogError("playerBard's Journal is null in UpdatePhraseUI!");
            return;
        }

        JournalPhrase phrase = journal.GetCurrentPhrase();
        if (phrase == null)
        {
            Debug.LogError("playerBard's Journal does not have a current phrase in UpdatePhraseUI!");
            return;
        }

        string displayText = phrase.GetDisplayText(playerSelectedCards);
        currentPhraseText.text = displayText;
    }

    private void OnCardSelected(Card card)
    {
        if (playerSelectedCards.Contains(card)) return; // Prevent duplicate selection

        playerSelectedCards.Add(card);
        UpdatePhraseUI();
    }
}
