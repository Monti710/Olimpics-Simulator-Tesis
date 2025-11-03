using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ScoreHistory : MonoBehaviour
{
    [Header("Textos de listas")]
    public TextMeshProUGUI top5Text;
    public TextMeshProUGUI allScoresText;

    [Header("Textos de los 3 primeros lugares")]
    public TextMeshProUGUI firstPlaceText;
    public TextMeshProUGUI secondPlaceText;
    public TextMeshProUGUI thirdPlaceText;

    void Start()
    {
        // Cargar puntajes guardados
        ScoreList scoreList = LocalScoreManager.LoadScores();

        if (scoreList == null || scoreList.scores.Count == 0)
        {
            top5Text.text = "Sin puntajes registrados.";
            allScoresText.text = "No hay datos.";
            if (firstPlaceText) firstPlaceText.text = "";
            if (secondPlaceText) secondPlaceText.text = "";
            if (thirdPlaceText) thirdPlaceText.text = "";
            return;
        }

        // Ordenar de mayor a menor
        List<ScoreData> ordered = scoreList.scores
            .OrderByDescending(s => s.score)
            .ToList();

        // 🥇 Mostrar los 3 primeros lugares (si existen)
        if (ordered.Count > 0)
            firstPlaceText.text = $"1. {ordered[0].playerName}   {ordered[0].score} pts";
        if (ordered.Count > 1)
            secondPlaceText.text = $"2. {ordered[1].playerName}   {ordered[1].score} pts";
        if (ordered.Count > 2)
            thirdPlaceText.text = $"3. {ordered[2].playerName}   {ordered[2].score} pts";

        // 🏆 Top 5
        string top5String = "<b>🏆 TOP 5:</b>\n";
        for (int i = 0; i < Mathf.Min(5, ordered.Count); i++)
        {
            ScoreData s = ordered[i];
            top5String += $"{i + 1}. {s.playerName} - {s.score} pts ({s.date})\n";
        }
        top5Text.text = top5String;

        // 📋 Todos los puntajes
        string allString = "<b>📋 TODOS LOS PUNTAJES:</b>\n";
        int count = 1;
        foreach (ScoreData s in scoreList.scores)
        {
            allString += $"{count++}. {s.playerName} - {s.score} pts ({s.date})\n";
        }
        allScoresText.text = allString;
    }
}
