using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The DayOverviewManager shows an overview of workouts (if any) for the selected date from
/// the CalendarManager
/// </summary>
public class DayOverviewManager : MonoBehaviour
{
    // Variables
    public int numCurrentDay;
    public TMP_Text currentDay;
    public TMP_Text dayOverviewText;
    public Button addWorkoutButton;
    public GridLayoutGroup dayOverviewGridLayout;
    public GameObject workoutEntryPrefab;
    public DateTime calendarDayDateTime;

    #region Window Functions

    public void ShowWindow() {
        this.gameObject.SetActive(true);
    }

    public void HideWindow() {
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Button/OnClick Functions

    public void OnAddWorkoutClick(DateTime dateTime) {
        HideWindow();
        GUIManager.instance.workoutEditor.SetDay(dateTime);
        GUIManager.instance.workoutEditor.ShowWindow();
    }

    private void OnDeleteWorkoutClick(Workout workout) {
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();
        workouts.RemoveAll(w => w.dateIdentifier == numCurrentDay && w.name == workout.name);
        WorkoutDatabase.SaveWorkouts(workouts);
        LoadAndDisplayHistory(calendarDayDateTime);
        Debug.Log($"Deleted {workout.name} on Day {numCurrentDay}");
        
    }

    #endregion
    
    public void LoadAndDisplayHistory(DateTime selectedDate) {
        numCurrentDay = int.Parse($"{selectedDate.Day}{selectedDate.Month}{selectedDate.Year}");
        calendarDayDateTime = selectedDate;
        ShowWindow();

        // Loads the workouts for the slected current date.
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();
        List<Workout> workoutsForCurrentDay = workouts.FindAll(w => w.dateIdentifier == numCurrentDay); 

        // Resets the Day Overview
        foreach(Transform child in dayOverviewGridLayout.transform) {
            Destroy(child.gameObject);
        }

        // Sets up Day Overview (Checks and Displays Workouts)
        currentDay.text = $"{calendarDayDateTime.ToString("dd/MM/yyyy")}";
        if(workoutsForCurrentDay.Count != 0) {
            dayOverviewText.text = "";

            foreach (Workout workout in workoutsForCurrentDay) {
                GameObject entry = Instantiate(workoutEntryPrefab, dayOverviewGridLayout.transform);
                WorkoutEntry entryScript = entry.GetComponent<WorkoutEntry>();

                string workoutDetails = $"{workout.name}: {workout.weight} lbs\nSets: {workout.sets} | Reps: {workout.reps}";
                entryScript.SetWorkoutDetails(workoutDetails);

                // Delete Button Callback
                entryScript.SetDeletebuttonCallback(() => OnDeleteWorkoutClick(workout));
            }

            if(workoutsForCurrentDay.Count > 6) {
                dayOverviewGridLayout.GetComponent<RectTransform>().anchoredPosition = new Vector3(-225,230,0);
                dayOverviewGridLayout.cellSize = new Vector2(0, 100);
                dayOverviewGridLayout.spacing = new Vector2(560,10);
            }
            else {
                dayOverviewGridLayout.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,230,0);
                dayOverviewGridLayout.cellSize = new Vector2(100, 100);
                dayOverviewGridLayout.spacing = new Vector2(0,10);
            }
        }
        else {
            dayOverviewText.text = "No Workouts";
        }

        addWorkoutButton.GetComponent<Button>().onClick.AddListener(() => OnAddWorkoutClick(calendarDayDateTime));
    }

}
