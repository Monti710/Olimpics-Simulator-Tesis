using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ScoreManager
{
    private const string ScoresKey = "ScoreHistory";

    public static void SaveNewScore(int score)
    {
        List<int> scores = GetAllScores();
        scores.Add(score);
        string scoreString = string.Join(",", scores);
        PlayerPrefs.SetString(ScoresKey, scoreString);
        PlayerPrefs.Save();
    }

    public static List<int> GetAllScores()
    {
        string scoreString = PlayerPrefs.GetString(ScoresKey, "");
        if (string.IsNullOrEmpty(scoreString))
            return new List<int>();

        return scoreString.Split(',').Select(int.Parse).ToList();
    }

    public static List<int> GetTopScores(int count)
    {
        return GetAllScores()
            .OrderByDescending(s => s)
            .Take(count)
            .ToList();
    }
}
