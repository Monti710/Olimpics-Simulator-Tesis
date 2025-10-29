using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TMP_InputField nameInput;

    [Tooltip("Objeto que se desactiva al guardar el nombre")]
    public GameObject objectToDeactivate1;
    public GameObject objectToDeactivate2;

    [Tooltip("Objeto que se activa al guardar el nombre")]
    public GameObject objectToActivate;

    private int finalScore;

    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Puntaje final: " + finalScore;
    }

    public void SaveNameAndUpdateScore()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName)) playerName = "Jugador";

        // Cargar lista actual de puntajes
        ScoreList list = LocalScoreManager.LoadScores();

        // Suponemos que el último puntaje agregado es el último de la lista
        if (list.scores.Count > 0)
        {
            ScoreData lastScore = list.scores[list.scores.Count - 1];
            if (lastScore.score == finalScore)
            {
                lastScore.playerName = playerName;
                // Guardar lista nuevamente
                LocalScoreManager.OverwriteScores(list);
            }
        }

        // En lugar de cambiar de escena, activar/desactivar objetos
        if (objectToDeactivate1 != null)
            objectToDeactivate1.SetActive(false);

        if (objectToDeactivate2 != null)
            objectToDeactivate2.SetActive(false);

        if (objectToActivate != null)
            objectToActivate.SetActive(true);
    }
}
