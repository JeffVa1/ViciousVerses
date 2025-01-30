using System.Collections;
using UnityEngine;

public class SceneOneSequence : MonoBehaviour
{
    public SpriteRenderer HillViewRenderer; 
    public SpriteRenderer BarRenderer;     
    public SpriteRenderer PlayerRenderer;  
    public SpriteRenderer BarmaidRenderer; 
    public SpriteRenderer LogoRenderer;    
    public SpriteRenderer Enemy1Renderer;
    public SpriteRenderer Enemy2Renderer;
    public SpriteRenderer Enemy3Renderer;


    public float FadeDuration = 1f; // Duration for fade effects

    public DialogueManager DialogueManager;

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
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(SpriteFadeOut(LogoRenderer));
               
        // Fade in Dialogue
        DialogueManager.Initialize("Assets/Data/openingScene.json");
        StartCoroutine(DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));
        DialogueManager.StartDialogue();

        // On Flag = 1: render player
        yield return StartCoroutine(WaitForFlag(1));
        StartCoroutine(SpriteFadeIn(PlayerRenderer));
       
        // On Flag = 2:
        // Fade out HillView and player and fade in Bar
        yield return StartCoroutine(WaitForFlag(2));
        StartCoroutine(SpriteFadeOut(PlayerRenderer));
        StartCoroutine(SpriteFadeOut(HillViewRenderer));
        StartCoroutine(SpriteFadeIn(BarRenderer));
        
        // Transition to the Bar
        yield return StartCoroutine(WaitForFlag(3));
        StartCoroutine(SpriteFadeIn(BarmaidRenderer));
        yield return StartCoroutine(WaitForFlag(4));
        StartCoroutine(SpriteFadeIn(Enemy1Renderer));
        yield return StartCoroutine(WaitForFlag(5));
        StartCoroutine(SpriteFadeOut(BarmaidRenderer));
        StartCoroutine(SpriteFadeIn(PlayerRenderer));

        // initialze battle
        yield return StartCoroutine(WaitForFlag(6));
        DialogueManager.EndDialogue();

        Debug.Log("Calling StartNextBattle");
        GameManager.Instance.StartNextBattle();
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
