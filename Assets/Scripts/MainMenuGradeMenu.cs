using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGradeMenu : MonoBehaviour
{
    [SerializeField] Calendar calendar;
    [SerializeField] GameObject MeetingPrefab;
    [SerializeField] GameObject AllMeetings;
    [SerializeField] Button BlockingPersonButton;
    [SerializeField] Button NotGradeMeetingButton;
    [SerializeField] Button GradeMeetingButton;
    [SerializeField] Button ClosingGradingPanelButton;
    [SerializeField] GameObject GradingMeetingsPanel;
    [SerializeField] TMP_Text DisplayMeetingInfo;
    [SerializeField] TMP_InputField GradeOFTheMeetingInput;
    private Calendar.Meeting currentMeeting;
    // Start is called before the first frame update
    private void Start()
    {
        
        NotGradeMeetingButton.onClick.AddListener(() => { GradingMeetingsPanel.SetActive(false); });
        GradeMeetingButton.onClick.AddListener(() => { GradingMeetingsPanel.SetActive(false); currentMeeting.Grade = int.Parse(GradeOFTheMeetingInput.text);Debug.Log(currentMeeting.Grade); });
        ClosingGradingPanelButton.onClick.AddListener(() => { GradingMeetingsPanel.SetActive(false); });
    }
    private void OnEnable()
    {
        calendar.OnEnable();
        while (AllMeetings.transform.childCount > 0) { 
            DestroyImmediate(AllMeetings.transform.GetChild(0).gameObject);
        }
        AddMeetings();
        GradingMeetingsPanel.SetActive(false);
        
    }
    private void OnDisable()
    {
        calendar.OnDisable();
    }
    public void AddMeetings()
    {
        foreach (var meeting in calendar.meets)
        {
            var tempObj = Instantiate(MeetingPrefab, AllMeetings.transform);
            tempObj.GetComponentInChildren<Button>().onClick.AddListener(() => { GradingMeetingsPanel.SetActive(true); DisplayMeetingInfo.text = tempObj.GetComponentInChildren<TMP_Text>().text; currentMeeting = meeting;  });
            if (meeting.Month <= 9)
            {
                tempObj.GetComponentInChildren<TMP_Text>().text = $"{meeting.Hour}:00 {meeting.Day + 1}.0{meeting.Month}.{meeting.Year}";

            }
            else
            {
                tempObj.GetComponentInChildren<TMP_Text>().text = $"{meeting.Hour}:00 {meeting.Day + 1}.{meeting.Month}.{meeting.Year}";
            }
            //Jako rozwiniecie mozna podac ze spotkania nie w kolejnosci zapisow tylko chronologicznie
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    // Update is called once per frame
    
}
