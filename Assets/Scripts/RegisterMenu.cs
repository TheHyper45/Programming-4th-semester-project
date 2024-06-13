using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterMenu : MonoBehaviour {
    [SerializeField]
    private StateManagement stateManagement;
    [SerializeField]
    private TMP_InputField loginInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private TMP_InputField passwordRepeatInput;
    [SerializeField]
    private TMP_InputField firstNameInput;
    [SerializeField]
    private TMP_InputField lastNameInput;
    [SerializeField]
    private TMP_InputField callNumberInput;
    [SerializeField]
    private TMP_InputField emailInput;
    [SerializeField]
    private TMP_Text errorMsg;
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Button registerButton;
    [SerializeField]
    private Button returnButton;

    private void Start() {
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        returnButton.onClick.AddListener(OnReturnButtonClick);
    }

    private void OnEnable() {
        loginInput.text = "";
        passwordInput.text = "";
        passwordRepeatInput.text = "";
        firstNameInput.text = "";
        lastNameInput.text = "";
        callNumberInput.text = "";
        emailInput.text = "";
        errorMsg.text = "";
        scrollbar.value = 1.0f;
    }

    public void OnReturnButtonClick() {
        stateManagement.SwitchToLoginMenu();
    }

    public void OnRegisterButtonClick() {
        errorMsg.text = "";
        if(string.IsNullOrEmpty(loginInput.text)) {
            errorMsg.text = "Login nie mo¿e byæ pusty.";
            return;
        }
        if(string.IsNullOrEmpty(passwordInput.text)) {
            errorMsg.text = "Has³o nie mo¿e byæ puste.";
            return;
        }
        if(!passwordInput.text.Equals(passwordRepeatInput.text)) {
            errorMsg.text = "Has³a siê nie zgadzaj¹.";
            return;
        }
        if(string.IsNullOrEmpty(firstNameInput.text)) {
            errorMsg.text = "Imiê nie mo¿e byæ puste.";
            return;
        }
        if(string.IsNullOrEmpty(lastNameInput.text)) {
            errorMsg.text = "Nazwisko nie mo¿e byæ puste.";
            return;
        }
        if(string.IsNullOrEmpty(callNumberInput.text)) {
            errorMsg.text = "Nr telefonu nie mo¿e byæ pusty.";
            return;
        }
        if(string.IsNullOrEmpty(emailInput.text)) {
            errorMsg.text = "Adres e-mail nie mo¿e byæ pusty.";
            return;
        }
        //@TODO: Here you would send a request to the database to create an account.
        OnReturnButtonClick();
    }
}
