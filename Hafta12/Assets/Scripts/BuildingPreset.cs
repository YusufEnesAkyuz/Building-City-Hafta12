using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Building Preset", menuName = "New Building Preset")]
public class BuildingPreset : ScriptableObject   //pbjenin scriptle desteklenmesi için 
{
    public int cost;
    public int costPerTurn;
    public GameObject prefab;

    public int population;
    public int jobs;
    public int food;
    [Header("Advanced city stats")]
    [Tooltip("How much power this building adds to the city's grid each turn.")]
    public int powerProduction;
    [Tooltip("How much power this building consumes each turn.")]
    public int powerConsumption;
    [Tooltip("Positive numbers increase overall happiness, negative numbers decrease it.")]
    public int happiness;
    [Tooltip("How much pollution this building generates every turn.")]
    public int pollution;

}

