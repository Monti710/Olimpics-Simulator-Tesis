using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Cerrando el juego...");

#if UNITY_EDITOR
        // Detiene el juego en el editor de Unity
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Cierra la aplicación compilada
        Application.Quit();
#endif
    }
}
