using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorkoutEntry : MonoBehaviour
{
    public TMP_Text workoutDetailsText;
    public Button deleteButton;

    public void SetWorkoutDetails(string workoutDetails) {
        workoutDetailsText.text = workoutDetails;
    }

    public void SetDeletebuttonCallback(UnityAction callback) {
        deleteButton.onClick.AddListener(callback);
    }
}
