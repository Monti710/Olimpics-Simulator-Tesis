using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Implementación del Singleton
    public static SettingsManager Instance { get; private set; }

    // Referencias a los sistemas de la escena
    private PlayerSettings playerSettings;
    // private XRControllerManager xrControllerManager; 

    // Valores en Memoria (Buffer de Configuración)
    public float CurrentVolume { get; private set; }
    public float CurrentSound {  get; private set; }
    public float CurrentBrightness { get; private set; }
    public bool CurrentIsLeftHanded { get; private set; }
    public bool CurrentBulletMark {  get; private set; }
    public bool CurrentAutoPointer { get; private set; }
    public bool CurrentUserInterface {  get; private set; }
    public bool CurrentVisualObjects { get; private set; }

    void Awake()
    {
        // Asegurar que solo hay una instancia (Singleton Pattern)
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Opcional: persistir entre escenas
    }

    void Start()
    {
        // Obtener referencias a los componentes (Asegúrate de que PlayerSettings esté en la escena)
        playerSettings = FindObjectOfType<PlayerSettings>();
        // xrControllerManager = FindObjectOfType<XRControllerManager>(); 

        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings script not found in scene. Settings will not be persistent.");
            return;
        }

        // Cargar la configuración al inicio del juego
        LoadAndApplySettings();
    }

    public void LoadAndApplySettings()
    {
        if (playerSettings == null) return;

        // 1. Cargar los datos PERMANENTES del disco
        playerSettings.LoadPlayerSettings(out float volume, out float sound, out float brightness, out bool isLeftHanded, out bool bulletMark, out bool autoPointer, out bool userInterface, out bool visualObjects);

        // 2. Almacenar internamente
        CurrentVolume = volume;
        CurrentSound = sound;
        CurrentBrightness = brightness;
        CurrentIsLeftHanded = isLeftHanded;
        CurrentBulletMark = bulletMark;
        CurrentAutoPointer = autoPointer;
        CurrentUserInterface = userInterface;
        CurrentVisualObjects = visualObjects;

        Debug.Log("Configuración inicial cargada y aplicada desde disco.");
    }

    public void UpdateInMemorySettings(float newVolume, float newSound, float newBrightness, bool newIsLeftHanded, bool newBulletMark, bool newAutoPointer, bool newUserInterface, bool newVisualObjects)
    {
        // 1. Actualizar el buffer en memoria
        CurrentVolume = newVolume;
        CurrentSound = newSound;
        CurrentBrightness = newBrightness;
        CurrentIsLeftHanded = newIsLeftHanded;
        CurrentBulletMark = newBulletMark;
        CurrentAutoPointer = newAutoPointer;
        CurrentUserInterface = newUserInterface;
        CurrentVisualObjects = newVisualObjects;

        // 2. Aplicar los cambios inmediatamente al juego

        Debug.Log("Configuración actualizada en memoria y aplicada. NO GUARDADA en disco.");
    }

    public void SaveToDiskAndApply()
    {
        // 1. Guardar los datos del buffer al disco (PlayerSettings)
        playerSettings.SavePlayerSettings(CurrentVolume, CurrentSound, CurrentBrightness, CurrentIsLeftHanded, CurrentBulletMark, CurrentAutoPointer, CurrentUserInterface, CurrentVisualObjects);

        // 2. Aplicar al juego (por si acaso)

        Debug.Log("Configuración guardada en disco (PlayerPrefs) exitosamente.");
    }

}