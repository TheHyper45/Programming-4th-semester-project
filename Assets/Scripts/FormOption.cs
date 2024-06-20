using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormOption : MonoBehaviour {
    [SerializeField]
    private TMP_Text headerText;
    [SerializeField]
    private TMP_Dropdown optionsDropdown;
    [SerializeField]
    private Button removeButton;

    private void Start() {
        removeButton.onClick.AddListener(() => { Destroy(gameObject); });
    }

    public void LateInit(string text,int option) {
        headerText.text = text;
        optionsDropdown.value = option;
    }

    public string GetHeaderText() {
        return headerText.text;
    }
}
