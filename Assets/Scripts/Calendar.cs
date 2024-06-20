using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Data;
/// TO DO: 1.Add button to come back to current month

public class Calendar : MonoBehaviour
{
    [SerializeField] public GameObject AddingMeetingsPanel;
    [SerializeField] public TMP_Text DataAtThePanel; 
    [SerializeField] public Button AddingMeetingsButton; 
    [SerializeField] public Button DeleteMeetingsButton; 
    [SerializeField] public TMP_InputField HourOfMeetingText;
    [SerializeField] public TMP_Text WholeAboutMeetingText;

    /// <summary>
    /// All the days in the month. After we make our first calendar we store these days in this list so we do not have to recreate them every time.
    /// </summary>
    private List<Day> days = new List<Day>();


    private List<Meeting> meets = new List<Meeting>();

    /// <summary>
    /// Setup in editor since there will always be six weeks. 
    /// Try to figure out why it must be six weeks even though at most there are only 31 days in a month
    /// </summary>
    public Transform[] weeks;


    /// <summary>
    /// This is the text object that displays the current month and year
    /// </summary>
    public TMP_Text MonthAndYear;


    /// <summary>
    /// this currDate is the date our Calendar is currently on. The year and month are based on the calendar, 
    /// while the day itself is almost always just 1
    /// If you have some option to select a day in the calendar, you would want the change this objects day value to the last selected day
    /// </summary>
    public DateTime currDate = DateTime.Now;

    public int dayTocalculation = 0;
    /// <summary>
    /// Cell or slot in the calendar. All the information each day should now about itself
    /// </summary>
    public class Day
    {
        public int dayNum;
        public Color dayColor;
        public GameObject obj;
        
        /// <summary>
        /// Constructor of Day
        /// </summary>
        public Day(int dayNum, Color dayColor, GameObject obj)
        {
            this.dayNum = dayNum;
            this.obj = obj;
            UpdateColor(dayColor);
            UpdateDay(dayNum);
        }

        /// <summary>
        /// Call this when updating the color so that both the dayColor is updated, as well as the visual color on the screen
        /// </summary>
        public void UpdateColor(Color newColor)
        {
            obj.GetComponent<Image>().color = newColor;
            dayColor = newColor;
        }

        /// <summary>
        /// When updating the day we decide whether we should show the dayNum based on the color of the day
        /// This means the color should always be updated before the day is updated
        /// </summary>
        public void UpdateDay(int newDayNum)
        {
            this.dayNum = newDayNum;
            if (dayColor == Color.white || dayColor == Color.green)
            {
                obj.GetComponentInChildren<TMP_Text>().text = (dayNum + 1).ToString();
                
            }
            else
            {
                obj.GetComponentInChildren<TMP_Text>().text = "";
            }
        }
        
    }

    public class Meeting
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        
        public int PersonID=1;
        public bool Meet = false;

        public Meeting(int year, int month, int day, int hour,  int personID, bool meet)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            
            PersonID = personID ;
            Meet = meet;
        }
    }

   

    /// <summary>
    /// In start we set the Calendar to the current date
    /// </summary>
    private void Start()
    {
        UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
        AddingMeetingsPanel.SetActive(false);
        AddingMeetingsButton.onClick.AddListener(AddingMeetings);
        DeleteMeetingsButton.onClick.AddListener(deleteMeetings);
    }

    /// <summary>
    /// Anytime the Calendar is changed we call this to make sure we have the right days for the right month/year
    /// </summary>
     

    public void ClickingOnDayToSetUpMeetings(int day)
    {
        dayTocalculation = day;
        int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
        AddingMeetingsPanel.gameObject.SetActive(true);
        DataAtThePanel.text = (day - startDay+1).ToString() + " " + MonthAndYear.text;
        ShowMeeting();
    }
    void UpdateCalendar(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        currDate = temp;
        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();
        int startDay = GetMonthStartDay(year, month);
        int endDay = GetTotalNumberOfDays(year, month);

        ///Create the days
        ///This only happens for our first Update Calendar when we have no Day objects therefore we must create them
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

        ///loop through days
        ///Since we already have the days objects, we can just update them rather than creating new ones
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

        ///This just checks if today is on our calendar. If so, we highlight it in green
        if (DateTime.Now.Year == year && DateTime.Now.Month == month)
        {
            days[(DateTime.Now.Day - 1) + startDay].UpdateColor(Color.green);
        }
        

    }
    public  void AddingMeetings()
    {
        
        int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
        string temp = HourOfMeetingText.text;
        int hour = int.Parse(temp);
        Meeting meeting;
        meeting = new Meeting(currDate.Year, currDate.Month, dayTocalculation - startDay, hour, 1, true);
        AddingMeetingsPanel.gameObject.SetActive(false);
        meets.Add(meeting);
        UpdateCalendar(currDate.Year, currDate.Month);
    }
    public void ShowMeeting()
    {
        int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
        if (meets.Count == 0)
        {
            WholeAboutMeetingText.text = ("Brak spotkan");

        }
        else {
            foreach (var meeting in meets)
            {
                if (meeting.Year == currDate.Year && meeting.Month == currDate.Month && meeting.Day == dayTocalculation - startDay)
                {
                    if (meeting.Month <= 9)
                    {
                        WholeAboutMeetingText.text = meeting.Hour.ToString() + ":00 " + meeting.Day.ToString() + ".0" + meeting.Month.ToString() + "." + meeting.Year.ToString();
                    }
                    else
                    {
                        WholeAboutMeetingText.text = meeting.Hour.ToString() + ":00 " + meeting.Day.ToString() + "." + meeting.Month.ToString() + "." + meeting.Year.ToString();
                    }
                }
                else
                {
                    WholeAboutMeetingText.text = ("Brak spotkan");
                }

            }
        }
        
        
    }

    public void deleteMeetings()
    {
        int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
        foreach (var meeting in meets)
        {
            if (meeting.Year == currDate.Year && meeting.Month == currDate.Month && meeting.Day == dayTocalculation - startDay)
            {
                meets.Remove(meeting);
                UpdateCalendar(currDate.Year, currDate.Month);
                break;
            }
        }
        
        ShowMeeting();
        
    }
    /// <summary>
    /// This returns which day of the week the month is starting on
    /// </summary>
    int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);

        //DayOfWeek Sunday == 0, Saturday == 6 etc.
        return (int)temp.DayOfWeek;
    }

    /// <summary>
    /// Gets the number of days in the given month.
    /// </summary>
    int GetTotalNumberOfDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    /// <summary>
    /// This either adds or subtracts one month from our currDate.
    /// The arrows will use this function to switch to past or future months
    /// </summary>
    public void SwitchMonth(int direction)
    {
        if (direction < 0)
        {
            currDate = currDate.AddMonths(-1);
            int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
           
        }
        else
        {
            currDate = currDate.AddMonths(1);
            int startDay = GetMonthStartDay(currDate.Year, currDate.Month);
           
        }

        UpdateCalendar(currDate.Year, currDate.Month);
    }
    
    

    
    public void CloseAddingMeetings()
    {
        AddingMeetingsPanel.gameObject.SetActive(false);
    }
}