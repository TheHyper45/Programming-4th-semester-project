using TMPro;
using UnityEngine;

public class LanguageOption : MonoBehaviour {
    [SerializeField]
    private TMP_Text headerText;
    [SerializeField]
    private TMP_Dropdown optionsDropdown;

    public void SetHeaderText(string text) {
        headerText.text = text;
    }

    public void SetCurrentOption(int option) {
        optionsDropdown.value = option;
    }
}
