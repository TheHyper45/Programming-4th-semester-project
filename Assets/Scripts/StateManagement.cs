using UnityEngine;
using UnityEngine.Assertions;

public class StateManagement : MonoBehaviour {
    [SerializeField]
    private GameObject loginMenu;
    [SerializeField]
    private GameObject registerMenu;
    [SerializeField]
    private GameObject mainMenu;

    private void OnEnable() {
        SwitchToLoginMenu();
    }

    public void SwitchToLoginMenu() {
        SwitchState(loginMenu);
    }

    public void SwitchToRegisterMenu() {
        SwitchState(registerMenu);
    }

    public void SwitchToMainMenu() {
        SwitchState(mainMenu);
    }

    private void SwitchState(GameObject obj) {
        Assert.IsNotNull(obj);
        loginMenu.SetActive(obj == loginMenu);
        registerMenu.SetActive(obj == registerMenu);
        mainMenu.SetActive(obj == mainMenu);
    }
}
