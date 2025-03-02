using UnityEngine;
using TMPro;

public class EventSelectionManager : MonoBehaviour
{
    public TMP_Dropdown InnDropdown;
    public TMP_Dropdown BookStoreDropdown;
    public TMP_Dropdown TravelDropdown;

    void Start()
    {
        InnDropdown.onValueChanged.AddListener(delegate { OnDropdownItemSelected(InnDropdown); });
        BookStoreDropdown.onValueChanged.AddListener(delegate { OnDropdownItemSelected(BookStoreDropdown); });
        TravelDropdown.onValueChanged.AddListener(delegate { OnDropdownItemSelected(TravelDropdown); });
    }

    void OnDropdownItemSelected(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        string selectedItem = dropdown.options[index].text;
        Debug.Log("Selected item: " + selectedItem);

        HandleSceneSelection(selectedItem);
    }

    void HandleSceneSelection(string sceneName)
    {
        Debug.Log("Handle scene selection for: " + sceneName);
    }
}
