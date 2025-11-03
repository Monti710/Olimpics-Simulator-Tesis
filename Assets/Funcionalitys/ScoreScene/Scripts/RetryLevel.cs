using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadScene()
    {
        string sceneToLoad = PlayerPrefs.GetString("LevelScene");
        PlayerPrefs.SetString("NextScene", sceneToLoad);
        SceneManager.LoadScene("LoadingScene"); // Va a la escena de carga
    }
}
