using UnityEngine;
using UnityEngine.UI;

public class PlayHighlightAudio : MonoBehaviour
{
    [Header("Configuración del audio")]
    public AudioSource backgroundAudio;   // Audio de fondo
    public AudioSource playAudio;         // Audio que resaltará
    public Button playButton;             // Botón que dispara el audio

    [Header("Ajustes de énfasis")]
    [Range(0f, 1f)] public float normalBackgroundVolume = 0.4f;
    [Range(0f, 1f)] public float emphasizedBackgroundVolume = 0.2f;
    [Range(0f, 1f)] public float playAudioVolume = 1f;
    public float emphasisDuration = 2f;   // Duración en segundos del efecto de énfasis

    private bool isEmphasizing = false;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonPressed);
    }

    void OnPlayButtonPressed()
    {
        if (playAudio != null && !isEmphasizing)
            StartCoroutine(EmphasizePlayAudio());
    }

    private System.Collections.IEnumerator EmphasizePlayAudio()
    {
        isEmphasizing = true;

        // Baja un poco el fondo para que el nuevo audio resalte
        if (backgroundAudio != null)
            backgroundAudio.volume = emphasizedBackgroundVolume;

        // Reproduce el audio de énfasis
        playAudio.volume = playAudioVolume;
        playAudio.priority = 0; // Prioridad más alta posible
        playAudio.Play();

        // Espera a que termine el audio
        yield return new WaitForSeconds(playAudio.clip.length + 0.1f);

        // Restaura el volumen de fondo
        if (backgroundAudio != null)
            backgroundAudio.volume = normalBackgroundVolume;

        isEmphasizing = false;
    }
}
