using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
/// TO DO: 1.Add button to come back to current month
/// To Do layoutgroup zeby sie nie rozjezdzal
/// dodanie info z ID Osoby
public class Calendar : MonoBehaviour {
    [SerializeField]
    private GameObject AddingMeetingsPanel;
    [SerializeField]
    private TMP_Text DataAtThePanel; 
    [SerializeField]
    private Button addingMeetingsButton; 
    [SerializeField]
    private Button deleteMeetingsButton;
    [SerializeField]
    private Button closeMeetingsButton;
    [SerializeField]
    private TMP_InputField HourOfMeetingText;
    [SerializeField]
    private TMP_Text WholeAboutMeetingText;
    [SerializeField]
    private Button leftClickMonthButton;
    [SerializeField]
    private Button rightClickMonthButton;
    [SerializeField]
    private TMP_Text errorText;

    public Transform[] weeks;
    public TMP_Text MonthAndYear;

    private readonly List<Day> days = new();
    public List<Meeting> meets = new();
    private DateTime currDate = DateTime.Now;
    private int dayToCalculation = 0;

    private float errorTextClearCooldown = 0.0f;

    public class Day {
        public int DayNum { get; private set; }
        public Color DayColor { get; private set; }
        public GameObject Obj { get; private set; }

        public Day(int _dayNum,Color _dayColor,GameObject _obj) {
            Obj = _obj;
            UpdateColor(_dayColor);
            UpdateDay(_dayNum);
        }

        public void UpdateColor(Color newColor) {
            Obj.GetComponent<Image>().color = newColor;
            DayColor = newColor;
        }

        public void UpdateDay(int newDayNum) {
            DayNum = newDayNum;
            Color targetColor = ColorUtility.HexToColor("8C52FF");
            if (DayColor == Color.white || DayColor == targetColor) {
                Obj.GetComponentInChildren<TMP_Text>().text = (DayNum + 1).ToString();
            }
            else {
                Obj.GetComponentInChildren<TMP_Text>().text = "";
            }
        }
    }

    public class Meeting {
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }
        public int Hour { get; private set; }
        public int PersonID { get; private set; }
        public int Grade { get; set; }

