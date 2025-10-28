using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Sub : MonoBehaviour
{
    public AudioSource audioSource;
    public TMP_Text subtitleText;
    public float fadeDuration = 1f;
    public float typingSpeed = 0.05f;
    public float delayBetweenChunks = 1.30f;

    public float distanceFromCamera = 2f;
    public float heightOffset = -0.2f;

    private Transform cameraTransform;
    private bool isXRSetup = false;

    [TextArea(2, 5)]
    public List<string> subtitleChunks = new List<string>();

    private Coroutine currentSubtitlesCoroutine;
    private bool isPlaying = false;

    void Start()
    {
        subtitleText.text = "";
        subtitleText.alpha = 0f;

        cameraTransform = Camera.main.transform;
        isXRSetup = cameraTransform.parent != null && cameraTransform.parent.name.Contains("XR");

        // Reproducir automáticamente al inicio
        PlaySubtitlesAndAudio();
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition;
            Quaternion targetRotation;

            if (isXRSetup)
            {
                targetPosition = cameraTransform.position +
                                 cameraTransform.forward * distanceFromCamera +
                                 cameraTransform.up * heightOffset;

                targetRotation = Quaternion.LookRotation(cameraTransform.forward);
                targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
            }
            else
            {
                targetPosition = cameraTransform.position +
                                 cameraTransform.forward * distanceFromCamera;
                targetPosition.y += heightOffset;
                targetRotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            subtitleText.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        subtitleText.alpha = 1f;
    }

    IEnumerator PlaySubtitles()
    {
        isPlaying = true;

        // Fade in inicial
        yield return StartCoroutine(FadeIn());

        for (int i = 0; i < subtitleChunks.Count; i++)
        {
            string chunk = subtitleChunks[i];
            subtitleText.text = "";  // Limpia antes de escribir cada fragmento

            foreach (char letter in chunk)
            {
                subtitleText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(delayBetweenChunks);
        }

        yield return StartCoroutine(FadeOut());
        isPlaying = false;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            subtitleText.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        subtitleText.alpha = 0f;
        subtitleText.text = "";
    }

    public void PlaySubtitlesAndAudio()
    {
        // Detener corutina activa si hay una en progreso
        if (currentSubtitlesCoroutine != null)
        {
            StopCoroutine(currentSubtitlesCoroutine);
            subtitleText.alpha = 0f;
            subtitleText.text = "";
            isPlaying = false;
        }

        // Mutear o detener cualquier audio que esté sonando en la escena antes de reproducir el nuevo
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in allAudioSources)
        {
            if (src != audioSource && src.isPlaying)
            {
                src.Stop(); // O puedes usar src.volume = 0f; para solo mutear sin detener
            }
        }

        // Reiniciar audio del audioSource de este script
        audioSource.Stop();
        audioSource.Play();

        // Iniciar nueva corutina
        currentSubtitlesCoroutine = StartCoroutine(PlaySubtitles());
    }

    // Método para el botón de repetir
    public void RepeatSubtitles()
    {
        if (isPlaying)
        {
            // Si ya está reproduciendo, detener y reiniciar
            PlaySubtitlesAndAudio();
        }
        else
        {
            // Si no está reproduciendo, simplemente iniciar
            PlaySubtitlesAndAudio();
        }
    }
}
