using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class SubtitleLine
{
    [TextArea(2, 5)]
    public string text;        // Texto del subtítulo
    public float duration;     // Cuánto tiempo permanece en pantalla
}

public class SubtitleManager : MonoBehaviour
{
    [Header("🎧 Audio y Texto")]
    public AudioSource audioSource;     // Asigna tu AudioSource (opcional)
    public TMP_Text subtitleText;       // Asigna tu TextMeshPro 3D

    [Header("🎬 Subtítulos (duración definida por el usuario)")]
    public SubtitleLine[] subtitles = new SubtitleLine[]
    {
        new SubtitleLine(){ text = "Bienvenido a este sistema de adiestramiento de armas.", duration = 3.5f },
        new SubtitleLine(){ text = "Aquí podrás entrenar para mejorar tu tiro.", duration = 3f },
        new SubtitleLine(){ text = "En frente tuyo hay un arma.", duration = 2f },
        new SubtitleLine(){ text = "Tu objetivo es disparar a la diana que está en tu dirección.", duration = 3f },
        new SubtitleLine(){ text = "Mientras más al centro le des, más puntaje vas a obtener.", duration = 4f },
        new SubtitleLine(){ text = "¡Vamos! Es hora de comenzar.", duration = 2.5f }
    };

    [Header("✨ Efectos")]
    public float fadeDuration = 0.4f;   // Tiempo de fade in/out
    public bool typingEnabled = false;  // Efecto escritura letra a letra
    public float typingSpeed = 0.03f;   // Tiempo entre letras

    private Coroutine showCoroutine;

    void Start()
    {
        if (subtitleText == null)
        {
            Debug.LogError("[SubtitleManager] No se asignó un TextMeshPro en el Inspector.");
            return;
        }

        subtitleText.text = "";
        subtitleText.alpha = 0f;

        // Reproduce el audio si está asignado
        if (audioSource != null)
            audioSource.Play();

        // Arranca los subtítulos
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        showCoroutine = StartCoroutine(ShowSubtitles());
    }

    IEnumerator ShowSubtitles()
    {
        foreach (SubtitleLine line in subtitles)
        {
            // Mostrar con fade-in + typing
            yield return StartCoroutine(FadeInAndShow(line.text));

            // Mantener visible por la duración indicada
            yield return new WaitForSeconds(line.duration);

            // Fade out antes del siguiente
            yield return StartCoroutine(FadeOut());
        }

        // Limpiar al final
        subtitleText.text = "";
        subtitleText.alpha = 0f;
    }

    IEnumerator FadeInAndShow(string newText)
    {
        // Fade-in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            subtitleText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        subtitleText.alpha = 1f;

        // Texto normal o letra por letra
        if (typingEnabled)
        {
            subtitleText.text = "";
            foreach (char c in newText)
            {
                subtitleText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else
        {
            subtitleText.text = newText;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            subtitleText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        subtitleText.alpha = 0f;
        subtitleText.text = "";
    }
}
