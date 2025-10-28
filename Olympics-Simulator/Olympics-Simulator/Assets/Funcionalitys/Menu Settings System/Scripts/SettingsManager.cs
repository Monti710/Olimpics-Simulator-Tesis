using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Implementaci�n del Singleton
    public static SettingsManager Instance { get; private set; }

    // Referencias a los sistemas de la escena
    private PlayerSettings playerSettings;
    // private XRControllerManager xrControllerManager; 

    // Valores en Memoria (Buffer de Configuraci�n)
    public float CurrentVolume { get; private set; }
    public float CurrentSound {  get; private set; }
    public float CurrentBrightness { get; private set; }
    public bool CurrentIsLeftHanded { get; private set; }
    public bool CurrentBulletMark {  get; private set; }
    public bool CurrentAutoPointer { get; private set; }

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
        // Obtener referencias a los componentes (Aseg�rate de que PlayerSettings est� en la escena)
        playerSettings = FindObjectOfType<PlayerSettings>();
        // xrControllerManager = FindObjectOfType<XRControllerManager>(); 

        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings script not found in scene. Settings will not be persistent.");
            return;
        }

        // Cargar la configuraci�n al inicio del juego
        LoadAndApplySettings();
    }

    public void LoadAndApplySettings()
    {
        if (playerSettings == null) return;

        // 1. Cargar los datos PERMANENTES del disco
        playerSettings.LoadPlayerSettings(out float volume, out float sound, out float brightness, out bool isLeftHanded, out bool bulletMark, out bool autoPointer);

        // 2. Almacenar internamente
        CurrentVolume = volume;
        CurrentSound = sound;
        CurrentBrightness = brightness;
        CurrentIsLeftHanded = isLeftHanded;
        CurrentBulletMark = bulletMark;
        CurrentAutoPointer = autoPointer;

        Debug.Log("Configuraci�n inicial cargada y aplicada desde disco.");
    }

    public void UpdateInMemorySettings(float newVolume, float newSound, float newBrightness, bool newIsLeftHanded, bool newBulletMark, bool newAutoPointer)
    {
        // 1. Actualizar el buffer en memoria
        CurrentVolume = newVolume;
        CurrentSound = newSound;
        CurrentBrightness = newBrightness;
        CurrentIsLeftHanded = newIsLeftHanded;
        CurrentBulletMark = newBulletMark;
        CurrentAutoPointer = newAutoPointer;

        // 2. Aplicar los cambios inmediatamente al juego

        Debug.Log("Configuraci�n actualizada en memoria y aplicada. NO GUARDADA en disco.");
    }

    public void SaveToDiskAndApply()
    {
        // 1. Guardar los datos del buffer al disco (PlayerSettings)
        playerSettings.SavePlayerSettings(CurrentVolume, CurrentSound, CurrentBrightness, CurrentIsLeftHanded, CurrentBulletMark, CurrentAutoPointer);

        // 2. Aplicar al juego (por si acaso)

        Debug.Log("Configuraci�n guardada en disco (PlayerPrefs) exitosamente.");
    }

}