using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettingsMenu : MonoBehaviour {
    [SerializeField]
    private MainMenu menu;
    [SerializeField]
    private Button logoutButton;

    private void Start() {
        logoutButton.onClick.AddListener(OnLogoutButtonClick);
    }

    public void OnLogoutButtonClick() {
        menu.Logout();
    }
}
