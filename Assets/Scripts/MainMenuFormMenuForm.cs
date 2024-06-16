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
    private List<string> possibleOptions;

    public RectTransform RectTransform { get; private set; }

    private void Awake() {
        RectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        possibleOptionDropdown.ClearOptions();
        possibleOptionDropdown.AddOptions(possibleOptions);
        addOptionButton.onClick.AddListener(AddCurrentOption);
    }

    private void AddCurrentOption() {
        var option = Instantiate(optionPrefab,allOptionsContainer.transform);
        option.GetComponent<FormOption>().LateInit(possibleOptionDropdown.options[possibleOptionDropdown.value].text,0);
        //This is so weird.
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
    }

    private void OnEnable() {
        while(allOptionsContainer.transform.childCount > 0) {
            DestroyImmediate(allOptionsContainer.transform.GetChild(0).gameObject);
        }
    }
}
