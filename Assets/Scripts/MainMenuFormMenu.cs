using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MainMenuFormMenu : MonoBehaviour {
    [SerializeField]
    private MainMenu menu;
    [SerializeField]
    private GameObject languageForm;
    [SerializeField]
    private GameObject sportForm;
    [SerializeField]
    private GameObject subjectForm;
    [SerializeField]
    private Button languageFormButton;
    [SerializeField]
    private Button sportFormButton;
    [SerializeField]
    private Button subjectFormButton;

    private void Start() {
        languageFormButton.onClick.AddListener(SwitchToLanguageForm);
        sportFormButton.onClick.AddListener(SwitchToSportForm);
        subjectFormButton.onClick.AddListener(SwitchToSubjectForm);
    }

    private void OnEnable() {
        SwitchToLanguageForm();
    }

    public void SwitchToLanguageForm() {
        SwitchState(languageForm);
    }

    public void SwitchToSportForm() {
        SwitchState(sportForm);
    }

    public void SwitchToSubjectForm() {
        SwitchState(subjectForm);
    }

    private void SwitchState(GameObject obj) {
        Assert.IsNotNull(obj);
        languageForm.SetActive(obj == languageForm);
        sportForm.SetActive(obj == sportForm);
        subjectForm.SetActive(obj == subjectForm);
    }
}
