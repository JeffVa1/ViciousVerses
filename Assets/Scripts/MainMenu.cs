using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.GoToOpening();
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}