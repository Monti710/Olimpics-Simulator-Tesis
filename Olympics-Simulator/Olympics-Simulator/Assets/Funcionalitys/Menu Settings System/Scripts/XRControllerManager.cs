using UnityEngine;

public class XRControllerManager : MonoBehaviour
{
    // Referencias a los dos componentes XR Input Modality Manager
    public GameObject xrInputModalityManager1;  // Primer componente con LeftController y RightController
    public GameObject xrInputModalityManager2;  // Segundo componente con LeftPistolController y RightPistolController

    // Referencias a los objetos en el GameObject
    public GameObject object1;  // Referencia al objeto 1 (LeftController)
    public GameObject object2;  // Referencia al objeto 2 (RightController)
    public GameObject object3;  // Referencia al objeto 3 (LeftPistolController)
    public GameObject object4;  // Referencia al objeto 4 (RightPistolController)

    // Referencia al personaje o controlador XR que necesita ser actualizado
    public GameObject playerCharacter; // El personaje o controlador XR que quieres actualizar (si es necesario)

    private int isLeftHanded;  // Para almacenar la preferencia de mano (0 = diestro, 1 = zurdo)

    void Start()
    {
        // Cargar la preferencia de mano izquierda/derecha desde PlayerPrefs al inicio
        isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);  // 0 es diestro, 1 es zurdo

        // Llamar a la función para activar/desactivar los componentes según la preferencia
        ToggleXRControllers(isLeftHanded);
    }

    // Función para activar/desactivar los componentes de los controladores
    public void ToggleXRControllers(int isLeftHanded)
    {
        if (isLeftHanded == 1)
        {
            // Si es zurdo (1), activamos el segundo componente y desactivamos el primero
            xrInputModalityManager1.SetActive(false);  // Desactivamos el primer componente
            object1.SetActive(false);                   // Desactivamos LeftController
            object2.SetActive(false);                   // Desactivamos RightController

            xrInputModalityManager2.SetActive(true);   // Activamos el segundo componente
            object3.SetActive(true);                   // Activamos LeftPistolController
            object4.SetActive(true);                   // Activamos RightPistolController

            // Si es necesario, actualizar otros componentes relacionados con el personaje
            UpdatePlayerCharacter("Left");
        }
        else
        {
            // Si es diestro (0), activamos el primer componente y desactivamos el segundo
            xrInputModalityManager1.SetActive(true);   // Activamos el primer componente
            object1.SetActive(true);                   // Activamos LeftController
            object2.SetActive(true);                   // Activamos RightController

            xrInputModalityManager2.SetActive(false);  // Desactivamos el segundo componente
            object3.SetActive(false);                  // Desactivamos LeftPistolController
            object4.SetActive(false);                  // Desactivamos RightPistolController

            // Si es necesario, actualizar otros componentes relacionados con el personaje
            UpdatePlayerCharacter("Right");
        }
    }

    // Función para actualizar el personaje o los controladores según la mano seleccionada
    private void UpdatePlayerCharacter(string handType)
    {
        // Actualiza el personaje o controlador XR en función de la mano seleccionada
        // Esto puede ser el cambio de los controladores, manos o modelos del personaje

        // Ejemplo: si hay un personaje, puedes cambiar sus propiedades según la mano seleccionada
        if (playerCharacter != null)
        {
            // Aquí podrías hacer ajustes al modelo del personaje, como cambiar las animaciones, manos, etc.
            // Este es solo un ejemplo, ajusta según tu implementación específica.
            if (handType == "Left")
            {
                // Configurar el personaje para la mano izquierda
                Debug.Log("Player is set to left-handed mode.");
            }
            else
            {
                // Configurar el personaje para la mano derecha
                Debug.Log("Player is set to right-handed mode.");
            }
        }
    }

    // Método para cambiar la preferencia de mano (ejecutado por los botones)
    public void SetHandedness()
    {
        // Llamar a la preferencia de mano en PlayerPrefs
        isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);

        // Actualizar la variable local y activar/desactivar los controladores
        ToggleXRControllers(isLeftHanded);
    }
}
