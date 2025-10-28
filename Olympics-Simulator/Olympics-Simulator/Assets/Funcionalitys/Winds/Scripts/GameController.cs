using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Sistemas del Juego a Activar")]
    [Tooltip("Arrastra aquí todos los GameObjects que deben activarse al iniciar el juego.")]
    public GameObject[] systemsToActivate; // Array para activar GameObjects (gameObject.SetActive(true))

    // 🆕 NUEVO ARRAY: Acepta cualquier script o componente derivado de MonoBehaviour.
    [Tooltip("Arrastra aquí los componentes (scripts) que deben habilitarse al iniciar el juego.")]
    public MonoBehaviour[] componentsToEnable; // Array para habilitar componentes (component.enabled = true)

    public void StartGame()
    {
        // 1. ACTIVACIÓN DE GAMEOBJECTS (Encender/Apagar todo el objeto)
        foreach (GameObject systemObject in systemsToActivate)
        {
            if (systemObject != null)
            {
                systemObject.SetActive(true);
            }
        }

        // 2. HABILITACIÓN DE COMPONENTES (Encender/Apagar solo el script)
        foreach (MonoBehaviour component in componentsToEnable)
        {
            // Verificamos que el componente no sea nulo.
            if (component != null)
            {
                // Habilitamos el componente (esto llama a Start() o Update() si estaba inactivo).
                component.enabled = true;
            }
        }

        // Desactiva este script GameController después de ejecutar la lógica de inicio.
        this.enabled = false;
    }
}