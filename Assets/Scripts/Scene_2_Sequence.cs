using System.Collections;
using UnityEngine;

public class Scene_2_Sequence : MonoBehaviour
{
 
    public SpriteRenderer BarRenderer;     
    public SpriteRenderer PlayerRenderer;  
    public SpriteRenderer BarmaidRenderer;     
    public SpriteRenderer Enemy2Renderer;
    public float FadeDuration = 1f; // Duration for fade effects

    public DialogueManager DialogueManager;
    private CanvasGroup DialogueCanvasGroup;

    void Start()
    {
        DialogueCanvasGroup = DialogueManager.GetCanvasGroup();
        StartCoroutine(EventSequence());
    }

    IEnumerator EventSequence(){
        yield return new WaitForSeconds(FadeDuration);
    }

    
    
}
