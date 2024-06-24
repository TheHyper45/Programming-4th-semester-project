using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuTinderMenu : MonoBehaviour {
    [SerializeField]
    private DatabaseManagement databaseManagement;
    [SerializeField]
    private GameObject titleScreen;
    [SerializeField]
    private GameObject matchingUserScreen;
    [SerializeField]
    private Button goButton;
    [SerializeField]
    private Button acceptMatchButton;
    [SerializeField]
    private Button declineMatchButton;
    [SerializeField]
    private TMP_Text matchNameText;
    [SerializeField]
    private TMP_Text languageInfoText;
    [SerializeField]
    private TMP_Text sportInfoText;
    [SerializeField]
    private TMP_Text subjectInfoText;

    private List<DatabaseModel.User> foundMatchingUsers;
    private int currentMatchIndex;

    private void OnEnable() {
        titleScreen.SetActive(true);
        matchingUserScreen.SetActive(false);
        goButton.onClick.AddListener(OnGoButtonClick);
        declineMatchButton.onClick.AddListener(OnDeclineButtonClick);
    }

    private void SetupUserMatchingScreen() {
        var user = foundMatchingUsers[currentMatchIndex];
        matchNameText.text = $"{user.FirstName} {user.LastName}";
        languageInfoText.text = "";
        sportInfoText.text = "";
        subjectInfoText.text = "";
        foreach(var language in user.Languages) {
            languageInfoText.text += $"{language.Name} - {language.Level}\n";
        }
        foreach(var sport in user.Sports) {
            sportInfoText.text += $"{sport.Name} - {sport.Level}\n";
        }
        foreach(var subject in user.Subjects) {
            subjectInfoText.text += $"{subject.Name} - {subject.Level}\n";
        }
    }

    private void OnGoButtonClick() {
        titleScreen.SetActive(false);
        matchingUserScreen.SetActive(true);
        currentMatchIndex = 0;
        foundMatchingUsers = databaseManagement.Model.GetMatchingUsersForCurrentAccount();
        SetupUserMatchingScreen();
    }

    private void OnDeclineButtonClick() {
        currentMatchIndex = (currentMatchIndex + 1) % foundMatchingUsers.Count;
        SetupUserMatchingScreen();
    }
}
