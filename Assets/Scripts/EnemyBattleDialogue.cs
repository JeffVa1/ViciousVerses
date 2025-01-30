using UnityEngine;

public class EnemyBattleDialogue : MonoBehaviour
{
    
    private CanvasGroup EnemyCanvasGroup;

   public CanvasGroup GetEnemyCanvasGroup () {

    EnemyCanvasGroup = GetComponent<CanvasGroup>();
    return EnemyCanvasGroup;

   }

}
