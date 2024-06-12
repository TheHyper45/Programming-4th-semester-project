using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour {
    [SerializeField]
    private StateManagement stateManagement;
    [SerializeField]
    private TMP_InputField loginInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private TMP_Text errorMsg;
    [SerializeField]
    private Scrollbar scrollbar;

    private void OnEnable() {
        loginInput.text = "";
        passwordInput.text = "";
        errorMsg.text = "";
        scrollbar.value = 1.0f;
    }

    public void OnLoginButtonClick() {
        errorMsg.text = "";
        if(string.IsNullOrEmpty(loginInput.text)) {
            errorMsg.text = "Login nie mo¿e byæ pusty.";
            return;
        }
        if(string.IsNullOrEmpty(passwordInput.text)) {
            errorMsg.text = "Has³o nie mo¿e byæ puste.";
            return;
        }
        //@TODO: Here you would login a user.
        stateManagement.SwitchToMainMenu();
    }

    public void OnRegisterButtonClick() {
        stateManagement.SwitchToRegisterMenu();
    }
}
