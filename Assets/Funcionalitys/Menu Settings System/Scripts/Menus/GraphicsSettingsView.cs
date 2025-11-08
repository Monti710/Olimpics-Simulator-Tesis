using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsView : MonoBehaviour
{
    [Header("UI (Lectura y Escritura)")]
    public Slider brightnessSlider;

    [Header("Botones")]
    public Button saveButton;
    public Button cancelButton;
    public Button confirmButton;
    public Button cancelConfirmationButton;

    [Header("Sistema de Preview")]
    [Tooltip("Arrastra aquí el objeto que tiene el script ControlBrilloYOscuridadURPModern")]
    public ControlBrilloYOscuridadURPModern brightnessControl; // <-- Referencia

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

        // El listener AHORA llama a preview Y buffer
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        confirmButton.onClick.AddListener(OnConfirmChange);
        cancelConfirmationButton.onClick.AddListener(OnCancelChange);
    }

    void OnEnable()
    {
        RefreshUIFromSettings();
    }

    // --- Método de Cambio (PREVIEW + BUFFER) ---

    public void OnBrightnessChanged(float newBrightness)
    {
        // 1. Aplicar la VISTA PREVIA
        if (brightnessControl != null)
        {
            brightnessControl.PreviewBrightness(newBrightness);
        }

        // 2. Actualizar el BUFFER (tu lógica original)
        float volume = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;
        bool handedness = SettingsManager.Instance.CurrentIsLeftHanded;
        //... (etc.)

        SettingsManager.Instance.UpdateInMemorySettings(volume, sound, newBrightness, handedness,
            SettingsManager.Instance.CurrentBulletMark,
            SettingsManager.Instance.CurrentAutoPointer,
            SettingsManager.Instance.CurrentUserInterface,
            SettingsManager.Instance.CurrentVisualObjects);
    }

    // --- Lógica de Botones ---

    private void OnSaveButtonClicked()
    {
        // El buffer ya está actualizado
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

        float brightness = SettingsManager.Instance.CurrentBrightness;

        // 1. Actualizar slider (sin disparar evento)
        if (brightnessSlider != null)
            brightnessSlider.SetValueWithoutNotify(brightness);

        // 2. Forzar al sistema de preview a resetearse
        if (brightnessControl != null)
        {
            brightnessControl.SetValue(brightness);
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