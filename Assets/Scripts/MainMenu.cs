using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;

    public void PlayGame()
    {
        levelLoader.CauseTransition();
        GameManager.Instance.GoToOpening();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT! ");
        Application.Quit();
    }
}
