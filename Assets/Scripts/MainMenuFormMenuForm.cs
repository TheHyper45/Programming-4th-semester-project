using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings.Switch;

public class MainMenuFormMenuForm : MonoBehaviour {
    [SerializeField]
    private DatabaseManagement databaseManagement;
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
    private Button saveButton;
    [SerializeField]
    private Button discardButton;

    public enum EntryType { Language,Sport,Subject }
    [SerializeField]
    private EntryType entryType;

    private float errorTextResetCooldown;

    public RectTransform RectTransform { get; private set; }

    private void Awake() {
        RectTransform = GetComponent<RectTransform>();
        addOptionButton.onClick.AddListener(AddCurrentOption);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        discardButton.onClick.AddListener(OnDiscardButtonClick);

        possibleOptionDropdown.ClearOptions();
        if(entryType == EntryType.Language) {
            possibleOptionDropdown.AddOptions(DatabaseGeneration.Languages.ToList());
        }
        else if(entryType == EntryType.Sport) {
            possibleOptionDropdown.AddOptions(DatabaseGeneration.Sports.ToList());
        }
        else if(entryType == EntryType.Subject) {
            possibleOptionDropdown.AddOptions(DatabaseGeneration.Subjects.ToList());
        }
    }

    private void OnEnable() {
        errorTextResetCooldown = 0.0f;
        errorText.text = "";
        OnDiscardButtonClick();
    }

    private void Update() {
        errorTextResetCooldown -= Time.deltaTime;
        if(errorTextResetCooldown <= 0.0f) {
            errorText.text = "";
            errorTextResetCooldown = 0.0f;
        }
    }

    private void OnSaveButtonClick() {
        List<DatabaseModel.Entity> entities = new();
        foreach(var existingOption in allOptionsContainer.GetComponentsInChildren<FormOption>()) {
            if(!int.TryParse(existingOption.GetPriority(),out int priority)) {
                errorText.text = "Priorytet musi byæ liczb¹ ca³kowit¹.";
                errorTextResetCooldown = 3.0f;
                return;
            }
            entities.Add(new(existingOption.GetHeaderText(),existingOption.GetOption(),priority));
        }
        if(entryType == EntryType.Language) {
            databaseManagement.Model.UpdateLanguagesForCurrentAccount(entities);
        }
        else if(entryType == EntryType.Sport) {
            databaseManagement.Model.UpdateSportsForCurrentAccount(entities);
        }
        else if(entryType == EntryType.Subject) {
            databaseManagement.Model.UpdateSubjectsForCurrentAccount(entities);
        }
    }

    private void OnEnableSetupEntries(List<DatabaseModel.Entity> entities,string[] levels) {
        foreach(var entity in entities) {
            int optionIndex = -1;
            for(int i = 0;i < levels.Length;i += 1) {
                if(levels[i].Equals(entity.Level)) {
                    optionIndex = i;
                    break;
                }
            }
            var entityInstance = Instantiate(optionPrefab,allOptionsContainer.transform);
            entityInstance.GetComponent<FormOption>().LateInit(entity.Name,optionIndex,entity.Priority);
        }
    }

    private void OnDiscardButtonClick() {
        while(allOptionsContainer.transform.childCount > 0) {
            DestroyImmediate(allOptionsContainer.transform.GetChild(0).gameObject);
        }
        if(entryType == EntryType.Language) {
            var languages = databaseManagement.Model.GetLanguagesForCurrentAccount();
            OnEnableSetupEntries(languages,DatabaseGeneration.LanguageLevels);
        }
        else if(entryType == EntryType.Sport) {
            var sports = databaseManagement.Model.GetSportsForCurrentAccount();
            OnEnableSetupEntries(sports,DatabaseGeneration.SportLevels);
        }
        else if(entryType == EntryType.Subject) {
            var subjects = databaseManagement.Model.GetSubjectsForCurrentAccount();
            OnEnableSetupEntries(subjects,DatabaseGeneration.SubjectLevels);
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
        option.GetComponent<FormOption>().LateInit(text,0,0);
        //This is so weird.
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
    }
}
