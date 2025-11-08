using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ControlBrilloYOscuridadURPModern : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public Light luzPrincipal;

    [Header("UI Sliders (Opcional, solo para Sync)")]
    [SerializeField] private List<Slider> brightnessSliders = new List<Slider>();

    [Header("Valores por Defecto")]
    [Range(-1f, 1f)]
    [SerializeField] private float defaultBrightness = 0f;

    private const string BrightnessKey = "Brightness";

    private Color skyOriginal, equatorOriginal, groundOriginal;
    private float intensidadOriginalLuz;
    private bool usaGradient;

    public float CurrentBrightness { get; private set; }

    void Start()
    {
        CacheBrightnessOriginals();

        // NO nos suscribimos a los sliders aquí.
        // La UI (GraphicsSettingsView) nos controlará.

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Cargar el valor guardado de PlayerPrefs al iniciar.
        float brightnessVal = PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness);

        SetValue(brightnessVal);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- FUNCIÓN DE PREVIEW (Llamada por GraphicsSettingsView) ---

    /// <summary>
    /// Aplica un cambio de brillo en tiempo real.
    /// </summary>
    public void PreviewBrightness(float previewValue)
    {
        ApplyBrightness(previewValue);
        SyncSliders(brightnessSliders, previewValue);
    }

    // --- FUNCIÓN DE CONTROL (Llamada por GraphicsSettingsView) ---

    /// <summary>
    /// Método PÚBLICO para forzar a este control a mostrar un valor (Reset).
    /// </summary>
    public void SetValue(float brightnessValue)
    {
        CurrentBrightness = brightnessValue;
        ApplyBrightness(brightnessValue);
        SyncSliders(brightnessSliders, brightnessValue);
    }

    // --- Métodos Internos ---

    private void ApplyBrightness(float valor)
    {
        float factor = 1f + valor;
        if (!usaGradient)
            RenderSettings.ambientIntensity = Mathf.Clamp(factor, 0f, 3f);
        else
        {
            RenderSettings.ambientSkyColor = skyOriginal * factor;
            RenderSettings.ambientEquatorColor = equatorOriginal * factor;
            RenderSettings.ambientGroundColor = groundOriginal * factor;
        }
        if (luzPrincipal != null)
            luzPrincipal.intensity = intensidadOriginalLuz * factor;
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
        float valueToApply = (SettingsManager.Instance != null) ?
                              SettingsManager.Instance.CurrentBrightness :
                              CurrentBrightness;
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
    }
}