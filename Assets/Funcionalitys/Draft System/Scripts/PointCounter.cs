using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class PointCounter : MonoBehaviour
{
    private int score = 0;

    [Header("Texto de puntos añadidos")]
    public TextMeshProUGUI pointsAddedText; // Texto que mostrará "+10", "+5", etc.

    // 🔹 Método para sumar puntos y mantener el texto visible hasta el siguiente cambio
    public void AddPoints(int points)
    {
        score += points;

        if (pointsAddedText != null)
        {
            pointsAddedText.text = "+" + points;
        }
    }

    // 🔹 Método para obtener el puntaje actual (por si lo necesitas)
    public int GetPoints()
    {
        return score;
    }
}
