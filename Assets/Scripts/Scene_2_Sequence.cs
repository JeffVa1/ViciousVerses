using System.Collections;
using UnityEngine;

public class Scene_2_Sequence : MonoBehaviour
{

    public SpriteRenderer BarRenderer;
    public SpriteRenderer PlayerRenderer;
    public SpriteRenderer BarmaidRenderer;
    public SpriteRenderer Enemy1Renderer;
    public SpriteRenderer Enemy2Renderer;
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

        Debug.Log("Scene 2 loaded sucessfully");
        yield return new WaitForSeconds(1);
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarRenderer));

        JsonLoader.LoadJson("scene2.json", (jsonData) =>
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

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(1));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(PlayerRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(Enemy1Renderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(2));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(Enemy1Renderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarmaidRenderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(3));
        StartCoroutine(TheSpriteManager.SpriteFadeOut(BarmaidRenderer));
        StartCoroutine(TheSpriteManager.SpriteFadeIn(Enemy2Renderer));

        yield return StartCoroutine(TheSpriteManager.WaitForFlag(4));

        Debug.Log("Calling StartNextBattle");
        GameManager.Instance.StartNextBattle();
    }



}
