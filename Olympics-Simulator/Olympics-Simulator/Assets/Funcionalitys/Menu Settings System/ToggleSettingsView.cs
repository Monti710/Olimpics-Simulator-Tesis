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

    // ⭐ NUEVO CAMPO: Componente a Activar/Desactivar ⭐
    // Usamos 'Behaviour' para poder asignar cualquier script, Renderer, Light, Collider, etc.
    public TrailRenderer bulletMarkTargetComponent;

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

    public void RefreshUIFromSettings()
    {
        if (SettingsManager.Instance == null) return;

        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        bulletMarkSwitcher.SetWithoutNotify(bulletMark);
        autoPointerSwitcher.SetWithoutNotify(autoPointer);

        UpdateStatusText(bulletMarkStatusText, bulletMark);
        UpdateStatusText(autoPointerStatusText, autoPointer);

        // ⭐ LÓGICA DEL COMPONENTE: Sincronizar estado del componente al abrir el menú ⭐
        SetBulletMarkComponentState(bulletMark);
    }

    private void OnBulletMarkChanged(bool? newValue)
    {
        UpdateStatusText(bulletMarkStatusText, newValue);

        if (!newValue.HasValue) return;

        // ⭐ LÓGICA DEL COMPONENTE: Activar/Desactivar inmediatamente al hacer clic ⭐
        SetBulletMarkComponentState(newValue.Value);

        UpdateSettingsBuffer(newValue.Value, SettingsManager.Instance.CurrentAutoPointer);
    }

    private void OnAutoPointerChanged(bool? newValue)
    {
        UpdateStatusText(autoPointerStatusText, newValue);

        if (!newValue.HasValue) return;

        UpdateSettingsBuffer(SettingsManager.Instance.CurrentBulletMark, newValue.Value);
    }

    // ⭐ MÉTODO DEDICADO PARA MANEJAR EL ESTADO DEL COMPONENTE ⭐
    private void SetBulletMarkComponentState(bool isEnabled)
    {
        if (bulletMarkTargetComponent != null)
        {
            // La propiedad 'enabled' activa/desactiva la funcionalidad del componente.
            bulletMarkTargetComponent.enabled = isEnabled;
            // Si quieres activar/desactivar todo el GameObject, usarías: 
            // bulletMarkTargetComponent.gameObject.SetActive(isEnabled);
        }
        else
        {
            Debug.LogWarning("bulletMarkTargetComponent no ha sido asignado en el Inspector.");
        }
    }

    // ... (El resto de los métodos UpdateStatusText y UpdateSettingsBuffer permanecen sin cambios) ...
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
        // Obtener TODOS los demás valores del buffer de memoria
        float volume = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool isLeftHanded = SettingsManager.Instance.CurrentIsLeftHanded;

        // Llamar a UpdateInMemorySettings con todos los valores
        SettingsManager.Instance.UpdateInMemorySettings(
            volume, sound, brightness, isLeftHanded, newBulletMark, newAutoPointer);
    }
}