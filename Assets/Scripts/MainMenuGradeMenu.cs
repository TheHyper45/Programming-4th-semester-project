using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuGradeMenu : MonoBehaviour {
    [SerializeField]
    private GameObject MeetingPrefab;
    [SerializeField]
    private GameObject AllMeetings;
    [SerializeField]
    private Button BlockingPersonButton;
    [SerializeField]
    private Button NotGradeMeetingButton;
    [SerializeField]
    private Button GradeMeetingButton;
    [SerializeField]
    private Button ClosingGradingPanelButton;
    [SerializeField]
    private GameObject GradingMeetingsPanel;
    [SerializeField]
    private TMP_Text DisplayMeetingInfo;
    [SerializeField]
    private TMP_InputField GradeOFTheMeetingInput;
    [SerializeField]
    private DatabaseManagement DatabaseManagement;
    [SerializeField]
    private TMP_Text meetingErrorText;

    private List<DatabaseModel.Meeting> meetings;
    private int currentMeetingIndex;

    private void Start() {
        BlockingPersonButton.onClick.AddListener(OnBlockingPersonButtonClick);
        NotGradeMeetingButton.onClick.AddListener(OnEnable);
        GradeMeetingButton.onClick.AddListener(() => {
            if(!int.TryParse(GradeOFTheMeetingInput.text,out int grade)) {
                meetingErrorText.text = "Ocena jest liczb¹ 0 - 5.";
                return;
            }
            if(grade < 0 || grade > 5) {
                meetingErrorText.text = "Ocena jest liczb¹ 0 - 5.";
                return;
            }

            var meeting = meetings[currentMeetingIndex];
            meetings[currentMeetingIndex] = new(meeting.Year,meeting.Month,meeting.Day,meeting.Hour,meeting.FriendID,grade,meeting.FriendGrade);
            DatabaseManagement.Model.UpdateMeetingsForCurrentAccount(meetings);
            OnEnable();
        });
        ClosingGradingPanelButton.onClick.AddListener(OnEnable);
    }

    private void OnEnable() {
        AllMeetings.SetActive(true);
        GradingMeetingsPanel.SetActive(false);
        while(AllMeetings.transform.childCount > 0) {
            DestroyImmediate(AllMeetings.transform.GetChild(0).gameObject);
        }

        meetings = DatabaseManagement.Model.GetMeetingsForCurrentAccount();
        for(int i = 0;i < meetings.Count;i += 1) {
            AddMeeting(i);
        }

        //This is so weird.
        var rectTransform = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void OnBlockingPersonButtonClick() {
        DatabaseManagement.Model.BanUserForCurrentAccount(meetings[currentMeetingIndex].FriendID);
        OnEnable();
    }

    private void AddMeeting(int meetingIndex) {
        var meeting = meetings[meetingIndex];
        var tempObj = Instantiate(MeetingPrefab,AllMeetings.transform);

        if(meeting.Grade >= 0) {
            tempObj.GetComponentInChildren<Button>().interactable = false;
        }
        else {
            tempObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
                meetingErrorText.text = "";
                GradingMeetingsPanel.SetActive(true);
                DisplayMeetingInfo.text = tempObj.GetComponentInChildren<TMP_Text>().text;
                currentMeetingIndex = meetingIndex;
                AllMeetings.SetActive(false);
            });
        }
        
        var friend = DatabaseManagement.Model.GetUserEntity(meeting.FriendID);
        var friendFullName = $"{friend.FirstName} {friend.LastName}";

        if(meeting.Grade >= 0) {
            tempObj.GetComponentInChildren<TMP_Text>().text = $"{meeting.Hour}:00 {meeting.Day + 1}.{(meeting.Month < 9 ? $"0{meeting.Month}" : $"{meeting.Month}")}.{meeting.Year}\n{friendFullName} Ocena: {meeting.Grade}";
        }
        else {
            tempObj.GetComponentInChildren<TMP_Text>().text = $"{meeting.Hour}:00 {meeting.Day + 1}.{(meeting.Month < 9 ? $"0{meeting.Month}" : $"{meeting.Month}")}.{meeting.Year}\n{friendFullName}";
        }
    }
}
