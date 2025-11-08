using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeControl : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private List<AudioSource> musicAudioSources = new List<AudioSource>();
    [SerializeField] private List<AudioSource> soundEffectAudioSources = new List<AudioSource>();

    [Header("UI Sliders (Opcional, solo para Sync)")]
    // Esta lista AHORA es solo para sincronizar sliders entre sí,
    // pero el control principal viene de AudioSettingsView.
    [SerializeField] private List<Slider> musicSliders = new List<Slider>();
    [SerializeField] private List<Slider> soundSliders = new List<Slider>();

    [Header("Valores por Defecto")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float defaultSound = 0.5f;

    private const string VolumeKey = "Volume";
    private const string SoundKey = "Sound";

    public float CurrentMusicVolume { get; private set; }
    public float CurrentSoundVolume { get; private set; }

    void Start()
    {
        // NO nos suscribimos a los sliders aquí.
        // La UI (AudioSettingsView) nos controlará.

        // Cargar el valor guardado de PlayerPrefs al iniciar.
        float musicVol = PlayerPrefs.GetFloat(VolumeKey, defaultVolume);
        float soundVol = PlayerPrefs.GetFloat(SoundKey, defaultSound);

        // Aplicar estos valores como estado inicial
        SetValues(musicVol, soundVol);
    }

    // --- FUNCIONES DE PREVIEW (Llamadas por AudioSettingsView) ---

    /// <summary>
    /// Aplica un cambio de volumen de música en tiempo real.
    /// </summary>
    public void PreviewMusicVolume(float previewValue)
    {
        ApplyMusicVolume(previewValue);
        SyncSliders(musicSliders, previewValue);
    }

    /// <summary>
    /// Aplica un cambio de volumen de sonido en tiempo real.
    /// </summary>
    public void PreviewSoundVolume(float previewValue)
    {
        ApplySoundVolume(previewValue);
        SyncSliders(soundSliders, previewValue);
    }

    // --- FUNCIÓN DE CONTROL (Llamada por AudioSettingsView) ---

    /// <summary>
    /// Método PÚBLICO para forzar a este control a mostrar un valor (Reset).
    /// </summary>
    public void SetValues(float musicVol, float soundVol)
    {
        CurrentMusicVolume = musicVol;
        CurrentSoundVolume = soundVol;

        ApplyMusicVolume(musicVol);
        ApplySoundVolume(soundVol);

        SyncSliders(musicSliders, musicVol);
        SyncSliders(soundSliders, soundVol);
    }

    // --- Métodos Internos ---
    private void ApplyMusicVolume(float v)
    {
        foreach (AudioSource source in musicAudioSources)
        {
            if (source != null) source.volume = v;
        }
    }

    private void ApplySoundVolume(float v)
    {
        foreach (AudioSource source in soundEffectAudioSources)
        {
            if (source != null) source.volume = v;
        }
    }

    private void SyncSliders(List<Slider> sliders, float value)
    {
        foreach (Slider s in sliders)
        {
            if (s != null)
            {
                s.SetValueWithoutNotify(value);
            }
        }
    }
}