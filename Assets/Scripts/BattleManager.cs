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
    public TMP_Text enemyPhraseText;

    private Bard playerBard;
    private Bard enemyBard;

    private bool isPlayerTurn;
    private int roundNumber;

    private List<Card> playerSelectedCards = new List<Card>(); // Tracks player-selected cards for the phrase
    private List<Card> enemySelectedCards = new List<Card>(); // Tracks enemy-selected cards for the phrase

    private int MoneyEarned = 0;

    public void Initialize(Bard player, Bard enemy)
    {
        enemyPhraseText.gameObject.SetActive(false);
        currentPhraseText.gameObject.SetActive(true);
        
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
        enemyBard.GetDeck().DrawMaxPlayerHandFromLibrary();
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
            Debug.Log("IS PLAYER TURN: " + isPlayerTurn);
            if (isPlayerTurn)
            {
                currentPhraseText.gameObject.SetActive(true);
                Debug.Log("Player's turn.");
                yield return PlayerTurn(); // Wait for the player's turn to finish
            }
            else
            {
                Debug.Log("Enemy's turn.");
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
        
        Debug.Log("Player, complete your roast!");

        // Wait for the player to press the End Turn button.
        yield return new WaitUntil(() => !isPlayerTurn);

        UpdatePhraseUI();
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy is roasting...");

        // Get the current phrase for the enemy
        JournalPhrase currentPhrase = enemyBard.GetJournal().GetCurrentPhrase();

        if (currentPhrase == null)
        {
            Debug.LogError("Enemy's journal does not have a current phrase!");
            yield break;
        }

        // Clear previous selections
        enemySelectedCards.Clear();

        // Randomly select cards from the enemy's hand to fill the phrase blanks
        List<Card> enemyHand = enemyBard.GetDeck().GetHand();
        int blanksToFill = currentPhrase.GetNumBlanks();
        for (int i = 0; i < blanksToFill; i++)
        {
            if (enemyHand.Count == 0) break; // No more cards to select

            // Randomly select a card from the enemy's hand
            Card selectedCard = enemyHand[Random.Range(0, enemyHand.Count)];
            enemySelectedCards.Add(selectedCard);
            enemyHand.Remove(selectedCard); // Remove the card from the hand
        }

        // Update the enemy's phrase text and display it
        string enemyDisplayText = currentPhrase.GetDisplayText(enemySelectedCards);
        enemyPhraseText.text = enemyDisplayText;
        enemyPhraseText.gameObject.SetActive(true);

        // Wait for 3 seconds to display the enemy's completed phrase
        yield return new WaitForSeconds(3f);

        // Hide the enemy phrase text
        enemyPhraseText.gameObject.SetActive(false);

        // Calculate the phrase effect
        int egoDamage = CalculatePhraseEffect(enemyBard, enemySelectedCards);
        playerBard.AddEgo(-egoDamage);

        Debug.Log($"Player's ego reduced by {egoDamage}. Current ego: {playerBard.GetEgo()}.");

        // Select a new phrase for the enemy
        enemyBard.GetJournal().SelectNewPhrase();

        // Simulate enemy drawing new cards
        enemyBard.GetDeck().DiscardHandExcept(enemySelectedCards);
        enemyBard.GetDeck().DrawCards(5);

        isPlayerTurn = true;
        Debug.Log("Enemy turn ended.");
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
        Debug.Log("THIS IS A TEST1");
        if (playerSelectedCards.Count == playerBard.GetJournal().GetCurrentPhrase().GetNumBlanks())
        {
            Debug.Log("THIS IS A TEST2");
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
        if (playerSelectedCards.Contains(card))
        {
            // If the card is already selected, deselect it
            playerSelectedCards.Remove(card);
            UpdateCardOpacity(card, 1.0f); // Reset opacity to 100%
        }
        else if (playerSelectedCards.Count < playerBard.GetJournal().GetCurrentPhrase().GetNumBlanks())
        {
            // Select the card and change its opacity
            playerSelectedCards.Add(card);
            UpdateCardOpacity(card, 0.5f); // Set opacity to 50%
        }

        UpdatePhraseUI();
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
