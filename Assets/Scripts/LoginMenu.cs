using TMPro;
using System;
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
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button registerButton;
    [SerializeField]
    private DatabaseManagement database;

    private void Start() {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

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
        try {
            database.Connect(loginInput.text,passwordInput.text);
        }
        catch(DatabaseManagement.NonexistentAccountException) {
            errorMsg.text = "Niepoprawny login lub has³o.";
            return;
        }
        catch(Exception) {
            errorMsg.text = "Wyst¹pi³ b³¹d.";
            return;
        }
        stateManagement.SwitchToMainMenu();
    }

    public void OnRegisterButtonClick() {
        stateManagement.SwitchToRegisterMenu();
    }
}
