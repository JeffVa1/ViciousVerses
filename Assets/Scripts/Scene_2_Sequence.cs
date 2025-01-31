using System.Collections;
using UnityEngine;

public class Scene_2_Sequence : MonoBehaviour
{
 
    public SpriteRenderer BarRenderer;     
    // public SpriteRenderer PlayerRenderer;  
    // public SpriteRenderer BarmaidRenderer;     
    // public SpriteRenderer Enemy2Renderer;
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
        
        Debug.Log("Scene 2 loaded sucessfully");
        yield return new WaitForSeconds(1);
        StartCoroutine(TheSpriteManager.SpriteFadeIn(BarRenderer));

        // Debug.Log("Calling StartNextBattle");
        // GameManager.Instance.StartNextBattle();
    }


    
}
