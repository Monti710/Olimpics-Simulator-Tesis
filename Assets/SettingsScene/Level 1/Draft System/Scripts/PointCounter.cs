using UnityEngine;
using TMPro;

public class PontCounter : MonoBehaviour
{
    public Sublittle subAt50;   // referencia al GameObject con Sub para 50 puntos
    public Sublittle subAt100;  // referencia al GameObject con Sub para 100 puntos

    public TextMeshProUGUI pointsText; // referencia para mostrar puntos

    private int totalPoints = 0;

    private bool hasTriggered50 = false;
    private bool hasTriggered100 = false;

    public void AddPoints(int amount)
    {
        totalPoints += amount;
        UpdateDisplay();
        CheckPoints();
    }

    private void UpdateDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = "Puntos: " + totalPoints.ToString();
        }
    }

    private void CheckPoints()
    {
        if (!hasTriggered50 && totalPoints >= 50)
        {
            hasTriggered50 = true;
            if (subAt50 != null)
                subAt50.PlaySubtitlesAndAudio();
        }

        if (!hasTriggered100 && totalPoints >= 100)
        {
            hasTriggered100 = true;
            if (subAt100 != null)
                subAt100.PlaySubtitlesAndAudio();
        }
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }
}
