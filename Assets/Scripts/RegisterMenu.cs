using TMPro;
using System;
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
    private TMP_Dropdown sexDropdown;
    [SerializeField]
    private TMP_Text errorMsg;
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Button registerButton;
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private DatabaseManagement database;

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
        sexDropdown.value = 0;
        errorMsg.text = "";
        scrollbar.value = 1.0f;
    }

    private void OnReturnButtonClick() {
        stateManagement.SwitchToLoginMenu();
    }

    private void OnRegisterButtonClick() {
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
            errorMsg.text = "Numer telefonu nie mo¿e byæ pusty.";
            return;
        }
        if(callNumberInput.text.Length > 12) {
            errorMsg.text = "Numer telefonu mo¿e sk³adaæ siê z maksymalnie 12 cyfr.";
            return;
        }
        if(int.TryParse(callNumberInput.text,out int callNumber)) {
            try {
                var sex = sexDropdown.options[sexDropdown.value].text;
                database.Model.CreateNewAccount(loginInput.text,passwordInput.text,firstNameInput.text,lastNameInput.text,sex,callNumber);
            }
            catch(Exception error) {
                errorMsg.text = error.Message;
                return;
            }
            OnReturnButtonClick();
        }
        else {
            errorMsg.text = "Numer telefonu musi byæ liczb¹.";
            return;
        }
    }
}
