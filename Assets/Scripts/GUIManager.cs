using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GUIManager class is used as a Singleton to reference all other scripts.
/// </summary>
public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;
    public CalendarManager calendarManager;
    public WorkoutEditor workoutEditor;
    public ExerciseEditor exerciseEditor;
    public DayOverviewManager dayOverviewManager;
    
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        calendarManager.ShowWindow();
    }
}
