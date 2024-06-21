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
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        while (AllMeetings.transform.childCount > 0) { 
            DestroyImmediate(AllMeetings.transform.GetChild(0).gameObject);
        }
        AddMeetings();
    }
    public void AddMeetings()
    {
        foreach (var meeting in calendar.meets)
        {
            var tempObj = Instantiate(MeetingPrefab, AllMeetings.transform);

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
