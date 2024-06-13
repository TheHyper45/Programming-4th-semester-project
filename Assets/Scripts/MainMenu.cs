using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    private StateManagement stateManagement;
    [SerializeField]
    private GameObject calenderMenu;
    [SerializeField]
    private GameObject gradeMenu;
    [SerializeField]
    private GameObject tinderMenu;
    [SerializeField]
    private GameObject formMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private Button calenderButton;
    [SerializeField]
    private Button gradeButton;
    [SerializeField]
    private Button tinderButton;
    [SerializeField]
    private Button formButton;
    [SerializeField]
    private Button settingsButton;

    private void Start() {
        calenderButton.onClick.AddListener(SwitchToCalenderMenu);
        gradeButton.onClick.AddListener(SwitchToGradeMenu);
        tinderButton.onClick.AddListener(SwitchToTinderMenu);
        formButton.onClick.AddListener(SwitchToFormMenu);
        settingsButton.onClick.AddListener(SwitchToSettingsMenu);
    }

    private void OnEnable() {
        SwitchState(tinderMenu);
    }

    public void SwitchToCalenderMenu() {
        SwitchState(calenderMenu);
    }

    public void SwitchToGradeMenu() {
        SwitchState(gradeMenu);
    }

    public void SwitchToTinderMenu() {
        SwitchState(tinderMenu);
    }

    public void SwitchToFormMenu() {
        SwitchState(formMenu);
    }

    public void SwitchToSettingsMenu() {
        SwitchState(settingsMenu);
    }

    private void SwitchState(GameObject obj) {
        Assert.IsNotNull(obj);
        calenderMenu.SetActive(obj == calenderMenu);
        gradeMenu.SetActive(obj == gradeMenu);
        tinderMenu.SetActive(obj == tinderMenu);
        formMenu.SetActive(obj == formMenu);
        settingsMenu.SetActive(obj == settingsMenu);
    }

    public void Logout() {
        stateManagement.SwitchToLoginMenu();
    }
}
