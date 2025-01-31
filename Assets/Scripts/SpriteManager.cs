using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour
{

    private float FadeDuration;
    private DialogueManager DialogueManager;
    public void Initialize(float FadeDuration, DialogueManager DialougeManager) {
        this.FadeDuration = FadeDuration;
        this.DialogueManager = DialougeManager;
    }

    public IEnumerator WaitForFlag(int flagValue)
    {
        while (DialogueManager.Flag != flagValue)
        {
            yield return null;
        }
    }

    public IEnumerator SpriteFadeIn(SpriteRenderer renderer)
    {
        float elapsedTime = 0f;
        Color color = renderer.color;
        color.a = 0f; // Start fully transparent
        renderer.color = color;

        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / FadeDuration); // Increase alpha
            renderer.color = color;
            yield return null;
        }

        color.a = 1f; // Ensure it's fully visible
        renderer.color = color;
    }

    public IEnumerator DialogueFade(CanvasGroup DialogueCanvasGroup, float startAlpha, float endAlpha, float FadeDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            DialogueCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / FadeDuration);
            yield return null;
        }

        DialogueCanvasGroup.alpha = endAlpha;
    }

    public IEnumerator SpriteFadeOut(SpriteRenderer renderer)
    {
        float elapsedTime = 0f;
        Color color = renderer.color;
        color.a = 1f; // Start fully visible
        renderer.color = color;

        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / FadeDuration); // Decrease alpha
            renderer.color = color;
            yield return null;
        }

        color.a = 0f; // Ensure it's fully transparent
        renderer.color = color;
    }
}
