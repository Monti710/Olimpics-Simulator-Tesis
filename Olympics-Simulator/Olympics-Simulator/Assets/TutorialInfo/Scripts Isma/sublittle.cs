using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Sublittle : MonoBehaviour
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

    [TextArea(2, 5)]  // Hace más fácil editar texto en Inspector, aplica para cada string
    public List<string> subtitleChunks = new List<string>();

    void Start()
    {
        subtitleText.text = "";
        subtitleText.alpha = 0f;

        cameraTransform = Camera.main.transform;

        isXRSetup = cameraTransform.parent != null && cameraTransform.parent.name.Contains("XR");

        // NO iniciar audio ni subtítulos aquí
        // Esto evita interferencias al inicio
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
        foreach (string chunk in subtitleChunks)
        {
            subtitleText.text = "";
            foreach (char letter in chunk.ToCharArray())
            {
                subtitleText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(delayBetweenChunks);
        }

        StartCoroutine(FadeOut());
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
        if (audioSource.isPlaying)
            return; // Ya está reproduciendo, no hacer nada

        StopAllCoroutines(); // Por si estaba corriendo alguna coroutine
        subtitleText.alpha = 0f;
        subtitleText.text = "";
        audioSource.Play();
        StartCoroutine(FadeIn());
        StartCoroutine(PlaySubtitles());
    }
}
