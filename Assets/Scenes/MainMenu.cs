using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehavior
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
{
    
}
