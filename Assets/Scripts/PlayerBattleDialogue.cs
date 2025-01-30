using UnityEngine;

public class PlayerBattleDialogue : MonoBehaviour
{

    private CanvasGroup PlayerCanvasGroup;

   public CanvasGroup GetPlayerCanvasGroup () {

    PlayerCanvasGroup = GetComponent<CanvasGroup>();
    return PlayerCanvasGroup;

   }
}
