using UnityEngine;
using TMPro; // Aseg�rate de tener este namespace

public class PointDisplay : MonoBehaviour
{
    public PointCounter pointCounter; // Referencia al script PointCounter
    public TextMeshProUGUI scoreText; // Referencia al TextMeshPro donde se mostrar� el puntaje

    void Start()
    {
        // Aseg�rate de que las referencias est�n asignadas
        if (pointCounter == null || scoreText == null)
        {
            Debug.LogError("PointCounter o TextMeshProUGUI no est�n asignados correctamente.");
        }
    }

    void Update()
    {
        // Actualiza el texto de puntaje cada cuadro
        if (pointCounter != null && scoreText != null)
        {
            scoreText.text = "Puntaje: " + pointCounter.GetPoints();
        }
    }
}
