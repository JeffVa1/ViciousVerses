using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f; // Default transition time

    private void Awake()
    {
        if (transition == null)
        {
            transition = GetComponent<Animator>(); // Auto-assigns Animator if not set
            if (transition == null)
            {
                Debug.LogError("No Animator found on LevelLoader! Assign it manually in the Inspector.");
            }
        }
    }

    /// Causes the transition animation without changing the scene.
    public void CauseTransition()
    {
        StartCoroutine(PlayCurtainDrop());
    }


    /// Causes the transition animation and then loads the next scene.

    public void CauseSceneTransition()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// Triggers the transition animation without scene change.

    private IEnumerator PlayCurtainDrop()
    {
        Debug.Log("Curtain Drop Triggered...");

        transition.ResetTrigger("Start");
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("Curtain Drop Completed!");
    }


    /// Triggers the transition animation and loads the next scene.

    private IEnumerator LoadLevel(int sceneIndex)
    {
        Debug.Log("Scene Transition Started...");

        transition.ResetTrigger("Start");
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("Loading Scene...");
        SceneManager.LoadScene(sceneIndex);
    }
}
