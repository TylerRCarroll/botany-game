using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI timeText;
   [SerializeField] TimeSettings timeSettings;
   [SerializeField] AnimationCurve LightIntensityCurve;
   [SerializeField] private Light Sun;
   [SerializeField]  private Light Moon;
   
   [SerializeField] float maxSunIntensity = 1;
   [SerializeField] float maxMoonIntensity = .5f;

   [SerializeField] private Color dayAmbientLight;
   [SerializeField] private Color nightAmbientLight;
   [SerializeField] private Volume volume;
   [SerializeField] private Material SkyboxMaterial;
   
   ColorAdjustments colorAdjustments;
   
   TimeService timeService;

   void Start()
   {
      timeService = new TimeService(timeSettings);
      Debug.Log(timeService);

      volume.profile.TryGet(out colorAdjustments);

   }

   void Update()
   {
      UpdateTimeOfDay();
      RotateSun();
      UpdateLightSettings();
      UpdateSkyBlend();
   }

   void UpdateSkyBlend()
   {
      float dotProduct = Vector3.Dot(Sun.transform.forward, Vector3.up);
      float blend = Mathf.Lerp(0,1,LightIntensityCurve.Evaluate(dotProduct));
      SkyboxMaterial.SetFloat("_Blend", blend);
   }

   void UpdateLightSettings()
   {
      float dotProduct = Vector3.Dot(Sun.transform.forward, Vector3.down);
      Sun.intensity = Mathf.Lerp(0, maxSunIntensity, LightIntensityCurve.Evaluate(dotProduct));
      
      Moon.bounceIntensity = Mathf.Lerp(maxMoonIntensity, 0,  LightIntensityCurve.Evaluate(dotProduct));
      if (colorAdjustments == null) return;
      
      colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, LightIntensityCurve.Evaluate(dotProduct));
   }

   void RotateSun()
   {
      float rotation = timeService.CalculateSunAngle();
      Sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
      
   }

   void UpdateTimeOfDay()
   {
      timeService.UpdateTime(Time.deltaTime);
      if (timeText != null)
      {
         timeText.text = timeService.CurrentTime.ToString("hh:mm");
      }
   }

}
