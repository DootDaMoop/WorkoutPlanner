using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class WorkoutDatabase
{
    private static string pathToFiles = $"{Application.dataPath}/WorkoutData/WorkoutData.json";

    public static void SaveWorkouts(List<Workout> workouts) {
        string json = JsonUtility.ToJson(new SerializableWorkoutList(workouts));
        File.WriteAllText(pathToFiles, json);
        Debug.Log("Workout data successfully saved");
    }

    public static List<Workout> LoadWorkouts() {
        if(File.Exists(pathToFiles)) {
            string json = File.ReadAllText(pathToFiles);
            SerializableWorkoutList serializedWorkouts = JsonUtility.FromJson<SerializableWorkoutList>(json);
            
            if(serializedWorkouts != null) {
                Debug.Log("Workout data loaded successfully");
                return serializedWorkouts.workouts;
            }
            else {
                Debug.LogError("Failed to deserialize workout data");
                return new List<Workout>();
            } 
        }
        Debug.LogWarning("No workout data found, returning empty List");
        return new List<Workout>();
    }

    [System.Serializable]
    private class SerializableWorkoutList {
        public List<Workout> workouts;

        public SerializableWorkoutList(List<Workout> workouts) {
            this.workouts = workouts;
        }
    }
}
