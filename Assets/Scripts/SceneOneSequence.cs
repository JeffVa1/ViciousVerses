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

    private SpriteManager TheSpriteManager;

    public float FadeDuration = 1f; // Duration for fade effects

    public DialogueManager DialogueManager;

    private CanvasGroup DialogueCanvasGroup;

    void Start()
    {
        TheSpriteManager = GetComponent<SpriteManager>();
        TheSpriteManager.Initialize(FadeDuration, DialogueManager);
        DialogueCanvasGroup = DialogueManager.GetCanvasGroup();
        StartCoroutine(EventSequence());

    }

    IEnumerator EventSequence()
    {

        // Logo in
        yield return StartCoroutine(TheSpriteManager.SpriteFadeIn(LogoRenderer));
        // Fade in HillView
        StartCoroutine(TheSpriteManager.SpriteFadeIn(HillViewRenderer));

        // wait and fade logo
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TheSpriteManager.SpriteFadeOut(LogoRenderer));

        // Fade in Dialogue
        JsonLoader.LoadJson("openingScene.json", (jsonData) =>
        {
          if (jsonData != null)
          {
              StartCoroutine(TheSpriteManager.DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));
              DialogueManager.StartDialogueWithJson(jsonData);
          }
          else
          {
              Debug.LogError("DialogueManager initialization failed due to missing or empty JSON Using UWR");
              DialogueManager.Initialize("Assets/Data/openingScene.json");
              StartCoroutine(TheSpriteManager.DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));
              DialogueManager.StartDialogue();
          }
        });

        // On Flag = 1: render player
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(1));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(PlayerRenderer));
       
        // On Flag = 2:
        // Fade out HillView and player and fade in Bar
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(2));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(PlayerRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(HillViewRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarRenderer));

        // Transition to the Bar
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(3));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarmaidRenderer));
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(4));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(Enemy1Renderer));
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(5));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(BarmaidRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(PlayerRenderer));

        // initialze battle
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(6));
        DialogueManager.EndDialogue();

        Debug.Log("Calling StartNextBattle");
        GameManager.Instance.StartNextBattle();
    }

    
}
