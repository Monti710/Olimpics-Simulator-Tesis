using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("AudioSource para Música/Volumen General")]
    [SerializeField] private AudioSource musicAudioSource;

    [Tooltip("AudioSource para Efectos de Sonido 1")]
    [SerializeField] private AudioSource soundEffectAudioSource1;

    [Tooltip("AudioSource para Efectos de Sonido 2")]
    [SerializeField] private AudioSource soundEffectAudioSource2;

    [Tooltip("AudioSource para Efectos de Sonido 2")]
    [SerializeField] private AudioSource soundEffectAudioSource3;

    [Header("UI Sliders")]
    [Tooltip("Slider para controlar el volumen de la Música")]
    [SerializeField] private Slider volumeSlider;

    [Tooltip("Slider para controlar el volumen de los Efectos de Sonido")]
    [SerializeField] private Slider soundSlider;

    private const string VolumeKey = "Volume";
    private const string SoundKey = "Sound";

    [Header("Opciones")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float defaultSound = 0.5f;

    private void Awake()
    {
        // 1) Cargar volúmenes guardados (o usar defaults)
        float musicVol = PlayerPrefs.GetFloat(VolumeKey, defaultVolume);
        float soundVol = PlayerPrefs.GetFloat(SoundKey, defaultSound);

        // 2) Aplicarlos a los AudioSources correspondientes
        ApplyMusicVolume(musicVol);
        ApplySoundVolume(soundVol);

        // 3) Sincronizar los sliders (si existen), sin disparar callbacks
        ConfigureSlider(volumeSlider, musicVol, OnMusicSliderChanged);
        ConfigureSlider(soundSlider, soundVol, OnSoundSliderChanged);
    }

    private void ConfigureSlider(Slider slider, float initialValue, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.SetValueWithoutNotify(initialValue);
            slider.onValueChanged.AddListener(callback);
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (soundSlider != null)
            soundSlider.onValueChanged.RemoveListener(OnSoundSliderChanged);
    }

    private void OnMusicSliderChanged(float v)
    {
        // Aplicar volumen al AudioSource de Música
        ApplyMusicVolume(v);

        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, v);
        PlayerPrefs.Save();
    }

    private void OnSoundSliderChanged(float v)
    {
        // Aplicar volumen a los AudioSources de Efectos de Sonido
        ApplySoundVolume(v);

        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat(SoundKey, v);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float v)
    {
        if (musicAudioSource != null)
            musicAudioSource.volume = v;
    }

    private void ApplySoundVolume(float v)
    {
        if (soundEffectAudioSource1 != null)
            soundEffectAudioSource1.volume = v;

        if (soundEffectAudioSource2 != null)
            soundEffectAudioSource2.volume = v;
        
        if (soundEffectAudioSource3 != null)
            soundEffectAudioSource2.volume = v;
    }

    // (Opcional) Forzar a que se re-aplique desde PlayerPrefs en runtime.
    public void ApplyFromPrefs()
    {
        float musicVol = PlayerPrefs.GetFloat(VolumeKey, defaultVolume);
        float soundVol = PlayerPrefs.GetFloat(SoundKey, defaultSound);

        ApplyMusicVolume(musicVol);
        ApplySoundVolume(soundVol);

        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(musicVol);

        if (soundSlider != null)
            soundSlider.SetValueWithoutNotify(soundVol);
    }
}
