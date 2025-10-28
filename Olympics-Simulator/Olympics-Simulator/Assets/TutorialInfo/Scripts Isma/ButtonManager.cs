using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject panel; // El panel que contiene los botones
    public GameObject button1;  // Referencia al Botón 1
    public GameObject button2;  // Referencia al Botón 2

    // Método para desactivar el panel al inicio
    void Start()
    {
        panel.SetActive(false); // Desactiva el panel al principio
    }

    // Método para mostrar el panel
    public void ShowPanel()
    {
        panel.SetActive(true); // Activa el panel
    }

    // Método para ocultar el panel
    public void HidePanel()
    {
        panel.SetActive(false); // Desactiva el panel
    }

    // Método para ocultar el Botón 2 al presionar el Botón 1
    public void OnButton1Pressed()
    {
        button2.SetActive(false);  // Oculta el Botón 2
    }

    // Método para ocultar el Botón 1 al presionar el Botón 2
    public void OnButton2Pressed()
    {
        button1.SetActive(false);  // Oculta el Botón 1
    }
}
