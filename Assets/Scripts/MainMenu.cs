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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
