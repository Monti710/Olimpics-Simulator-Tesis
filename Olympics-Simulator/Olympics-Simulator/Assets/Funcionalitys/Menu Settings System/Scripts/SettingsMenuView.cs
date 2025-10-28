using UnityEngine;
using UnityEngine.UI;

// 1. Nuevo Enum para seleccionar el Manager en el Inspector
public enum MenuManagerType
{
    UserMenuManagers,
    MenuManager
}

public class SettingsMenuView : MonoBehaviour
{
    // Referencias a los elementos de UI
    public Button izqButton;
    public Button derButton;
    public Button saveButton;
    public Button cancelButton;
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Button confirmButton;
    public Button cancelConfirmationButton;

    // Variables para almacenar los valores que se están editando
    private float currentVolume;
    private float currentBrightness;
    private bool currentIsLeftHanded;
    private bool initialIsLeftHanded;

    // 2. Campos de texto para las claves de los menús
    public string MenuConfirmation;
    public string MenuInitial;
    public string MenuSettings;

    // ⭐ 3. SELECTOR Y REFERENCIAS DE MANAGERS ⭐
    [Header("Menu Manager Selection")]
    public MenuManagerType activeManagerType = MenuManagerType.UserMenuManagers;
    public UserMenuManagers userMenuManagers; // Referencia al primer script gestor
    public MenuManager menuManager;           // Referencia al segundo script gestor

    // 4. MÉTODOS PRIVADOS DE ABSTRACCIÓN
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
    // FIN MÉTODOS DE ABSTRACCIÓN

    void Start()
    {
        // Llama al método de refresco para la carga inicial
        RefreshUIFromSettings();

        // Asignar listeners a los elementos de UI
        izqButton.onClick.AddListener(OnSelectLeftHanded);
        derButton.onClick.AddListener(OnSelectRightHanded);
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);

        // Añadir listeners para los sliders
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });
        brightnessSlider.onValueChanged.AddListener(delegate { OnBrightnessChanged(); });

        confirmButton.onClick.AddListener(OnConfirmChange);
        cancelConfirmationButton.onClick.AddListener(OnCancelChange);
    }
    void OnEnable()
    {
        // Esto garantiza que la UI siempre se sincronice al abrir el menú.
        RefreshUIFromSettings();
    }

    // ... (UpdateButtonColors, OnSelectLeftHanded, OnSelectRightHanded, OnVolumeChanged, OnBrightnessChanged se mantienen igual) ...

    private void UpdateButtonColors(bool isLeftHanded)
    {
        // Lógica de color de botones
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

    public void OnSelectLeftHanded()
    {
        currentIsLeftHanded = true;
        UpdateButtonColors(true);
    }

    public void OnSelectRightHanded()
    {
        currentIsLeftHanded = false;
        UpdateButtonColors(false);
    }

    public void OnVolumeChanged()
    {
        currentVolume = volumeSlider.value;
    }

    public void OnBrightnessChanged()
    {
        currentBrightness = brightnessSlider.value;
    }


    // Método de guardar (usa la abstracción)
    private void OnSaveButtonClicked()
    {
        if (currentIsLeftHanded != initialIsLeftHanded)
        {
            CallShow(MenuConfirmation); // Llama al Show del manager seleccionado
        }
        else
        {
            SaveSettingsToManager();
            CallShow(MenuSettings);
        }
    }

    // Método que guarda y aplica a través del Controlador Central
    private void SaveSettingsToManager()
    {
        // Llamada al Singleton:

        initialIsLeftHanded = currentIsLeftHanded;

        RefreshUIFromSettings();

        CallHideCurrent(); // Llama al HideCurrent del manager seleccionado
    }

    // Confirmar y Guardar
    private void OnConfirmChange()
    {
        SaveSettingsToManager();

        RefreshUIFromSettings();
    }

    // Cancelar la confirmación
    private void OnCancelChange()
    {
        RefreshUIFromSettings();
        CallShow(MenuInitial); // Llama al Show del manager seleccionado para volver al menú principal
    }

    // Cancelar el menú
    private void OnCancelButtonClicked()
    {
        // 1. Restaurar/Refrescar los valores del Singleton al menú
        RefreshUIFromSettings();

        // 2. Cerrar el menú visual
        gameObject.SetActive(false);
    }

    public void RefreshUIFromSettings()
    {
        // Usamos el Singleton para obtener los valores actuales
        currentVolume = SettingsManager.Instance.CurrentVolume;
        currentBrightness = SettingsManager.Instance.CurrentBrightness;
        currentIsLeftHanded = SettingsManager.Instance.CurrentIsLeftHanded;
        initialIsLeftHanded = currentIsLeftHanded;

        // Actualizar la interfaz de ESTE menú
        UpdateButtonColors(currentIsLeftHanded);

        // ESTO GARANTIZA QUE LOS SLIDERS REFLEJAN LOS VALORES GUARDADOS
        volumeSlider.value = currentVolume;
        brightnessSlider.value = currentBrightness;

        Debug.Log($"{gameObject.name} UI refreshed from current settings. Volume: {currentVolume}");
    }
}