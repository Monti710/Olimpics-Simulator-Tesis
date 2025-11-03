using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;
    public TMP_InputField nameInput;

    [Tooltip("Objeto que se desactiva al guardar el nombre")]
    public GameObject objectToDeactivate1;
    public GameObject objectToDeactivate2;

    [Tooltip("Objeto que se activa al guardar el nombre")]
    public GameObject objectToActivate;

    [Tooltip("Objeto que se muestra cuando el input está vacío")]
    public GameObject warningObject;

    [Tooltip("Duración del aviso en segundos")]
    public float warningDuration = 2f;

    private int finalScore;

    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Puntos: " + finalScore;
        Debug.Log("Ruta del archivo de puntajes: " + Application.persistentDataPath);
    }

    public void SaveNameAndUpdateScore()
    {
        string playerName = nameInput.text.Trim();

        // Si el campo está vacío, mostrar advertencia y salir del método
        if (string.IsNullOrEmpty(playerName))
        {
            if (warningObject != null)
            {
                StartCoroutine(ShowWarning());
            }
            Debug.LogWarning("Debe escribir un nombre antes de continuar.");
            return;
        }

        // Cargar la lista de puntajes
        ScoreList list = LocalScoreManager.LoadScores();

        // Si no hay puntajes guardados, crear el primero
        if (list == null || list.scores.Count == 0)
        {
            ScoreData newScore = new ScoreData
            {
                playerName = playerName,
                score = finalScore,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            LocalScoreManager.SaveScore(newScore);
        }
        else
        {
            // Buscar si ya existe un puntaje igual al actual
            ScoreData existing = list.scores.Find(s => s.score == finalScore);

            if (existing != null)
            {
                existing.playerName = playerName;
                LocalScoreManager.OverwriteScores(list);
            }
            else
            {
                ScoreData newScore = new ScoreData
                {
                    playerName = playerName,
                    score = finalScore,
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                };
                LocalScoreManager.SaveScore(newScore);
            }
        }

        // Activar/desactivar objetos
        if (objectToDeactivate1 != null) objectToDeactivate1.SetActive(false);
        if (objectToDeactivate2 != null) objectToDeactivate2.SetActive(false);
        if (objectToActivate != null) objectToActivate.SetActive(true);

        Debug.Log("Puntaje guardado correctamente en JSON.");
    }

    private IEnumerator ShowWarning()
    {
        warningObject.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningObject.SetActive(false);
    }
}
