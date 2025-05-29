using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "TimeSettings")]
public class TimeSettings : ScriptableObject
{
    public float timeMultiplier = 1000;
    public float sunriseHour = 5;
    public float sunsetHour = 17;
    
    public float morningHour = 6;
    public float afternoonHour = 10;
    public float eveningHour = 14;
    public float nightHour = 18;
}