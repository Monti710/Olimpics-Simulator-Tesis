using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioControllerInstruccions : MonoBehaviour
{
    [Header("Audio Source to play")]
    public AudioSource audioSource;

    [Header("UI Slider para controlar el audio")]
    public Slider slider;

    [Header("Texto que muestra el tiempo (ejemplo: 00:32 / 02:15)")]
    public Text tiempoTexto; // si usas TextMeshPro, cámbialo por TMP_Text

    private bool usuarioArrastrando = false;
    // Guardamos el estado de reproducción antes de arrastrar
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
            // 1. Si el usuario NO está arrastrando el slider:
            if (!usuarioArrastrando)
            {
                // El slider sigue la posición actual del audio
                slider.value = audioSource.time;
                
                // El texto muestra la posición actual del audio
                ActualizarTiempoTexto(audioSource.time, audioSource.clip.length);
            }
            // 2. Si el usuario SÍ está arrastrando (usuarioArrastrando es true):
            else
            {
                // El slider NO es forzado por audioSource.time, permitiendo moverlo.
                
                // El texto muestra la posición que el slider está marcando
                ActualizarTiempoTexto(slider.value, audioSource.clip.length);
            }
        }
    }

    // --- Eventos del Slider ---

    // 1. Cuando el usuario hace click o toca el slider
    public void OnSliderPointerDown()
    {
        usuarioArrastrando = true; 
        
        // Guardar el estado de reproducción
        estabaReproduciendoAlArrastrar = audioSource.isPlaying;

        // **CLAVE:** Pausar la reproducción mientras arrastra para buscar
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    // 2. Cuando el usuario suelta el slider
    public void OnSliderPointerUp()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // **CLAVE:** Actualiza la posición del audio a donde se movió el slider
            audioSource.time = slider.value; 

            // Si estaba reproduciendo antes de arrastrar, o si el audio no ha terminado, reanuda.
            if (estabaReproduciendoAlArrastrar || audioSource.time < audioSource.clip.length)
            {
                audioSource.Play();
            }
        }
        usuarioArrastrando = false; // El slider vuelve a ser controlado por el audio
    }

    // ********* A partir de aquí el código es el mismo *********
    
    // --- Botones ---
    public void PlaySound()
    {
        if (audioSource == null) return;

        // Detener todos los audios
        foreach (AudioSource a in FindObjectsOfType<AudioSource>())
        {
            if (a.isPlaying) a.Stop();
        }

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

        foreach (AudioSource a in FindObjectsOfType<AudioSource>())
        {
            if (a.isPlaying) a.Stop();
        }

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