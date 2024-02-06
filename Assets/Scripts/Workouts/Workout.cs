using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Workout Class is an object that takes the value of a selected Exercise and creates a Workout Object
/// with user parameters. These parameters are made within Workout Editor
/// </summary>
[Serializable]
public class Workout
{
    public string name;
    public float weight;
    public int sets;
    public int reps;
    public List<string> equipment;
    public List<string> muscleGroups;

    // Not using DateTime since I had this setup for a while.
    public int dateIdentifier;
    

    public Workout() {
        equipment = new List<string>();
        muscleGroups = new List<string>();
    }

    public Workout(string name, float weight, int sets, int reps, List<string> equipment, List<string> muscleGroups) {
        this.name = name;
        this.weight = weight;
        this.sets = sets;
        this.reps = reps;
        
        this.equipment = equipment ?? new List<string>();
        this.muscleGroups = muscleGroups ?? new List<string>();
    }
}
