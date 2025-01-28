using System.Collections;
using UnityEngine;

public class SceneOneSequence : MonoBehaviour
{
    public SpriteRenderer HillViewRenderer; // Reference to the SpriteRenderer for HillView
    public SpriteRenderer BarRenderer;     // Reference to the SpriteRenderer for Bar
    public SpriteRenderer PlayerRenderer;  // Reference to the SpriteRenderer for Player
    public SpriteRenderer BarmaidRenderer; // Reference to the SpriteRenderer for Barmaid
    public SpriteRenderer LogoRenderer;    // Reference to the SpriteRenderer for Logo


    public float FadeDuration = 1f; // Duration for fade effects

    public DialogueManager DialogueManager;

    public Sprite PlayerSprite;
    public Sprite EnemySprite;
    public Sprite NarratorSprite;

    private CanvasGroup DialogueCanvasGroup;

    void Start()
    {


        DialogueCanvasGroup = DialogueManager.GetCanvasGroup();

        StartCoroutine(EventSequence());
    }

    IEnumerator EventSequence()
    {

        // Logo in
        yield return StartCoroutine(SpriteFadeIn(LogoRenderer));
        // Fade in HillView
        StartCoroutine(SpriteFadeIn(HillViewRenderer));

        // wait and fade logo
        yield return new WaitForSeconds(5);
        yield return StartCoroutine(SpriteFadeOut(LogoRenderer));
               
        // Fade in Dialogue
        DialogueManager.Initialize("Assets/Data/openingScene.json", PlayerSprite, EnemySprite, NarratorSprite);
        StartCoroutine(DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));

        

        // On Flag = 1: render player
        yield return StartCoroutine(WaitForFlag(1));
        StartCoroutine(SpriteFadeIn(PlayerRenderer));
       
        // On Flag = 2:
        // Fade out HillView and player and fade in Bar
        yield return StartCoroutine(WaitForFlag(2));
        StartCoroutine(SpriteFadeOut(HillViewRenderer));
        StartCoroutine(SpriteFadeOut(PlayerRenderer));
        StartCoroutine(SpriteFadeIn(BarRenderer));

        // // Show Player and Barmaid
        // 
        // yield return StartCoroutine(FadeIn(BarmaidRenderer));


        // // Wait for 2 seconds, then fade everything out
        // yield return new WaitForSeconds(2);
        // yield return StartCoroutine(FadeOut(BarRenderer));
        // yield return StartCoroutine(FadeOut(PlayerRenderer));
        // yield return StartCoroutine(FadeOut(BarmaidRenderer));
        // yield return StartCoroutine(FadeOut(LogoRenderer));
    }

    IEnumerator WaitForFlag(int flagValue)
    {
        while (DialogueManager.Flag != flagValue)
        {
            yield return null;
        }
    }

    IEnumerator SpriteFadeIn(SpriteRenderer renderer)
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

    IEnumerator DialogueFade(CanvasGroup DialogueCanvasGroup, float startAlpha, float endAlpha, float FadeDuration)
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

    IEnumerator SpriteFadeOut(SpriteRenderer renderer)
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
