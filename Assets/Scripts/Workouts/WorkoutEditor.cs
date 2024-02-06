using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// WorkoutEditor is used when the user is able to make Workouts using Inputs, Toggle Groups, and Muscle/Equipment Dropdowns.
/// Additionally, filtering and generation options are added to the WorkoutEditor.
/// </summary>
public class WorkoutEditor : MonoBehaviour
{
    public TMP_Dropdown exerciseDropdown;
    public TMP_InputField weightInput;
    public TMP_InputField setsInput;
    public TMP_InputField repsInput;
    public ToggleGroup muscleGroupFilterToggleGroup;
    public ToggleGroup equipmentGroupFilterToggleGroup;
    public Toggle togglePrefab; 
    public Button submitButton;
    private int currentDay;
    private DateTime currentDate;


    private void Start() {
        submitButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnSubmitClick());
        InitializeFilters();
    }

    public void ClearInputFields() {
        weightInput.text = "";
        setsInput.text = "";
        repsInput.text = "";
        exerciseDropdown.ClearOptions();

        foreach(Toggle toggle in muscleGroupFilterToggleGroup.GetComponentsInChildren<Toggle>()) {
            toggle.isOn = false;
        }
        foreach(Toggle toggle in equipmentGroupFilterToggleGroup.GetComponentsInChildren<Toggle>()) {
            toggle.isOn = false;
        }
    }

    public void SetDay(DateTime dateTime) {
        currentDay = int.Parse($"{dateTime.Day}{dateTime.Month}{dateTime.Year}");
        currentDate = dateTime;
        Debug.Log($"Editing Workouts for Day {dateTime}");
        //dayLabel.text = $"Editing Workouts for Day {day}";
        LoadWorkoutData();
        LoadExerciseDropdown();
    }

    #region Window Functions

    public void ShowWindow() {
        this.gameObject.SetActive(true);
    }

    public void HideWindow() {
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Saving/Loading Functions

    // Gets user parameters and returns a new created Workout
    public void SaveWorkout() {
        Exercise selectedExercise = GetSelectedFilterExercise();
        float weight = float.Parse(weightInput.text);
        int sets = int.Parse(setsInput.text);
        int reps = int.Parse(repsInput.text);

        Workout workout = new Workout {
            name = selectedExercise.name,
            weight = weight,
            sets = sets,
            reps = reps,
            equipment = new List<string>(selectedExercise.equipment),
            muscleGroups = new List<string>(selectedExercise.muscleGroups)
        };

        SaveWorkoutData(workout);
    }

    public void RemoveWorkout() {
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();
        Workout existingWorkout = workouts.Find(w => w.dateIdentifier == currentDay);

        if(existingWorkout != null) {
            workouts.Remove(existingWorkout);
            WorkoutDatabase.SaveWorkouts(workouts);
            Debug.Log($"Workout for Date: {currentDay} removed successfully");
        }
        else {
            Debug.LogWarning($"No workout found for Day {currentDay}");
        }
    }

    // Loads Workout data for the current selected Day
    private void LoadWorkoutData() {
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();
        List<Workout> workoutsForCurrentDay = workouts.FindAll(w => w.dateIdentifier == currentDay);

        if (workoutsForCurrentDay.Count > 0) {
            Workout existingWorkout = workoutsForCurrentDay[0];

            List<Exercise> exercises = ExerciseDatabase.LoadExercises();
            int exerciseIndex = exercises.FindIndex(ex => ex.name == existingWorkout.name);
            weightInput.text = existingWorkout.weight.ToString();
            setsInput.text = existingWorkout.sets.ToString();
            repsInput.text = existingWorkout.reps.ToString();

            if(exerciseIndex != -1) {
                exerciseDropdown.value = exerciseIndex;
            }
            else {
                Debug.LogWarning($"Exercise not found for workout on Day {currentDay}");
            }

            Debug.Log($"Workout data for Date: {currentDay} loaded successfully.");
        }
        else {
            Debug.LogWarning($"No workouts found for Date: {currentDay}. Initializing with default values.");
        }

    }

    // Saves Workout data for the current selected Day.
    private void SaveWorkoutData(Workout workout) {
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();
        List<Workout> workoutsForCurrentDay = workouts.FindAll(w => w.dateIdentifier == currentDay);
        
        foreach (Workout existingWorkout in workoutsForCurrentDay) {
            if(existingWorkout.name == workout.name) {
                existingWorkout.name = workout.name;
                existingWorkout.weight = workout.weight;
                existingWorkout.sets = workout.sets;
                existingWorkout.reps = workout.reps;
                existingWorkout.equipment = workout.equipment;
                existingWorkout.muscleGroups = workout.muscleGroups;
            }
        }

        if(!workoutsForCurrentDay.Exists(w => w.name == workout.name)) {
            workout.dateIdentifier = currentDay;
            workouts.Add(workout);
        }

        WorkoutDatabase.SaveWorkouts(workouts);
        Debug.Log($"Workout data for Date: {currentDay} saved successfully.");
    }

    public void LoadExerciseDropdown() {
        List<Exercise> exercises = ExerciseDatabase.LoadExercises();

        List<string> selectedMuscleGroups = GetSelectedFilters(muscleGroupFilterToggleGroup);
        List<string> selectedEquipment = GetSelectedFilters(equipmentGroupFilterToggleGroup);

        List<Exercise> filteredExercises = FilterExercises(exercises, selectedMuscleGroups, selectedEquipment);

        exerciseDropdown.ClearOptions();

        List<string> exerciseNames = new List<string>();
        foreach (Exercise exercise in filteredExercises) {
            exerciseNames.Add(exercise.name);
        }

        
        exerciseDropdown.AddOptions(exerciseNames);
        Debug.Log($"First value of Exercise Dropdown: {exerciseDropdown.value}");


    }

    #endregion

    #region Button (OnClick) Functions

    private void OnSubmitClick() {
        SaveWorkout();
        HideWindow();
        GUIManager.instance.dayOverviewManager.ShowWindow();
        GUIManager.instance.dayOverviewManager.LoadAndDisplayHistory(currentDate);
        ClearInputFields();
    }

    #endregion

    #region Filtering Functions
    
    private void InitializeFilterToggleGroup(ToggleGroup toggleGroup, List<string> options) {
        int i = 0;
        foreach(string option in options) { 
            Toggle filterToggle = Instantiate(togglePrefab, toggleGroup.transform);
            filterToggle.GetComponentInChildren<Text>().text = option;
            filterToggle.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,i,0);
            i -= 25;
        }
    }

    private void InitializeFilters() {
        List<string> muscleGroupOptions = new List<string> { "Chest", "Core", "Obliques", "Shoulders", "Biceps", "Triceps", "Forearms",
                                                             "Upper Trapezius (Upper Back)", "Lower Trapezius (Middle Back)", "Spinae Erector (Lower Back)",
                                                             "Laterals", "Glutes", "Quadriceps", "Hamstrings", "Calves"};

        List<string> equipmentOptions = new List<string> { "None (Calisthenics/Bodyweight)", "Dumbells", "Barbells", "Cables",
                                                           "Kettlebells", "Medicine Balls", "Resistence Bands", "Plates", "Machines" };

        InitializeFilterToggleGroup(muscleGroupFilterToggleGroup, muscleGroupOptions);
        InitializeFilterToggleGroup(equipmentGroupFilterToggleGroup, equipmentOptions); 
    }

    private List<string> GetSelectedFilters(ToggleGroup toggleGroup) {
        List<string> selectedFilters = new List<string>();

        foreach(Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>()) {
            if(toggle.isOn)
                selectedFilters.Add(toggle.GetComponentInChildren<Text>().text);
        }

        return selectedFilters;
    }

    private List<Exercise> FilterExercises(List<Exercise> exercises, List<string> selectedMuscleGroups, List<string> selectedEquipment) {
        List<Exercise> filterExercises = new List<Exercise>();

        foreach(Exercise exercise in exercises) {
            if(MatchesFilters(exercise, selectedMuscleGroups, selectedEquipment))
                filterExercises.Add(exercise);
        }

        return filterExercises;
    }

    private bool MatchesFilters(Exercise exercise, List<string> selectedMuscleGroups, List<string> selectedEquipment) {
        bool matchesMuscleGroups = selectedMuscleGroups.Count == 0 || selectedMuscleGroups.Any(exercise.muscleGroups.Contains);
        bool matchesEquipment = selectedEquipment.Count == 0 || selectedEquipment.Any(exercise.equipment.Contains);

        return matchesMuscleGroups && matchesEquipment;
    }

    private Exercise GetSelectedFilterExercise() {
        string selectedExerciseName = exerciseDropdown.options[exerciseDropdown.value].text;

        List<Exercise> filteredExercises = FilterExercises(ExerciseDatabase.LoadExercises(), 
                                                           GetSelectedFilters(muscleGroupFilterToggleGroup), 
                                                           GetSelectedFilters(equipmentGroupFilterToggleGroup));

        return filteredExercises.Find(e => e.name == selectedExerciseName);
    }

    #endregion

    #region Generation Functions

    public void GenerateRandomWorkouts() {
        List<string> selectedMuscleGroups = GetSelectedFilters(muscleGroupFilterToggleGroup);
        List<string> selectedEquipment = GetSelectedFilters(equipmentGroupFilterToggleGroup);

        List<Exercise> filteredExercises = FilterExercises(ExerciseDatabase.LoadExercises(), selectedMuscleGroups, selectedEquipment);

        // Clear existing workouts from current day
        ClearWorkoutData();

        // Generate random workouts and save them for the current day
        foreach (Exercise exercise in filteredExercises)
        {
            float randomWeight = Mathf.Ceil(Random.Range(45f, 100f));
            int randomSets = Random.Range(2, 4);
            int randomReps = Random.Range(6, 12);

            Workout workout = new Workout
            {
                name = exercise.name,
                weight = randomWeight,
                sets = randomSets,
                reps = randomReps,
                equipment = new List<string>(exercise.equipment),
                muscleGroups = new List<string>(exercise.muscleGroups)
            };

            SaveWorkoutData(workout);
        }

        // Reload Workout Day
        HideWindow();
        GUIManager.instance.dayOverviewManager.ShowWindow();
        GUIManager.instance.dayOverviewManager.LoadAndDisplayHistory(currentDate);
        ClearInputFields();
    }

    public void ClearWorkoutData() {
        List<Workout> workouts = WorkoutDatabase.LoadWorkouts();

        workouts.RemoveAll(w => w.dateIdentifier == GUIManager.instance.dayOverviewManager.numCurrentDay);
        WorkoutDatabase.SaveWorkouts(workouts);

        GUIManager.instance.dayOverviewManager.LoadAndDisplayHistory(GUIManager.instance.dayOverviewManager.calendarDayDateTime);
    }


    #endregion
}