using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para trabajar con la UI

public class SaveScene : MonoBehaviour
{
    private string SceneSaveWithDIfficulty = "SceneSaveWithDIfficulty";

    // Variable a guardar
    public string nameScene;

    public void SaveMap()
    {
        // Guardar la variable en PlayerPrefs
        PlayerPrefs.SetString("SceneSaveWithDIfficulty", nameScene);

        // Asegurarse de que los cambios se guarden
        PlayerPrefs.Save();

        // Confirmar que se guardó correctamente (opcional)
        Debug.Log("Variable guardada: " + nameScene);
    }

    public void SaveMapWithDifficulty()
    {
        string valorGuardado = PlayerPrefs.GetString(SceneSaveWithDIfficulty, "MainMenu");
        string valorConjunto = valorGuardado + nameScene;

        PlayerPrefs.SetString("NextScene", valorConjunto); // Guarda el destino
        SceneManager.LoadScene("LoadingScene"); // Va a la escena de carga

        Debug.Log("Variable guardada: " + valorConjunto);
    }
}
