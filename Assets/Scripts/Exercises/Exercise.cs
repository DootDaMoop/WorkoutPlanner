using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Exercise class is to make an Exercise object for the Workout class to utilize. 
/// </summary>

[Serializable]
public class Exercise 
{
    public string name;
    public List<string> muscleGroups;
    public List<string> equipment;
}
