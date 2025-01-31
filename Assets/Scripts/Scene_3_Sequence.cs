using UnityEngine;
using System.Collections;

public class Scene_3_Sequence : MonoBehaviour
{
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

    private IEnumerator EventSequence(){
        
        // fade in scene assets
        yield return new WaitForSeconds(1);

        // parse json
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
        
    }
}
