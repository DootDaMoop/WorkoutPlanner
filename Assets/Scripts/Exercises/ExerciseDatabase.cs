using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Exercise Database stores Exercises into respective .json file to load and save when needed.
/// </summary>
public static class ExerciseDatabase
{
    // Variables
    private static string pathToFiles = $"{Application.dataPath}/WorkoutData/ExerciseData.json";

    public static List<Exercise> LoadExercises() {
        if(File.Exists(pathToFiles)) {
            string json = File.ReadAllText(pathToFiles);
            SerializableExerciseList serializedExercises = JsonUtility.FromJson<SerializableExerciseList>(json);
            
            if(serializedExercises != null) {
                //Debug.Log("Exercise data loaded successfully");
                return serializedExercises.exercises;
            }
            else {
                //Debug.LogError("Failed to deserialize exercise data");
                return new List<Exercise>();
            } 
        }
        else {
            return new List<Exercise>();
        }
    }

    public static void SaveExercises(List<Exercise> exercises) {
        string json = JsonUtility.ToJson(new SerializableExerciseList(exercises));
        File.WriteAllText(pathToFiles, json);
        //Debug.Log("Exercise data successfully saved");
    }

    public static Exercise GetExercise(int index) {
        List<Exercise> exercises = LoadExercises();

        if (index >= 0 && index < exercises.Count) {
            return exercises[index];
        }
        else {
            //Debug.LogWarning($"Exercise not found at index {index}");
            return null;
        }
    }

    [System.Serializable]
    private class SerializableExerciseList {
        public List<Exercise> exercises;

        public SerializableExerciseList(List<Exercise> exercises) {
            this.exercises = exercises;
        }
    }
}