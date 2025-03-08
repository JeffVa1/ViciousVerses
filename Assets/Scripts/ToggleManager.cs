using UnityEngine;
using UnityEngine.UIElements;

public class ToggleManager : MonoBehaviour
{
    private bool isDebugVisible = false;
    
    private void Start()
    {
        ToggleConsole(isDebugVisible);
    }

    
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            Debug.Log("DeBuGdS: Toggle");
            ToggleConsole(!isDebugVisible);
        }
    }

    private void ToggleConsole(bool show)
    {     
        isDebugVisible = show;
        foreach (Transform child in transform) {
            var uiDocument = child.GetComponent<UIDocument>();

            if (uiDocument != null)
            {
                uiDocument.rootVisualElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        
    }
}
