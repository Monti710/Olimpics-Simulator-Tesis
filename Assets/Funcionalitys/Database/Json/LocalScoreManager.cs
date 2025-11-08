using System.IO;
using UnityEngine;

public static class LocalScoreManager
{
    public static void SaveScore(ScoreData newScore, string path)
    {
        ScoreList scoreList = LoadScores(path);
        scoreList.scores.Add(newScore);

        string json = JsonUtility.ToJson(scoreList, true);
        File.WriteAllText(path, json);
    }

    public static ScoreList LoadScores(string path)
    {
        if (!File.Exists(path))
        {
            return new ScoreList();
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ScoreList>(json);
    }

    public static void OverwriteScores(ScoreList updatedList, string path)
    {
        string json = JsonUtility.ToJson(updatedList, true);
        File.WriteAllText(path, json);
    }
}
