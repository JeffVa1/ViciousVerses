using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;

    public void PlayGame()
    {
        if (levelLoader != null)
        {
            levelLoader.CauseSceneTransition();
        }
        else
        {
            Debug.LogWarning("LevelLoader is not assigned! Loading scene without transition.");
            GameManager.Instance.GoToOpening();
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT! ");
        Application.Quit();
    }
}
