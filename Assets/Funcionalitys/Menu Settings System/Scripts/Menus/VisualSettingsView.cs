using UnityEngine;
using UISwitcherControl = UISwitcher.UISwitcher;
using TMPro;

public class VisualSettingsView : MonoBehaviour
{
    // Switchers
    public UISwitcherControl userInterfaceSwitcher;
    public UISwitcherControl visualObjectsSwitcher;

    // Textos de estado
    public TextMeshProUGUI userInterfaceStatusText;
    public TextMeshProUGUI visualObjectsStatusText;

    // Objetos visuales controlados
    public GameObject userInterfaceTargetObject;
    public GameObject visualObjectsTargetObject;

    // Constantes de texto
    private const string EnabledText = "Activado";
    private const string DisabledText = "Desactivado";
    private const string NullText = "Indeterminado";

    void Start()
    {
        RefreshUIFromSettings();

        userInterfaceSwitcher.onValueChangedNullable.AddListener(OnUserInterfaceChanged);
        visualObjectsSwitcher.onValueChangedNullable.AddListener(OnVisualObjectsChanged);
    }

    void OnEnable()
    {
        RefreshUIFromSettings();
    }

    public void RefreshUIFromSettings()
    {
        if (SettingsManager.Instance == null) return;

        bool userInterface = SettingsManager.Instance.CurrentUserInterface;
        bool visualObjects = SettingsManager.Instance.CurrentVisualObjects;

        userInterfaceSwitcher.SetWithoutNotify(userInterface);
        visualObjectsSwitcher.SetWithoutNotify(visualObjects);

        UpdateStatusText(userInterfaceStatusText, userInterface);
        UpdateStatusText(visualObjectsStatusText, visualObjects);

        SetUserInterfaceObjectState(userInterface);
        SetVisualObjectsObjectState(visualObjects);
    }

    private void OnUserInterfaceChanged(bool? newValue)
    {
        UpdateStatusText(userInterfaceStatusText, newValue);
        if (!newValue.HasValue) return;

        SetUserInterfaceObjectState(newValue.Value);
        UpdateSettingsBuffer(newValue.Value, SettingsManager.Instance.CurrentVisualObjects);
    }

    private void OnVisualObjectsChanged(bool? newValue)
    {
        UpdateStatusText(visualObjectsStatusText, newValue);
        if (!newValue.HasValue) return;

        SetVisualObjectsObjectState(newValue.Value);
        UpdateSettingsBuffer(SettingsManager.Instance.CurrentUserInterface, newValue.Value);
    }

    private void SetUserInterfaceObjectState(bool isEnabled)
    {
        if (userInterfaceTargetObject != null)
            userInterfaceTargetObject.SetActive(isEnabled);
        else
            Debug.LogWarning("userInterfaceTargetObject no ha sido asignado.");
    }

    private void SetVisualObjectsObjectState(bool isEnabled)
    {
        if (visualObjectsTargetObject != null)
            visualObjectsTargetObject.SetActive(isEnabled);
        else
            Debug.LogWarning("visualObjectsTargetObject no ha sido asignado.");
    }

    private void UpdateStatusText(TextMeshProUGUI targetText, bool? value)
    {
        if (targetText == null) return;
        targetText.text = value.HasValue ? (value.Value ? EnabledText : DisabledText) : NullText;
    }

    private void UpdateSettingsBuffer(bool newUserInterface, bool newVisualObjects)
    {
        float volume = SettingsManager.Instance.CurrentVolume;
        float sound = SettingsManager.Instance.CurrentSound;
        float brightness = SettingsManager.Instance.CurrentBrightness;
        bool isLeftHanded = SettingsManager.Instance.CurrentIsLeftHanded;
        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;

        SettingsManager.Instance.UpdateInMemorySettings(
            volume, sound, brightness, isLeftHanded,
            bulletMark, autoPointer, newUserInterface, newVisualObjects);
    }
}
