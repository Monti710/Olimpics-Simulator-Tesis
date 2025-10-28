using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlBrilloYOscuridadURPModern : MonoBehaviour
{
    [Header("Referencias")]
    public Slider sliderBrillo;
    public Slider sliderBrillo2;
    public Light luzPrincipal;  // Dirección de la luz principal (sol)
    
    private Color skyOriginal;
    private Color equatorOriginal;
    private Color groundOriginal;
    private float intensidadOriginalLuz;
    private bool usaGradient;

    void Start()
    {
        // Detecta si la escena usa Gradient o Skybox
        usaGradient = (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight);

        // Guarda los colores originales
        if (usaGradient)
        {
            skyOriginal = RenderSettings.ambientSkyColor;
            equatorOriginal = RenderSettings.ambientEquatorColor;
            groundOriginal = RenderSettings.ambientGroundColor;
        }

        if (luzPrincipal != null)
            intensidadOriginalLuz = luzPrincipal.intensity;

        // Configurar el slider con rango -1 = oscuro / +1 = brillante
        sliderBrillo.minValue = -1f;
        sliderBrillo.maxValue = 1f;

        sliderBrillo2.minValue = -1f;
        sliderBrillo2.maxValue = 1f;

        float brilloGuardado = PlayerPrefs.GetFloat("Brightness", 0f);
        sliderBrillo.value = brilloGuardado;
        sliderBrillo2.value = brilloGuardado;

        sliderBrillo.onValueChanged.AddListener(CambiarBrillo);
        sliderBrillo2.onValueChanged.AddListener(CambiarBrillo);

        // Aplicar el valor guardado al inicio
        CambiarBrillo(brilloGuardado);

        SceneManager.sceneLoaded += (s, m) => CambiarBrillo(PlayerPrefs.GetFloat("Brightness", 0f));
    }

    public void CambiarBrillo(float valor)
    {
        float factor = 1f + valor;  // valor = -1 → 0 (oscuro) / 0 → 1 (normal) / +1 → 2 (brillante)

        // Skybox → controla el brillo ambiental
        if (!usaGradient)
        {
            RenderSettings.ambientIntensity = Mathf.Clamp(factor, 0f, 3f);
        }
        else // Gradient
        {
            RenderSettings.ambientSkyColor = skyOriginal * factor;
            RenderSettings.ambientEquatorColor = equatorOriginal * factor;
            RenderSettings.ambientGroundColor = groundOriginal * factor;
        }

        // Luz principal
        if (luzPrincipal != null)
            luzPrincipal.intensity = intensidadOriginalLuz * factor;

        // Guardar valor
        PlayerPrefs.SetFloat("Brightness", valor);
    }
}
