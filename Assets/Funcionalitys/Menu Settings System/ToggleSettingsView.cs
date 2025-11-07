using UnityEngine;
using UISwitcherControl = UISwitcher.UISwitcher;
using TMPro;
// Si usas TextMeshPro, usa 'using TMPro;'

public class ToggleSettingsView : MonoBehaviour
{
    // Referencia al componente UISwitcher
    public UISwitcherControl bulletMarkSwitcher;
    public UISwitcherControl autoPointerSwitcher;

    // Campos de texto para mostrar el estado
    public TextMeshProUGUI bulletMarkStatusText;
    public TextMeshProUGUI autoPointerStatusText;

    // Campo del BulletMark (sin cambios)
    public TrailRenderer bulletMarkTargetComponent;

    // ⭐ NUEVOS CAMPOS: Dos LineRenderer para el AutoPointer ⭐
    public Behaviour autoPointerLine1;
    public Behaviour autoPointerLine2;

    // Constantes de texto
    private const string EnabledText = "Activado";
    private const string DisabledText = "Desactivado";
    private const string NullText = "Indeterminado";

    void Start()
    {
        // 1. Sincronizar UI al iniciar (una vez)
        RefreshUIFromSettings();

        // 2. Asignar Listeners a los eventos (sólo se dispara al hacer clic)
        bulletMarkSwitcher.onValueChangedNullable.AddListener(OnBulletMarkChanged);
        autoPointerSwitcher.onValueChangedNullable.AddListener(OnAutoPointerChanged);
    }

    void OnEnable()
    {
        RefreshUIFromSettings();
    }

    private void OnDisable()
    {
        RefreshUIFromSettings();
    }

    public void RefreshUIFromSettings()
    {
        if (SettingsManager.Instance == null) return;

        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        bulletMarkSwitcher.SetWithoutNotify(bulletMark);
        autoPointerSwitcher.SetWithoutNotify(autoPointer);

        UpdateStatusText(bulletMarkStatusText, bulletMark);
        UpdateStatusText(autoPointerStatusText, autoPointer);

        // Sincronizar componentes al cargar
        SetBulletMarkComponentState(bulletMark);
        // ⭐ NUEVA LLAMADA: Sincronizar LineRenderers del AutoPointer ⭐
        SetAutoPointerComponentsState(autoPointer);
    }

    private void OnBulletMarkChanged(bool? newValue)
    {
        UpdateStatusText(bulletMarkStatusText, newValue);

        if (!newValue.HasValue) return;

        SetBulletMarkComponentState(newValue.Value);

        UpdateSettingsBuffer(newValue.Value, SettingsManager.Instance.CurrentAutoPointer);
    }

    // ⭐ Manejador de evento del AutoPointer Switcher MODIFICADO ⭐
    private void OnAutoPointerChanged(bool? newValue)
    {
        UpdateStatusText(autoPointerStatusText, newValue);

        if (!newValue.HasValue) return;

        // ⭐ LLAMADA DE ACCIÓN: Activar/Desactivar los LineRenderers ⭐
        SetAutoPointerComponentsState(newValue.Value);

        UpdateSettingsBuffer(SettingsManager.Instance.CurrentBulletMark, newValue.Value);
    }

    // --- Métodos de Control de Componentes ---

    private void SetBulletMarkComponentState(bool isEnabled)
    {
        if (bulletMarkTargetComponent != null)
        {
            bulletMarkTargetComponent.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning("bulletMarkTargetComponent no ha sido asignado.");
        }
    }

    // ⭐ NUEVO MÉTODO DEDICADO PARA LOS LINE RENDERERS DEL AUTO POINTER ⭐
    private void SetAutoPointerComponentsState(bool isEnabled)
    {
        // Activar/Desactivar el primer LineRenderer
        if (autoPointerLine1 != null)
        {
            autoPointerLine1.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning("AutoPointerLine1 no ha sido asignado.");
        }

        // Activar/Desactivar el segundo LineRenderer
        if (autoPointerLine2 != null)
        {
            autoPointerLine2.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning("AutoPointerLine2 no ha sido asignado.");
        }
    }

    // --- Métodos de Utilidad (sin cambios) ---

    private void UpdateStatusText(TextMeshProUGUI targetText, bool? value)
    {
        if (targetText == null) return;

        if (value.HasValue)
        {
            targetText.text = value.Value ? EnabledText : DisabledText;
        }
        else
        {
            targetText.text = NullText;
        }
    }

    private void UpdateSettingsBuffer(bool newBulletMark, bool newAutoPointer)
    {
        float volume = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool isLeftHanded = SettingsManager.Instance.CurrentIsLeftHanded;
        bool userInterface = SettingsManager.Instance.CurrentUserInterface;
        bool visualObjects = SettingsManager.Instance.CurrentVisualObjects;

        SettingsManager.Instance.UpdateInMemorySettings(
            volume, sound, brightness, isLeftHanded, newBulletMark, newAutoPointer, userInterface, visualObjects);
    }
}