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
                if (show) {

                    uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

                    uiDocument.rootVisualElement.schedule.Execute(() => {
                        TextField inputField = uiDocument.rootVisualElement.Q<TextField>("ConsoleInput");
                        
                        if (inputField != null) {
                            Debug.Log("DeBuGdS: Focusing");
                            inputField.Focus();
                            inputField.textSelection.selectAllOnFocus = true;
                        }

                    }).ExecuteLater(10);

                } else {
                    uiDocument.rootVisualElement.style.display = DisplayStyle.None;
                }
            }
        }
        
    }
}
