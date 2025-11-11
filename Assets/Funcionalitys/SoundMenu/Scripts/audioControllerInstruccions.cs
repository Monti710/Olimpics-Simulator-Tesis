using UnityEngine;
using UnityEngine.UI;

public class AudioControllerInstruccions : MonoBehaviour
{
    [Header("Audio Source to play")]
    public AudioSource audioSource;

    [Header("UI Slider para controlar el audio")]
    public Slider slider;

    [Header("Texto que muestra el tiempo (ejemplo: 00:32 / 02:15)")]
    public Text tiempoTexto; // si usas TextMeshPro, cámbialo por TMP_Text

    private bool usuarioArrastrando = false;
    private bool estabaReproduciendoAlArrastrar = false;

    void Start()
    {
        if (slider != null)
        {
            slider.minValue = 0f;

            if (audioSource != null && audioSource.clip != null)
                slider.maxValue = audioSource.clip.length;
        }

        ActualizarTiempoTexto(0f, audioSource.clip != null ? audioSource.clip.length : 0f);
    }

    void Update()
    {
        if (audioSource != null && audioSource.clip != null && slider != null)
        {
            if (!usuarioArrastrando)
            {
                slider.value = audioSource.time;
                ActualizarTiempoTexto(audioSource.time, audioSource.clip.length);
            }
            else
            {
                ActualizarTiempoTexto(slider.value, audioSource.clip.length);
            }
        }
    }

    // --- Eventos del Slider ---

    public void OnSliderPointerDown()
    {
        usuarioArrastrando = true;
        estabaReproduciendoAlArrastrar = audioSource.isPlaying;

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void OnSliderPointerUp()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.time = slider.value;

            if (estabaReproduciendoAlArrastrar || audioSource.time < audioSource.clip.length)
            {
                audioSource.Play();
            }
        }
        usuarioArrastrando = false;
    }

    // --- Botones ---
    public void PlaySound()
    {
        if (audioSource == null) return;

        // Solo controla su propio AudioSource
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void PauseSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ReplaySound()
    {
        if (audioSource == null) return;

        // Reinicia solo su propio AudioSource
        audioSource.Stop();
        audioSource.Play();
    }

    // --- Función auxiliar para mostrar tiempo ---
    private void ActualizarTiempoTexto(float tiempoActual, float duracion)
    {
        if (tiempoTexto == null) return;

        string tiempoActualStr = FormatearTiempo(tiempoActual);
        string duracionStr = FormatearTiempo(duracion);

        tiempoTexto.text = $"{tiempoActualStr} / {duracionStr}";
    }

    private string FormatearTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60f);
        int segundos = Mathf.FloorToInt(tiempo % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}
