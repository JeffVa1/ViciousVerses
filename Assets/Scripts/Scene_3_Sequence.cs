using UnityEngine;
using System.Collections;

public class Scene_3_Sequence : MonoBehaviour
{
    public SpriteRenderer BarRenderer;
    public SpriteRenderer TheaterRenderer;
    public SpriteRenderer PlayerRenderer;
    
    public SpriteRenderer Enemy2Renderer;
    public SpriteRenderer Enemy3Renderer;

    public float FadeDuration = 1f; // Duration for fade effects
    public DialogueManager DialogueManager;
    private CanvasGroup DialogueCanvasGroup;
    private SpriteManager TheSpriteManager;

    void Start()
    {
        TheSpriteManager = GetComponent<SpriteManager>();
        TheSpriteManager.Initialize(FadeDuration, DialogueManager);
        DialogueCanvasGroup = DialogueManager.GetCanvasGroup();
        StartCoroutine(EventSequence());
    }

    private IEnumerator EventSequence()
    {

        Debug.Log("Scene 3 loaded sucessfully");
        yield return new WaitForSeconds(1);
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarRenderer));

        // parse json
        JsonLoader.LoadJson("scene3.json", (jsonData) =>
        {
            if (jsonData != null)
            {
                StartCoroutine(TheSpriteManager.DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));
                DialogueManager.StartDialogueWithJson(jsonData);
            }
            else
            {
                Debug.LogError("DialogueManager initialization failed due to missing or empty JSON Using UWR");
                DialogueManager.Initialize("Assets/Data/scene3.json");
                StartCoroutine(TheSpriteManager.DialogueFade(DialogueCanvasGroup, 0f, 1f, FadeDuration));
                DialogueManager.StartDialogue();
            }
        });
        yield return StartCoroutine(TheSpriteManager.WaitForFlag(1));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(PlayerRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(Enemy2Renderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(2));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(Enemy2Renderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(Enemy3Renderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(3));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(BarRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(TheaterRenderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(4));

        Debug.Log("Calling StartNextBattle");
        GameManager.Instance.StartNextBattle();
    }
}
