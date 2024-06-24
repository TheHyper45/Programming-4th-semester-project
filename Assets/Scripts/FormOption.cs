using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FormOption : MonoBehaviour {
    [SerializeField]
    private TMP_Text headerText;
    [SerializeField]
    private TMP_Dropdown optionsDropdown;
    [SerializeField]
    private Button removeButton;
    [SerializeField]
    private TMP_InputField priorityInputField;
    [SerializeField]
    private MainMenuFormMenuForm.EntryType entryType;

    public void LateInit(string text,int option,int priority) {
        optionsDropdown.ClearOptions();
        if(entryType == MainMenuFormMenuForm.EntryType.Language) {
            optionsDropdown.AddOptions(DatabaseGeneration.LanguageLevels.ToList());
        }
        else if(entryType == MainMenuFormMenuForm.EntryType.Sport) {
            optionsDropdown.AddOptions(DatabaseGeneration.SportLevels.ToList());
        }
        else if(entryType == MainMenuFormMenuForm.EntryType.Subject) {
            optionsDropdown.AddOptions(DatabaseGeneration.SubjectLevels.ToList());
        }
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => { DestroyImmediate(gameObject); });

        headerText.text = text;
        optionsDropdown.value = option;
        priorityInputField.text = priority.ToString();
    }

    public string GetHeaderText() {
        return headerText.text;
    }

    public string GetOption() {
        return optionsDropdown.options[optionsDropdown.value].text;
    }

    public string GetPriority() {
        return priorityInputField.text;
    }
}
