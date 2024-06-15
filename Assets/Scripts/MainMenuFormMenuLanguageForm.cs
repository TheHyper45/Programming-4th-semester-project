using UnityEngine;

public class MainMenuFormMenuLanguageForm : MonoBehaviour {
    [SerializeField]
    private GameObject languageOptionPrefab;
    [SerializeField]
    private GameObject allOptions;

    private void AddOption(string name) {
        var option = Instantiate(languageOptionPrefab,allOptions.transform);
        var comp = option.GetComponent<LanguageOption>();
        comp.SetHeaderText(name);
        comp.SetCurrentOption(0);
    }

    private void Start() {
        AddOption("polski");
        AddOption("angielski");
        AddOption("niemiecki");
        AddOption("francuski");
        AddOption("hiszpañski");
    }
}
