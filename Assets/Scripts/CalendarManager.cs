using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The CalendarManager creates a GridLayoutGroup of Calendar Days for the user to click on and interact.
/// </summary>
public class CalendarManager : MonoBehaviour
{
    public GameObject calendarDayPrefab;
    public GridLayoutGroup calendarGrid;
    public TextMeshProUGUI monthYearText;
    private DateTime currentDate;
    private DateTime actualDate;
    private DateTime firstDayOfMonth;

    private void Start() {
        currentDate = DateTime.Now;
        actualDate = DateTime.Now;
        firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        monthYearText.text = $"{currentDate.ToString("MMMM yyyy")}";
        InitializeCalendar();
    }

    private void InitializeCalendar() {

        monthYearText.text = $"{currentDate.ToString("MMMM yyyy")}";
        //Reset Buttons
        foreach(Transform calendaryDayButton in calendarGrid.transform) {
            Destroy(calendaryDayButton.gameObject);
        }

        int totalDays = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

        for(int i = firstDayOfMonth.Day; i <= totalDays; i++) {
            GameObject calendarDayButton = Instantiate(calendarDayPrefab, calendarGrid.transform);
            TextMeshProUGUI dayText = calendarDayButton.GetComponentInChildren<TextMeshProUGUI>();

            if(dayText != null) {
                dayText.text = $"{i}";
                Debug.Log($"Date Created: Day {firstDayOfMonth.ToString("dd MMMM yyyy")}");
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }
            else {
                Debug.LogError("Text Component not found");
            }

            /*if(i == actualDate.Day) {
                Image currentDayCheck = GameObject.FindGameObjectWithTag("CurrentDayCheck").GetComponent<Image>();
                currentDayCheck.enabled = true;
            }
            else {
                Image currentDayCheck = GameObject.FindGameObjectWithTag("CurrentDayCheck").GetComponent<Image>();
                currentDayCheck.enabled = false;
            }*/

            calendarDayButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnDayClick(firstDayOfMonth, int.Parse(dayText.text))); 
        }
        
        firstDayOfMonth = firstDayOfMonth.AddDays(-totalDays);
    }

    public void NextMonth() {
        currentDate = currentDate.AddMonths(1);
        firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        InitializeCalendar();
    }

    public void PreviousMonth() {
        currentDate = currentDate.AddMonths(-1);
        firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        InitializeCalendar();
    }

    public void ShowWindow() {
        this.gameObject.SetActive(true);
    }

    public void HideWindow() {
        this.gameObject.SetActive(false);
    }

    private void OnDayClick(DateTime selectedDay, int dayNumber) {
        selectedDay = selectedDay.AddDays(dayNumber-1);
        Debug.Log($"Day {selectedDay} Clicked!");
        HideWindow();
        GUIManager.instance.dayOverviewManager.LoadAndDisplayHistory(selectedDay);
    }
}
