using UnityEngine;
using TMPro;
using System.Collections;

public class ResultsManager : MonoBehaviour
{
    public TextMeshProUGUI egoRemainingValueText;
    public TextMeshProUGUI audienceReactionValueText;
    public TextMeshProUGUI moneyEarnedValueText;
    public TextMeshProUGUI totalGoldValueText;
    public TextMeshProUGUI winLoseText;

    public int egoRemaining = 100;
    public int audienceReaction = 50;
    public int moneyEarned = 200;
    public int totalGold = 1500;
    public float countUpDuration = 1.5f; // Duration for counting up money earned
    public bool win_lose = false;

    private void Start()
    {
        egoRemaining = GameManager.Instance.PlayerBard.GetEgo();
        audienceReaction = GameManager.Instance.prev_audience_score;
        moneyEarned = GameManager.Instance.prev_gold_earned;
        totalGold = GameManager.Instance.PlayerBard.GetMoneyBalance();
        win_lose = GameManager.Instance.WonLastMatch;
        // Clear initial text
        egoRemainingValueText.text = "";
        audienceReactionValueText.text = "";
        moneyEarnedValueText.text = "";
        totalGoldValueText.text = "";

        if (win_lose)
        {
            winLoseText.text = "You Won!";
        }
        else
        {
            winLoseText.text = "You Lost!";
        }

        // Start sequential result display
        StartCoroutine(DisplayResults());
    }

    private IEnumerator DisplayResults()
    {
        // Display Ego Remaining
        egoRemainingValueText.text = egoRemaining.ToString();
        yield return new WaitForSeconds(1f);

        // Display Audience Reaction
        audienceReactionValueText.text = audienceReaction.ToString();
        yield return new WaitForSeconds(1f);

        // Animate Money Earned Count-Up
        yield return StartCoroutine(CountUpMoneyEarned());

        // Display Total Gold
        totalGoldValueText.text = totalGold.ToString();
    }

    private IEnumerator CountUpMoneyEarned()
    {
        int currentGold = 0;
        float elapsedTime = 0f;

        while (elapsedTime < countUpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / countUpDuration;
            currentGold = Mathf.RoundToInt(Mathf.Lerp(0, moneyEarned, progress));
            moneyEarnedValueText.text = currentGold.ToString();
            yield return null;
        }

        // Ensure final value is set correctly
        moneyEarnedValueText.text = moneyEarned.ToString();
    }

    public void OnContinueButtonPressed()
    {
        Debug.Log("Continue button pressed. Implement scene transition or logic here.");
        // Example: SceneManager.LoadScene("NextSceneName");
        GameManager.Instance.GoToNextScene();
    }
}
