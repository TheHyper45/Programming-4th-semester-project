using TMPro;
using System;
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
    [SerializeField]
    private TMP_Text titleScreenErrorText;

    private List<int> foundMatchingUserIDs;
    private int currentMatchIndex;

    private void Awake() {
        goButton.onClick.AddListener(OnGoButtonClick);
        acceptMatchButton.onClick.AddListener(OnAcceptButtonClick);
        declineMatchButton.onClick.AddListener(OnDeclineButtonClick);
    }

    private void OnEnable() {
        titleScreen.SetActive(true);
        matchingUserScreen.SetActive(false);
        titleScreenErrorText.text = "";
    }

    private void SetupUserMatchingScreen() {
        matchNameText.text = ":(";
        languageInfoText.text = "";
        sportInfoText.text = "";
        subjectInfoText.text = "";
        var userID = foundMatchingUserIDs[currentMatchIndex];
        var user = databaseManagement.Model.GetUserEntity(userID);
        if(user == null) {
            titleScreen.SetActive(true);
            matchingUserScreen.SetActive(false);
            titleScreenErrorText.text = "B³¹d techniczny, prosimy spróbowaæ póŸniej.";
            return;
        }
        matchNameText.text = $"{user.FirstName} {user.LastName}";
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
        try {
            foundMatchingUserIDs = databaseManagement.Model.GetMatchingUserIDsForCurrentAccount();
        }
        catch(Exception error) {
            titleScreenErrorText.text = error.Message;
            return;
        }

        titleScreen.SetActive(false);
        matchingUserScreen.SetActive(true);
        currentMatchIndex = 0;
        SetupUserMatchingScreen();
    }

    private void OnAcceptButtonClick() {
        //databaseManagement.Model.AddFriendForCurrentAccount(foundMatchingUserIDs[currentMatchIndex].Id);
    }

    private void OnDeclineButtonClick() {
        currentMatchIndex = (currentMatchIndex + 1) % foundMatchingUserIDs.Count;
        SetupUserMatchingScreen();
    }
}
