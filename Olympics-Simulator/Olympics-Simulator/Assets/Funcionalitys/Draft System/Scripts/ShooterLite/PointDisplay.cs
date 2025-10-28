using UnityEngine;
using TMPro; // Asegúrate de tener este namespace

public class PointDisplay : MonoBehaviour
{
    public PointCounter pointCounter; // Referencia al script PointCounter
    public TextMeshProUGUI scoreText; // Referencia al TextMeshPro donde se mostrará el puntaje

    void Start()
    {
        // Asegúrate de que las referencias estén asignadas
        if (pointCounter == null || scoreText == null)
        {
            Debug.LogError("PointCounter o TextMeshProUGUI no están asignados correctamente.");
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
