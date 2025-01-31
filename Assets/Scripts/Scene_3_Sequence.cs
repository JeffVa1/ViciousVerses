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
        
        Debug.Log("Scene 3 loaded sucessfully");
        yield return new WaitForSeconds(1);
        
    }
}
