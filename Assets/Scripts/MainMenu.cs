using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{


    public void Start()
    {
    }


    public void PlayGame()
    {
        SceneManager.LoadScene("TavernScene");
    }
    public void OpenSettings()
    {
        Debug.Log("Settings menu opened!");
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}