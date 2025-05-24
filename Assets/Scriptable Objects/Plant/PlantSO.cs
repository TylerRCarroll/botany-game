using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantSO", menuName = "Scriptable Objects/PlantSO")]
public class PlantSO : ScriptableObject
{
    public string name;
    public List<PlantStageType> Stages;
    
    public SunlightRequirement sunlightRequirement;
    public WaterRequirement waterRequirement;
    public MoistureRequirement moistureRequirement;
}