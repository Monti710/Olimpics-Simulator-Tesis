using System.IO;
using UnityEngine;

public static class LocalScoreManager
{
    private static string path = Application.persistentDataPath + "/scores.json";

    public static void SaveScore(ScoreData newScore)
    {
        ScoreList scoreList = LoadScores();

        scoreList.scores.Add(newScore);

        string json = JsonUtility.ToJson(scoreList, true);
        File.WriteAllText(path, json);
    }

    public static ScoreList LoadScores()
    {
        if (!File.Exists(path))
        {
            return new ScoreList();
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ScoreList>(json);
    }
    public static void OverwriteScores(ScoreList updatedList)
    {
        string json = JsonUtility.ToJson(updatedList, true);
        File.WriteAllText(path, json);
    }

}
