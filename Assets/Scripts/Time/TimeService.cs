using System;
using UnityEngine;


public class TimeService
{
    private readonly TimeSettings settings;
    private DateTime currentTime;
    
    private readonly TimeSpan sunriseTime;
    private readonly TimeSpan sunsetTime;
    
    readonly TimeSpan morningTime;
    readonly TimeSpan afternoonTime;
    readonly TimeSpan eveningTime;
    readonly TimeSpan nightTime;

    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action<int> OnHourChange = delegate { };
    public event Action<ETimeOfDay> OnTimeOfDayChange = delegate { };

    private readonly Observable<ETimeOfDay> currentTimeOfDay;
    private readonly Observable<bool> isDayTime;
    private readonly Observable<int> currentHour;

    public TimeService(TimeSettings settings)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.morningHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);
        
        morningTime = TimeSpan.FromHours(settings.morningHour);
        afternoonTime =  TimeSpan.FromHours(settings.afternoonHour);
        eveningTime =  TimeSpan.FromHours(settings.eveningHour);
        nightTime = TimeSpan.FromHours(settings.nightHour);
        
        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);
        currentTimeOfDay = new Observable<ETimeOfDay>(GetTimeOfDay());
        
        isDayTime.ValueChanged += isDay => (isDay ? OnSunrise : OnSunset)?.Invoke();
        currentHour.ValueChanged += hour => OnHourChange?.Invoke(hour);
        
        currentTimeOfDay.ValueChanged += timeOfDay => OnTimeOfDayChange?.Invoke(timeOfDay);
    }

    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
    }

    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();

        float startDegree = isDay ? 0 : 180;
        
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);
        
        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;

        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }
    
    public DateTime CurrentTime => currentTime;
    
    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    ETimeOfDay GetTimeOfDay()
    {
        if (currentTime.TimeOfDay > morningTime && currentTime.TimeOfDay < afternoonTime)
        {
            return ETimeOfDay.Morning;
        }

        if (currentTime.TimeOfDay > afternoonTime && currentTime.TimeOfDay < eveningTime)
        {
            return ETimeOfDay.Afternoon;
        }

        if (currentTime.TimeOfDay > eveningTime && currentTime.TimeOfDay < nightTime)
        {
            return ETimeOfDay.Evening;
        }
        
        return ETimeOfDay.Night;
    }

    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
    
    
    
}
