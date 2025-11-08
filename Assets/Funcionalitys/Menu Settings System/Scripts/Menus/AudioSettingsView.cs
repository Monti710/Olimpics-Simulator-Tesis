using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsView : MonoBehaviour
{
    [Header("UI (Lectura y Escritura)")]
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Botones")]
    public Button saveButton;
    public Button cancelButton;
    public Button confirmButton;
    public Button cancelConfirmationButton;

    [Header("Sistema de Preview")]
    [Tooltip("Arrastra aquí el objeto que tiene el script VolumeControl")]
    public VolumeControl volumeControl; // <-- Referencia al sistema de preview

    [Header("Navegación de Menú")]
    public string MenuConfirmation;
    public string MenuInitial;
    public string MenuSettings;
    public MenuManagerType activeManagerType = MenuManagerType.UserMenuManagers;
    public UserMenuManagers userMenuManagers;
    public MenuManager menuManager;

    private void CallShow(string key)
    {
        if (activeManagerType == MenuManagerType.UserMenuManagers && userMenuManagers != null)
        {
            userMenuManagers.Show(key);
        }
        else if (activeManagerType == MenuManagerType.MenuManager && menuManager != null)
        {
            menuManager.Show(key);
        }
        else
        {
            Debug.LogError($"Cannot Show menu '{key}'. No valid manager assigned for type: {activeManagerType}");
        }
    }

    private void CallHideCurrent()
    {
        if (activeManagerType == MenuManagerType.UserMenuManagers && userMenuManagers != null)
        {
            userMenuManagers.HideCurrent();
        }
        else if (activeManagerType == MenuManagerType.MenuManager && menuManager != null)
        {
            menuManager.HideCurrent();
        }
        else
        {
            Debug.LogError($"Cannot HideCurrent menu. No valid manager assigned for type: {activeManagerType}");
        }
    }

    void Start()
    {
        RefreshUIFromSettings();

        saveButton.onClick.AddListener(OnSaveButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);

        // Los listeners AHORA llaman a las funciones de preview Y de buffer
        musicSlider.onValueChanged.AddListener(OnVolumeChanged);
        soundSlider.onValueChanged.AddListener(OnSoundChanged);

        confirmButton.onClick.AddListener(OnConfirmChange);
        cancelConfirmationButton.onClick.AddListener(OnCancelChange);
    }

    void OnEnable()
    {
        RefreshUIFromSettings();
    }

    // --- Métodos de Cambio (PREVIEW + BUFFER) ---

    public void OnVolumeChanged(float newVolume)
    {
        // 1. Aplicar la VISTA PREVIA
        if (volumeControl != null)
        {
            volumeControl.PreviewMusicVolume(newVolume);
        }

        // 2. Actualizar el BUFFER (tu lógica original)
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool handedness = SettingsManager.Instance.CurrentIsLeftHanded;
        //... (etc.)

        SettingsManager.Instance.UpdateInMemorySettings(newVolume, sound, brightness, handedness,
            SettingsManager.Instance.CurrentBulletMark,
            SettingsManager.Instance.CurrentAutoPointer,
            SettingsManager.Instance.CurrentUserInterface,
            SettingsManager.Instance.CurrentVisualObjects);
    }

    public void OnSoundChanged(float newSound)
    {
        // 1. Aplicar la VISTA PREVIA
        if (volumeControl != null)
        {
            volumeControl.PreviewSoundVolume(newSound);
        }

        // 2. Actualizar el BUFFER (tu lógica original)
        float volume = SettingsManager.Instance.CurrentVolume;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool handedness = SettingsManager.Instance.CurrentIsLeftHanded;
        //... (etc.)

        SettingsManager.Instance.UpdateInMemorySettings(volume, newSound, brightness, handedness,
            SettingsManager.Instance.CurrentBulletMark,
            SettingsManager.Instance.CurrentAutoPointer,
            SettingsManager.Instance.CurrentUserInterface,
            SettingsManager.Instance.CurrentVisualObjects);
    }

    // --- Lógica de Botones ---

    private void OnSaveButtonClicked()
    {
        // No necesitamos llamar a UpdateSettingsBufferFromUI()
        // porque el buffer ya se actualizó en vivo.

        if (SettingsManager.Instance.CurrentIsLeftHanded != (PlayerPrefs.GetInt("IsLeftHanded", 0) == 1))
            CallShow(MenuConfirmation);
        else
        {
            SaveSettingsToManager(); // Solo guarda
            CallShow(MenuSettings);
        }
    }

    private void OnCancelButtonClicked()
    {
        SettingsManager.Instance.LoadAndApplySettings(); // Carga desde disco al buffer
        RefreshUIFromSettings(); // Refresca UI y Preview
        gameObject.SetActive(false);
        CallShow(MenuSettings);
    }

    // --- Método de Sincronización (Reset) ---
    public void RefreshUIFromSettings()
    {
        if (SettingsManager.Instance == null) return;

        float music = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;

        // 1. Actualizar sliders (sin disparar eventos)
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(music);
        if (soundSlider != null) soundSlider.SetValueWithoutNotify(sound);

        // 2. Forzar al sistema de preview a resetearse
        if (volumeControl != null)
        {
            volumeControl.SetValues(music, sound);
        }
    }

    private void SaveSettingsToManager()
    {
        SettingsManager.Instance.SaveToDiskAndApply();
        gameObject.SetActive(false);
    }

    private void OnConfirmChange()
    {
        SaveSettingsToManager();
        CallHideCurrent();
    }

    private void OnCancelChange()
    {
        SettingsManager.Instance.LoadAndApplySettings();
        RefreshUIFromSettings();
        CallShow(MenuInitial);
    }
}