using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlBrilloAmbiental : MonoBehaviour
{
    public Slider sliderBrillo;

    void Start()
    {
        float brilloGuardado = PlayerPrefs.GetFloat("BrilloAmbiental", 1f);
        RenderSettings.ambientIntensity = brilloGuardado;

        if (sliderBrillo != null)
        {
            sliderBrillo.minValue = 0f;
            sliderBrillo.maxValue = 3f;
            sliderBrillo.value = brilloGuardado;
            sliderBrillo.onValueChanged.AddListener(AdjustAmbientIntensity);
        }

        // 🔹 Escucha el evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Aplica el brillo guardado al cargar cualquier escena nueva
        RenderSettings.ambientIntensity = PlayerPrefs.GetFloat("BrilloAmbiental", 1f);
    }

    public void AdjustAmbientIntensity(float nuevoValor)
    {
        RenderSettings.ambientIntensity = nuevoValor;
        PlayerPrefs.SetFloat("BrilloAmbiental", nuevoValor);
    }
}
