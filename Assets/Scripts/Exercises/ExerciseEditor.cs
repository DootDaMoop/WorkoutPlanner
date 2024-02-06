using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ExerciseEditor is used when the user is able to make exercises using Input, Toggle Group, and Equipment Dropdowns.
/// </summary>
public class ExerciseEditor : MonoBehaviour
{   
    // Variables
    public TMP_InputField exerciseNameInput;
    public ToggleGroup muscleGroupToggleGroup;
    public TMP_Dropdown equipmentDropdown;
    public Toggle muscleGroupTogglePrefab;
    private List<string> equipment = new List<string>();
    
    private void Start() {
        LoadDropdownOptions();
    }

    private void ClearInputFields()
    {
        exerciseNameInput.text = "";
        equipment.Clear();

        foreach(Toggle toggle in muscleGroupToggleGroup.GetComponentsInChildren<Toggle>()) {
            toggle.isOn = false;
        }
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

    private void LoadDropdownOptions() {
        List<string> muscleGroupOptions = new List<string> { "Chest", "Core", "Obliques", "Shoulders", "Biceps", "Triceps", "Forearms",
                                                             "Upper Trapezius (Upper Back)", "Lower Trapezius (Middle Back)", "Spinae Erector (Lower Back)",
                                                             "Laterals", "Glutes", "Quadriceps", "Hamstrings", "Calves"};

        List<string> equipmentOptions = new List<string> { "None (Calisthenics/Bodyweight)", "Dumbells", "Barbells", "Cables",
                                                           "Kettlebells", "Medicine Balls", "Resistence Bands", "Plates", "Machines" };

        equipmentDropdown.AddOptions(equipmentOptions);

        // Edits spacing for each Toggle object
        float i = 0;
        foreach(string muscleGroup in muscleGroupOptions) {
            Toggle muscleGroupToggle = Instantiate(muscleGroupTogglePrefab, muscleGroupToggleGroup.transform);
            muscleGroupToggle.GetComponentInChildren<Text>().text = muscleGroup;
            muscleGroupToggle.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,i,0);
            i -= 25;
        }

    }

    public void SaveExercise() {
        string exerciseName = exerciseNameInput.text;
        string equipment = equipmentDropdown.options[equipmentDropdown.value].text;

        List<string> equipments = new List<string>();
        List<string> muscleGroups = GetSelectedMuscleGroups();
        
        equipments.Add(equipment);

        Exercise exercise = new Exercise {name = exerciseName, muscleGroups = muscleGroups, equipment = equipments };

        List<Exercise> exercises = ExerciseDatabase.LoadExercises();
        exercises.Add(exercise);
        ExerciseDatabase.SaveExercises(exercises);
        ClearInputFields();
        Debug.Log($"Exercise {exerciseName} saved succesfully");
    }

    private List<string> GetSelectedMuscleGroups() {
        List<string> selectedMuscleGroups = new List<string>();

        // Note: Unable to use muscleGroupToggleGroup.ActiveToggles()
        foreach (Toggle toggle in muscleGroupToggleGroup.GetComponentsInChildren<Toggle>()) {
            if(toggle.isOn)
                selectedMuscleGroups.Add(toggle.GetComponentInChildren<Text>().text);
        }

        return selectedMuscleGroups;
    }

    #endregion

}