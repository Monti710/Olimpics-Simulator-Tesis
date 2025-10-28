using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsView : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public Button saveButton;
    public Button cancelButton;

    public Button confirmButton;
    public Button cancelConfirmationButton;

    // 2. Campos de texto para las claves de los menús
    public string MenuConfirmation;
    public string MenuInitial;
    public string MenuSettings;

    // ⭐ 3. SELECTOR Y REFERENCIAS DE MANAGERS ⭐
    [Header("Menu Manager Selection")]
    public MenuManagerType activeManagerType = MenuManagerType.UserMenuManagers;
    public UserMenuManagers userMenuManagers; // Referencia al primer script gestor
    public MenuManager menuManager;           // Referencia al segundo script gestor

    // Estos deciden a qué gestor llamar basándose en el enum
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

        // Los listeners llaman a la función de ACTUALIZACIÓN EN MEMORIA
        musicSlider.onValueChanged.AddListener(OnVolumeChanged);
        soundSlider.onValueChanged.AddListener(OnSoundChanged);

        // Listeners de confirmación
        confirmButton.onClick.AddListener(OnConfirmChange);
        cancelConfirmationButton.onClick.AddListener(OnCancelChange);

    }

    void OnEnable()
    {
        // Sincroniza la UI al abrir el menú para reflejar el buffer actual
        RefreshUIFromSettings();
    }

    // --- Métodos de Cambio (ACTUALIZA EL BUFFER DE MEMORIA) ---
    // Usamos los valores actuales del Manager para no perder los otros parámetros.

    public void OnVolumeChanged(float newVolume)
    {
        // Carga los otros valores del buffer de memoria para mantener la consistencia
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool handedness = SettingsManager.Instance.CurrentIsLeftHanded;
        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        // Actualiza el buffer en memoria y aplica los cambios al juego
        SettingsManager.Instance.UpdateInMemorySettings(newVolume, sound, brightness, handedness, bulletMark, autoPointer);
    }

    public void OnSoundChanged(float newSound)
    {
        // Carga los otros valores del buffer de memoria para mantener la consistencia
        float volume = SettingsManager.Instance.CurrentVolume;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool handedness = SettingsManager.Instance.CurrentIsLeftHanded;
        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        // Actualiza el buffer en memoria y aplica los cambios al juego
        SettingsManager.Instance.UpdateInMemorySettings(volume, newSound, brightness, handedness, bulletMark, autoPointer);
    }

    // --- Lógica de Botones (GUARDA EN DISCO O CANCELA) ---

    private void OnSaveButtonClicked()
    {

        // Si hay lógica de confirmación (cambio de mano)
        if (SettingsManager.Instance.CurrentIsLeftHanded != (PlayerPrefs.GetInt("IsLeftHanded", 0) == 1))
        {
            CallShow(MenuConfirmation);
        }
        else
        {
            SaveSettingsToManager();
            CallShow(MenuSettings);
        }
    }

    private void OnCancelButtonClicked()
    {
        // Restauramos el buffer de memoria a los últimos valores GUARDADOS al disco.
        SettingsManager.Instance.LoadAndApplySettings();

        // Refrescar la UI para reflejar los valores restaurados
        RefreshUIFromSettings();
        gameObject.SetActive(false);
        CallShow(MenuSettings);
    }

    // --- Método de Sincronización ---
    public void RefreshUIFromSettings()
    {
        // Usamos el Singleton para obtener los valores actuales (del buffer de memoria)
        musicSlider.value = SettingsManager.Instance.CurrentVolume;
        soundSlider.value = SettingsManager.Instance.CurrentSound;
    }

    private void SaveSettingsToManager()
    {
        // El Manager ya tiene el estado más reciente, solo le pedimos que guarde a disco.
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
        // Restaurar los valores del buffer de memoria a los últimos valores GUARDADOS al disco
        SettingsManager.Instance.LoadAndApplySettings();

        RefreshUIFromSettings();
        CallShow(MenuInitial);
    }
}