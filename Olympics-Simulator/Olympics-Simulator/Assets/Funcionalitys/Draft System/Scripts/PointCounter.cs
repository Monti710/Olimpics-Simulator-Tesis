using UnityEngine;

public class PointCounter : MonoBehaviour
{
    private int score = 0;

    // Método para sumar puntos
    public void AddPoints(int points)
    {
        score += points;
    }

    // Método para obtener el puntaje actual
    public int GetPoints()
    {
        return score;
    }
}
