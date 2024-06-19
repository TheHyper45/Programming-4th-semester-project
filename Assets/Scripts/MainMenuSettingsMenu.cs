using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettingsMenu : MonoBehaviour {
    [SerializeField]
    private MainMenu menu;
    [SerializeField]
    private Button logoutButton;
    [SerializeField]
    private Button accountDeleteButton;
    [SerializeField]
    private GameObject accountDeletePrompt;
    [SerializeField]
    private TMP_Text accountDeletePromptText;
    [SerializeField]
    private Button accountDeleteConfirm;
    [SerializeField]
    private Button accountDeleteCancel;
    [SerializeField]
    private DatabaseManagement databaseManagement;

    private void Start() {
        logoutButton.onClick.AddListener(OnLogoutButtonClick);
        accountDeleteButton.onClick.AddListener(() => {
            accountDeleteButton.gameObject.SetActive(false);
            accountDeletePrompt.SetActive(true);
        });
        accountDeleteConfirm.onClick.AddListener(OnDeleteAccountButtonClick);
        accountDeleteCancel.onClick.AddListener(() => {
            accountDeleteButton.gameObject.SetActive(true);
            accountDeletePrompt.SetActive(false);
        });
    }

    private void OnEnable() {
        accountDeleteButton.gameObject.SetActive(true);
        accountDeletePrompt.SetActive(false);
        accountDeletePromptText.text = (databaseManagement.CurrentSex ? "Jesteœ pewien usuniêcia konta?" : "Jesteœ pewna usuniêcia konta?");
    }

    private void OnLogoutButtonClick() {
        menu.Logout();
    }

    private void OnDeleteAccountButtonClick() {
        databaseManagement.DeleteCurrentAccount();
        menu.Logout();
    }
}
