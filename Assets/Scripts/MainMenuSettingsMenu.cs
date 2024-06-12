using UnityEngine;

public class MainMenuSettingsMenu : MonoBehaviour {
    [SerializeField]
    private MainMenu menu;

    public void OnLogoutButtonClick() {
        menu.Logout();
    }
}
