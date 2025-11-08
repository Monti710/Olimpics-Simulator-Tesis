using UnityEngine;
using UnityEngine.UI;

public class ActivadorConTexto : MonoBehaviour
{
    [Header("Objetos a controlar")]
    public GameObject objetoParaActivar;

    public Text textoUI;
    public Text textoUI1;

    [Header("Contenido del Texto")]
    public string mensajeParaEscribir;
    public string mensajeParaEscribir1;

    public void ActivarYMostrarTexto()
    {
        if (objetoParaActivar != null)
        {
            objetoParaActivar.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No has asignado el 'objetoParaActivar' en el Inspector.");
        }

        if (textoUI != null && textoUI1 != null)
        {
            textoUI.text = mensajeParaEscribir;
            textoUI1.text = mensajeParaEscribir1;
        }
        else
        {
            Debug.LogWarning("No has asignado el 'textoUI' (TextMeshPro) en el Inspector.");
        }
    }
}