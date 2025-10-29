using UnityEngine;
using TMPro;

// Aunque queremos la asignación manual del AudioSource, 
// se recomienda mantener el RequireComponent para garantizar que el objeto tenga el componente.
[RequireComponent(typeof(AudioSource))]
public class WindManager : MonoBehaviour
{
    // Usamos un Singleton para que cualquier objeto pueda acceder fácilmente al viento.
    public static WindManager instance;

    [Header("Configuración del Viento")]
    [Tooltip("La fuerza o aceleración del viento.")]
    [Range(0f, 100f)]
    public float windStrength = 10f;
    [Tooltip("La dirección global hacia donde sopla el viento.")]
    public Vector3 windDirection = Vector3.right;

    [Header("UI")]
    [Tooltip("El campo de texto para mostrar la velocidad del viento.")]
    public TextMeshProUGUI windStrengthText;

    [Header("Audio 🔊")]
    [Tooltip("Referencia al componente AudioSource que gestionará el sonido del viento. ¡Debe ser asignado en el Inspector!")]
    // Hacemos esta variable pública para forzar la asignación en el Inspector.
    public AudioSource audioSource;

    // ❌ ELIMINADO: Ya no necesitamos asignar el AudioClip aquí, debe estar en el AudioSource.
    // public AudioClip windSoundClip; 

    [Tooltip("El volumen máximo que alcanzará el sonido con la máxima fuerza del viento.")]
    [Range(0f, 1f)]
    public float maxVolume = 0.8f;
    [Tooltip("El tono mínimo del sonido (viento calmado).")]
    public float minPitch = 0.7f;
    [Tooltip("El tono máximo del sonido (viento fuerte).")]
    public float maxPitch = 1.3f;

    void Awake()
    {
        // Si el AudioSource no se ha asignado manualmente, lo obtenemos del GameObject (Seguridad).
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Configuración del Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("El componente AudioSource NO ha sido asignado al WindManager. El audio del viento no funcionará.", this);
            return;
        }

        // ⚠️ El AudioClip (sonido del viento) DEBE estar asignado directamente en el componente AudioSource 
        // a través del Inspector de Unity.

        // Configuramos para que siempre esté en bucle (loop) y lo iniciamos si tiene un clip.
        audioSource.loop = true;

        // Solo reproducimos si hay un clip asignado directamente en el AudioSource.
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("El AudioSource asignado no tiene un AudioClip (sonido de viento) asignado. El audio no se reproducirá.", this);
        }
    }

    void Update()
    {
        // Actualiza la UI
        UpdateWindUI();

        // Actualiza el audio solo si el AudioSource está asignado y tiene un clip.
        if (audioSource != null && audioSource.clip != null)
        {
            UpdateWindAudio();
        }
    }

    void UpdateWindUI()
    {
        if (windStrengthText != null)
        {
            float windKPH = windStrength * 5f;
            windStrengthText.text = $"Viento: {windKPH:F1}km/h";
        }
    }

    void UpdateWindAudio()
    {
        // ❌ CAMBIO CLAVE: Ya no necesitamos la verificación de windSoundClip, 
        // pues la hicimos en Update() verificando audioSource.clip.

        // Calcula el progreso del viento (de 0 a 1) basado en la fuerza máxima (100)
        float windProgress = windStrength / 100f;

        // Ajusta el volumen y el tono basándose en la fuerza actual del viento
        audioSource.volume = Mathf.Lerp(0, maxVolume, windProgress);
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, windProgress);
    }

}