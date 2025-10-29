using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        PlayerPrefs.SetString("NextScene", sceneToLoad); // Guarda el destino
        SceneManager.LoadScene("LoadingScene"); // Va a la escena de carga
    }
}