        public Meeting(int year,int month,int day,int hour,int personID) {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            PersonID = personID;
            Grade = 0;
        }
        public new string ToString()
        {
            return $"{Day}/{Month}/{Year}";
        }
    }

    private void Start() {
        leftClickMonthButton.onClick.AddListener(() => {
            currDate = currDate.AddMonths(-1);
            UpdateCalendar(currDate.Year,currDate.Month);
        });
        rightClickMonthButton.onClick.AddListener(() => {
            currDate = currDate.AddMonths(1);
            UpdateCalendar(currDate.Year,currDate.Month);
        });

        UpdateCalendar(currDate.Year,currDate.Month);
        AddingMeetingsPanel.SetActive(false);
        addingMeetingsButton.onClick.AddListener(AddingMeetings);
        deleteMeetingsButton.onClick.AddListener(DeleteMeetings);
        closeMeetingsButton.onClick.AddListener(() => { AddingMeetingsPanel.SetActive(false); });
    }

    private void Update() {
        errorTextClearCooldown -= Time.deltaTime;
        if(errorTextClearCooldown < 0.0f) {
            errorText.text = "";
            errorTextClearCooldown = 0.0f;
        }
    }

    //Says '0 refs' but it is used in the editor.
    public void ClickingOnDayToSetUpMeetings(int day) {
        dayToCalculation = day;
        int startDay = GetMonthStartDay(currDate.Year,currDate.Month);
        AddingMeetingsPanel.SetActive(true);
        DataAtThePanel.text = (day - startDay + 1).ToString() + " " + MonthAndYear.text;
        ShowMeeting();
    }

    void UpdateCalendar(int year,int month) {
        DateTime temp = new DateTime(year,month,1);
        currDate = temp;
        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();
        int startDay = GetMonthStartDay(year,month);
        int endDay = DateTime.DaysInMonth(year,month);

        if (days.Count == 0)
        {
            for (int w = 0; w < 6; ++w)
            {
                for (int i = 0; i < 7; ++i)
                {
                    Day newDay;
                    int currDay = (w * 7) + i;
                    if (currDay < startDay || currDay - startDay >= endDay)
                    {
                        Button button  = weeks[w].GetChild(i).GetComponentInChildren<Button>();
                        newDay = new Day(currDay - startDay, Color.grey, weeks[w].GetChild(i).gameObject);
                        weeks[w].GetChild(i).GetChild(1).gameObject.SetActive(false);
                        button.enabled = false;
                        
                    }
                    else
                    {
                        newDay = new Day(currDay - startDay, Color.white, weeks[w].GetChild(i).gameObject);
                        Button button = weeks[w].GetChild(i).GetComponentInChildren<Button>();
                        button.enabled = true;
                        bool hasMeeting = false;
                        foreach (var meeeting in meets)
                        {
                            if (meeeting.Year == year && meeeting.Month == month && meeeting.Day == currDay - startDay)
                            {
                                hasMeeting = true;
                                weeks[w].GetChild(i).GetChild(1).gameObject.SetActive(true);
                                break;
                            }
                            
                        }
                        if (!hasMeeting) {
                            weeks[w].GetChild(i).GetChild(1).gameObject.SetActive(false);
                        }
                        
                    }
                    days.Add(newDay);
                }
            }
        }
        else
        {
            for (int i = 0; i < 42; i++)
            {
                if (i < startDay || i - startDay >= endDay)
                {
                    days[i].UpdateColor(Color.grey);
                   
                    weeks[i/7].GetChild(i%7).GetChild(1).gameObject.SetActive(false);
                    Button button = weeks[i / 7].GetChild(i % 7).GetComponentInChildren<Button>();
                    button.enabled= false;
                }
                else
                {

                    days[i].UpdateColor(Color.white);
                    weeks[i / 7].GetChild(i % 7).GetChild(1).gameObject.SetActive(true);
                    Button button = weeks[i / 7].GetChild(i % 7).GetComponentInChildren<Button>();
                    button.enabled = true;
                    bool hasMeeting = false;
                    foreach (var meeeting in meets)
                    {
                        if (meeeting.Year == year && meeeting.Month == month && meeeting.Day == i - startDay)
                        {
                            hasMeeting = true;
                            weeks[i / 7].GetChild(i % 7).GetChild(1).gameObject.SetActive(true);
                            break;
                        }
                        
                    }
                    if (!hasMeeting)
                    {
                        weeks[i / 7].GetChild(i % 7).GetChild(1).gameObject.SetActive(false);
                    }
                }

                days[i].UpdateDay(i - startDay);
            }
        }

        if(DateTime.Now.Year == year && DateTime.Now.Month == month) {
            days[(DateTime.Now.Day - 1) + startDay].UpdateColor(ColorUtility.HexToColor("8C52FF"));
        }
    }

    private void AddingMeetings() {
        int startDay = GetMonthStartDay(currDate.Year,currDate.Month);
        string temp = HourOfMeetingText.text;
        if(int.TryParse(temp,out int hour)) {
            if(hour < 0 || hour >= 24) {
                errorText.text = "Spotkanie mo¿e byæ umówione w godzinach 0-23.";
                errorTextClearCooldown = 3.0f;
                return;
            }
            foreach(var meeting in meets) {
                if(meeting.Year == currDate.Year && meeting.Month == currDate.Month && meeting.Day == dayToCalculation - startDay) {
                    errorText.text = "Ju¿ istnieje w tym dniu spotkanie.";
                    errorTextClearCooldown = 3.0f;
                    return;
                }
            }
            AddingMeetingsPanel.SetActive(false);
            meets.Add(new(currDate.Year,currDate.Month,dayToCalculation - startDay,hour,1));
            UpdateCalendar(currDate.Year,currDate.Month);
            
        }
        else {
            errorText.text = "Musisz podaæ godzinê.";
            errorTextClearCooldown = 3.0f;
        }
    }

    public void ShowMeeting() {
        int startDay = GetMonthStartDay(currDate.Year,currDate.Month);
        foreach(var meeting in meets) {
            if(meeting.Year == currDate.Year && meeting.Month == currDate.Month && meeting.Day == dayToCalculation - startDay) {
                if(meeting.Month <= 9) {
                    WholeAboutMeetingText.text = $"{meeting.Hour}:00 {meeting.Day + 1}.0{meeting.Month}.{meeting.Year}";
                }
                else {
                    WholeAboutMeetingText.text = $"{meeting.Hour}:00 {meeting.Day + 1}.{meeting.Month}.{meeting.Year}";
                }
                return;
            }
        }
        WholeAboutMeetingText.text = "Brak spotkañ";
    }

    private void DeleteMeetings() {
        int startDay = GetMonthStartDay(currDate.Year,currDate.Month);
        foreach(var meeting in meets) {
            if(meeting.Year == currDate.Year && meeting.Month == currDate.Month && meeting.Day == dayToCalculation - startDay) {
                meets.Remove(meeting);
                UpdateCalendar(currDate.Year, currDate.Month);
                break;
            }
        }
        ShowMeeting();
    }

    private int GetMonthStartDay(int year,int month) {
        return (int)(new DateTime(year,month,1)).DayOfWeek;
    }

    public static class ColorUtility
    {
        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1,hex.Length - 1);
            }

            if (hex.Length != 6)
            {
                throw new ArgumentException("Invalid hex color length. Expected 6 characters.");
            }

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, 255);
        }
    }
}