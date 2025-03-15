using System;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance.GoToNextScene();
    }
    public void QuitGame()
    {
        Debug.Log("QUIT! ");
        Application.Quit();
        
    }
}
