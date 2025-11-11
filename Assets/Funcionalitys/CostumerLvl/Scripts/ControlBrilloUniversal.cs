using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ControlBrilloYOscuridadURPModern : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public Light luzPrincipal;
    public List<Light> spotLights = new List<Light>();

    [Header("UI Sliders (Opcional, solo para Sync)")]
    [SerializeField] private List<Slider> brightnessSliders = new List<Slider>();

    [Header("Valores por Defecto")]
    [Range(-2f, 2f)]
    [SerializeField] private float defaultBrightness = 0f;

    private const string BrightnessKey = "Brightness";

    private Color skyOriginal, equatorOriginal, groundOriginal;
    private float intensidadOriginalLuz;
    private List<float> intensidadesSpotOriginales = new List<float>();
    private bool usaGradient;

    public float CurrentBrightness { get; private set; }

    void Start()
    {
        CacheBrightnessOriginals();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ✅ Carga el valor del slider, no la intensidad calculada
        float savedValue = PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness);
        SetValue(savedValue);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PreviewBrightness(float previewValue)
    {
        ApplyBrightness(previewValue);
        SyncSliders(brightnessSliders, previewValue);
    }

    public void SetValue(float brightnessValue)
    {
        CurrentBrightness = brightnessValue;
        ApplyBrightness(brightnessValue);
        SyncSliders(brightnessSliders, brightnessValue);

        // ✅ Guardar el valor del slider directamente
        PlayerPrefs.SetFloat(BrightnessKey, brightnessValue);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float valor)
    {
        // factor controlado por slider (-2 a 2)
        float factor = Mathf.Clamp(1f + valor, 0.1f, 4f);

        if (!usaGradient)
        {
            RenderSettings.ambientIntensity = Mathf.Clamp(factor, 0f, 3f);
        }
        else
        {
            RenderSettings.ambientSkyColor = skyOriginal * factor;
            RenderSettings.ambientEquatorColor = equatorOriginal * factor;
            RenderSettings.ambientGroundColor = groundOriginal * factor;
        }

        if (luzPrincipal != null)
            luzPrincipal.intensity = intensidadOriginalLuz * factor;

        for (int i = 0; i < spotLights.Count; i++)
        {
            if (spotLights[i] != null)
                spotLights[i].intensity = intensidadesSpotOriginales[i] * factor;
        }
    }

    private void SyncSliders(List<Slider> sliders, float value)
    {
        foreach (Slider s in sliders)
        {
            if (s != null) s.SetValueWithoutNotify(value);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CacheBrightnessOriginals();
        float valueToApply = PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness);
        SetValue(valueToApply);
    }

    private void CacheBrightnessOriginals()
    {
        usaGradient = (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight);
        if (usaGradient)
        {
            skyOriginal = RenderSettings.ambientSkyColor;
            equatorOriginal = RenderSettings.ambientEquatorColor;
            groundOriginal = RenderSettings.ambientGroundColor;
        }

        if (luzPrincipal != null)
            intensidadOriginalLuz = luzPrincipal.intensity;

        intensidadesSpotOriginales.Clear();
        foreach (var s in spotLights)
        {
            intensidadesSpotOriginales.Add(s != null ? s.intensity : 1f);
        }
    }
}
