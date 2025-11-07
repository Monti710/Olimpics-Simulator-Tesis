using UnityEngine;
using System.Collections; // Necesario para la Coroutine

/// <summary>
/// Aplica TODA la configuración guardada (visual, controles, etc.)
/// al inicio del juego, tan pronto como el SettingsManager esté listo.
/// </summary>
public class InitialSettingsApplier : MonoBehaviour
{
    [Header("Objetos de 'VisualSettingsView'")]
    [Tooltip("El objeto principal de la UI del juego.")]
    public GameObject userInterfaceTargetObject;

    [Tooltip("El objeto para los elementos visuales extra.")]
    public GameObject visualObjectsTargetObject;

    // --- NUEVOS CAMPOS ---
    [Header("Objetos de 'ToggleSettingsView'")]
    [Tooltip("El TrailRenderer para las marcas de bala.")]
    public TrailRenderer bulletMarkTargetComponent;

    [Tooltip("El primer LineRenderer (o Behaviour) del auto-puntero.")]
    public Behaviour autoPointerLine1;

    [Tooltip("El segundo LineRenderer (o Behaviour) del auto-puntero.")]
    public Behaviour autoPointerLine2;
    // --- FIN DE NUEVOS CAMPOS ---


    void Start()
    {
        // Iniciamos una coroutine para asegurarnos de que el SettingsManager
        // ya se ha cargado (evita errores de 'null' en el arranque).
        StartCoroutine(ApplySettingsWhenReady());
    }

    private IEnumerator ApplySettingsWhenReady()
    {
        // 1. Espera en un bucle CADA FOTOGRAMA hasta que SettingsManager.Instance ya no sea null
        while (SettingsManager.Instance == null)
        {
            yield return null; // Espera al siguiente fotograma
        }

        // --- El Manager está listo, ahora aplicamos TODA la configuración ---

        // 2. Aplicar config de 'VisualSettingsView'
        ApplyVisualSettings();

        // 3. Aplicar config de 'ToggleSettingsView'
        ApplyToggleSettings();
    }

    /// <summary>
    /// Aplica la configuración de los objetos de VisualSettingsView
    /// </summary>
    private void ApplyVisualSettings()
    {
        if (userInterfaceTargetObject != null)
        {
            userInterfaceTargetObject.SetActive(SettingsManager.Instance.CurrentUserInterface);
        }
        else
        {
            Debug.LogWarning("InitialSettingsApplier: 'userInterfaceTargetObject' no está asignado.", this);
        }

        if (visualObjectsTargetObject != null)
        {
            visualObjectsTargetObject.SetActive(SettingsManager.Instance.CurrentVisualObjects);
        }
        else
        {
            Debug.LogWarning("InitialSettingsApplier: 'visualObjectsTargetObject' no está asignado.", this);
        }
    }

    /// <summary>
    /// Aplica la configuración de los componentes de ToggleSettingsView
    /// </summary>
    private void ApplyToggleSettings()
    {
        // Aplicar Bullet Mark
        bool bulletMark = SettingsManager.Instance.CurrentBulletMark;
        if (bulletMarkTargetComponent != null)
        {
            bulletMarkTargetComponent.enabled = bulletMark; //
        }
        else
        {
            Debug.LogWarning("InitialSettingsApplier: 'bulletMarkTargetComponent' no está asignado.", this);
        }

        // Aplicar Auto Pointer
        bool autoPointer = SettingsManager.Instance.CurrentAutoPointer;
        if (autoPointerLine1 != null)
        {
            autoPointerLine1.enabled = autoPointer; //
        }
        else
        {
            Debug.LogWarning("InitialSettingsApplier: 'autoPointerLine1' no está asignado.", this);
        }

        if (autoPointerLine2 != null)
        {
            autoPointerLine2.enabled = autoPointer; //
        }
        else
        {
            Debug.LogWarning("InitialSettingsApplier: 'autoPointerLine2' no está asignado.", this);
        }
    }
}