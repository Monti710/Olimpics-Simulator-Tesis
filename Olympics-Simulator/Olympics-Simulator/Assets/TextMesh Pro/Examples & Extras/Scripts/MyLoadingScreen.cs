using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyLoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public Slider loadingBar;

    public float loadDuration = 0.7f; // Tiempo total de carga simulado
    private float loadProgress = 0f;

    private float dotTimer = 0f;
    private int dotCount = 0;

    void Update()
    {
        // Simula el progreso de carga
        if (loadProgress < 1f)
        {
            loadProgress += Time.deltaTime / loadDuration;
            loadingBar.value = loadProgress;
        }

        // Animación de los puntos "..."
        dotTimer += Time.deltaTime;
        if (dotTimer >= 0.5f)
        {
            dotTimer = 0f;
            dotCount = (dotCount + 1) % 4;
            loadingText.text = "Cargando" + new string('.', dotCount);
        }
    }
}
