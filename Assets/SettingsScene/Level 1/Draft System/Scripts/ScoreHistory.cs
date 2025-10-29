using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ScoreHistory : MonoBehaviour
{
    public TextMeshProUGUI top5Text;
    public TextMeshProUGUI allScoresText;

    void Start()
    {
        ScoreList scoreList = LocalScoreManager.LoadScores();

        // Ordenar de mayor a menor puntaje
        List<ScoreData> ordered = scoreList.scores
            .OrderByDescending(s => s.score)
            .ToList();

        // 🥇 Top 5
        string top5String = "<b>🏆 TOP 5:</b>\n";
        for (int i = 0; i < Mathf.Min(5, ordered.Count); i++)
        {
            ScoreData s = ordered[i];
            top5String += $"{i + 1}. {s.playerName} - {s.score} pts ({s.date})\n";
        }
        top5Text.text = top5String;

        // 📋 Todos
        string allString = "<b>📋 TODOS LOS PUNTAJES:</b>\n";
        int count = 1;
        foreach (ScoreData s in scoreList.scores)
        {
            allString += $"{count++}. {s.playerName} - {s.score} pts ({s.date})\n";
        }
        allScoresText.text = allString;
    }
}
