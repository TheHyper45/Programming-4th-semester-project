using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFormMenuForm : MonoBehaviour {
    [SerializeField]
    private GameObject optionPrefab;
    [SerializeField]
    private GameObject allOptionsContainer;
    [SerializeField]
    private Button addOptionButton;
    [SerializeField]
    private TMP_Dropdown possibleOptionDropdown;
    [SerializeField]
    private TMP_Text errorText;
    [SerializeField]
    private List<string> possibleOptions;

    private float errorTextResetCooldown;

    public RectTransform RectTransform { get; private set; }

    private void Awake() {
        RectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        possibleOptionDropdown.ClearOptions();
        possibleOptionDropdown.AddOptions(possibleOptions);
        addOptionButton.onClick.AddListener(AddCurrentOption);
    }

    private void OnEnable() {
        errorTextResetCooldown = 0.0f;
        errorText.text = "";
        while(allOptionsContainer.transform.childCount > 0) {
            DestroyImmediate(allOptionsContainer.transform.GetChild(0).gameObject);
        }
    }

    private void Update() {
        errorTextResetCooldown -= Time.deltaTime;
        if(errorTextResetCooldown <= 0.0f) {
            errorText.text = "";
            errorTextResetCooldown = 0.0f;
        }
    }

    private void AddCurrentOption() {
        errorTextResetCooldown = 0.0f;
        errorText.text = "";
        string text = possibleOptionDropdown.options[possibleOptionDropdown.value].text;
        foreach(var existingOption in allOptionsContainer.GetComponentsInChildren<FormOption>()) {
            if(existingOption.GetHeaderText().Equals(text)) {
                errorText.text = "Taka opcja ju¿ istnieje.";
                errorTextResetCooldown = 3.0f;
                return;
            }
        }

        var option = Instantiate(optionPrefab,allOptionsContainer.transform);
        option.GetComponent<FormOption>().LateInit(text,0);
        //This is so weird.
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
    }
}
