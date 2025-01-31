using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class JsonLoader
{
    public static void LoadJson(string fileName, Action<string> callback)
    {
        if (callback == null)
        {
            Debug.LogError("JsonLoader: Callback cannot be null!");
            return;
        }

        GameObject loaderObject = new GameObject("JsonLoader");
        JsonLoaderBehaviour loader = loaderObject.AddComponent<JsonLoaderBehaviour>();
        loader.StartCoroutine(loader.LoadJsonCoroutine(fileName, callback));
    }

    private class JsonLoaderBehaviour : MonoBehaviour
    {
        public IEnumerator LoadJsonCoroutine(string fileName, Action<string> callback)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

            // Apply correct URI format if needed
            if (!filePath.StartsWith("http") && !filePath.StartsWith("file://"))
            {
                filePath = "file://" + filePath; // Ensures it works on PC/Mac/Linux
            }

            Debug.Log($"JsonLoader: Attempting to load JSON from {filePath}");

            using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    string jsonData = uwr.downloadHandler.text;

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        //Debug.Log($"JsonLoader: Successfully loaded JSON:\n{jsonData}");
                        callback(jsonData);
                    }
                    else
                    {
                        Debug.LogError("JsonLoader: JSON file is empty!");
                        callback(null);
                    }
                }
                else
                {
                    Debug.LogError($"JsonLoader: Failed to load {fileName} - Error: {uwr.error}");
                    callback(null);
                }

                Destroy(gameObject); // Cleanup after completion
            }
        }
    }
}
