using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public void PlayGame()
    {
        // Debug.Log("Current State, " + GameManager.Instance.CurrentState);
        GameManager.Instance.GoToOpening();
        Debug.Log("Current State, " + GameManager.Instance.CurrentState);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}