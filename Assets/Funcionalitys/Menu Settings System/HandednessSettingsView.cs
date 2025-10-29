using UnityEngine;
using UnityEngine.UI;

public class HandednessSettingsView : MonoBehaviour
{
    public Button izqButton;
    public Button derButton;
    public Button saveButton;
    public Button cancelButton;

    // (Opcional) Referencias para la lógica de confirmación
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

        // Asignar listeners (llaman a la función de ACTUALIZACIÓN EN MEMORIA)
        izqButton.onClick.AddListener(() => OnSelectHandedness(true));
        derButton.onClick.AddListener(() => OnSelectHandedness(false));
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);

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
    public void OnSelectHandedness(bool isLeftHanded)
    {
        // Carga los otros valores del buffer de memoria para mantener la consistencia
        float volume = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        // 1. Actualiza el buffer en memoria y aplica los cambios al juego
        SettingsManager.Instance.UpdateInMemorySettings(volume, sound, brightness, isLeftHanded, bulletMark, autoPointer);

        // 2. Actualiza solo la parte visual de este menú
        UpdateButtonColors(isLeftHanded);
    }

    // --- Lógica de Guardado y Confirmación (GUARDA EN DISCO O CANCELA) ---
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
        }
    }

    private void SaveSettingsToManager()
    {
        // El Manager ya tiene el estado más reciente, solo le pedimos que guarde a disco.
        SettingsManager.Instance.SaveToDiskAndApply();
        gameObject.SetActive(false);
        CallShow(MenuSettings);
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
        bool isLeftHanded = SettingsManager.Instance.CurrentIsLeftHanded;
        UpdateButtonColors(isLeftHanded);
    }

    private void UpdateButtonColors(bool isLeftHanded)
    {
        // Lógica de color de botones para indicar la selección
        if (isLeftHanded)
        {
            izqButton.GetComponent<Image>().color = Color.green;
            derButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            derButton.GetComponent<Image>().color = Color.green;
            izqButton.GetComponent<Image>().color = Color.white;
        }
    }
}