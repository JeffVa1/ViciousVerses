using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance.GoToNextScene();
    }
}
