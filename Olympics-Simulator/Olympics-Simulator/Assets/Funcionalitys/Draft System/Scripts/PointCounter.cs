using UnityEngine;

public class PointCounter : MonoBehaviour
{
    private int score = 0;

    // M�todo para sumar puntos
    public void AddPoints(int points)
    {
        score += points;
    }

    // M�todo para obtener el puntaje actual
    public int GetPoints()
    {
        return score;
    }
}
