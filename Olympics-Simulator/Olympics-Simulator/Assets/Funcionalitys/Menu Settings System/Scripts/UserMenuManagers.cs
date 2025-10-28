using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gestiona múltiples menús de VR (activación/desactivación por clave, seguimiento de cabeza y control de Input por preferencia de mano).
/// </summary>
public class UserMenuManagers : MonoBehaviour
{
    // *** Estructura y Mapeo de Menús (Lógica similar a MenuManager.cs)
    [Serializable]
    public struct MenuEntry { public string key; public GameObject root; }

    [Header("Menu Collection")]
    [Tooltip("Lista de menús que pueden ser activados por clave.")]
    [SerializeField] private MenuEntry[] menus;

    [Tooltip("Clave del menú que se abre al pulsar el botón de toggle (ej: 'MainSettings').")]
    [SerializeField] private string primaryToggleKey = "Settings";

    // Diccionario para acceso rápido a los GameObjects
    private Dictionary<string, GameObject> menuMap;
    private GameObject currentMenuRoot; // El menú actualmente activo

    // *** Configuración de VR y Seguimiento (Lógica de VRSettingsMenu.cs)
    [Header("VR Settings")]
    public Transform headTransform;    // Cámara del jugador (XR Origin -> Main Camera)
    public float menuDistance = 1.5f;  // Distancia frente al jugador

    [Header("Input Actions")]
    public InputActionReference leftPrimaryAction;  // Botón Primario (A/X) del controlador izquierdo
    public InputActionReference rightPrimaryAction; // Botón Primario (A/X) del controlador derecho

    private InputAction currentToggleAction; // Acción seleccionada para abrir/cerrar

    // ----------------------------------------------------------------------

    void Awake()
    {
        // 1. Inicializar el mapeo de menús (Similar a MenuManager.Awake)
        menuMap = menus.Where(e => e.root != null)
                     .ToDictionary(e => e.key, e => e.root);

        // Desactivar todos los menús al inicio
        foreach (var go in menuMap.Values) go.SetActive(false);

        // 2. Asignar HeadTransform por defecto si no está asignado
        if (headTransform == null)
        {
            headTransform = Camera.main.transform;
        }

        // 3. Configurar la acción de entrada según PlayerPrefs
        SetToggleInputAction();
    }

    // --- Gestión de Input Actions ---
    void OnEnable()
    {
        // Habilitar la acción seleccionada y suscribirse
        if (currentToggleAction != null)
        {
            currentToggleAction.Enable();
            currentToggleAction.performed += OnPrimaryButtonPressed;
        }
    }

    void OnDisable()
    {
        // Deshabilitar y desuscribirse
        if (currentToggleAction != null)
        {
            currentToggleAction.performed -= OnPrimaryButtonPressed;
            currentToggleAction.Disable();
        }
    }

    private void OnPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // El botón 'Primary' siempre alternará el menú principal
            TogglePrimaryMenu();
        }
    }

    // --- Lógica de Seguimiento de Cabeza (VRSettingsMenu.FollowHead modificado) ---
    void Update()
    {
        // Si hay un menú activo (currentMenuRoot != null), lo seguimos
        if (currentMenuRoot != null)
            FollowHead();
    }

    /// <summary>
    /// Posiciona y orienta el menú actual frente al jugador.
    /// </summary>
    void FollowHead()
    {
        if (headTransform == null || currentMenuRoot == null) return;

        Transform menuTransform = currentMenuRoot.transform;

        // Posición: frente al jugador
        Vector3 forward = headTransform.forward;
        forward.y = 0; // Opcional: mantener altura constante
        forward.Normalize();
        menuTransform.position = headTransform.position + forward * menuDistance;

        // Rotación: mirar hacia el jugador
        Vector3 lookDir = menuTransform.position - headTransform.position;
        lookDir.y = 0; // Rotar solo en Y
        menuTransform.rotation = Quaternion.LookRotation(lookDir);
    }

    // --- Métodos Públicos de Gestión de Menús (Lógica de MenuManager.Show) ---

    /// <summary>
    /// Alterna el menú principal (PrimaryToggleKey).
    /// </summary>
    public void TogglePrimaryMenu()
    {
        if (currentMenuRoot != null)
        {
            // Si hay un menú abierto, lo cerramos
            HideCurrent();
        }
        else
        {
            // Si no hay menú abierto, abrimos el principal
            if (!string.IsNullOrEmpty(primaryToggleKey))
            {
                Show(primaryToggleKey);
            }
        }
    }

    /// <summary>
    /// Muestra un menú por su clave, cerrando el menú actual si está abierto.
    /// </summary>
    /// <param name="key">La clave del menú a abrir.</param>
    public void Show(string key)
    {
        if (!menuMap.TryGetValue(key, out var targetMenu)) return;
        if (currentMenuRoot == targetMenu) return; // Ya está activo

        // 1. Cerrar el menú anterior
        if (currentMenuRoot)
        {
            currentMenuRoot.SetActive(false);
        }

        // 2. Establecer y Abrir el nuevo menú
        currentMenuRoot = targetMenu;
        currentMenuRoot.SetActive(true);

        // 3. Colocar el menú inmediatamente para el inicio del seguimiento
        FollowHead();
    }

    /// <summary>
    /// Oculta el menú actualmente activo.
    /// </summary>
    public void HideCurrent()
    {
        if (!currentMenuRoot) return;

        currentMenuRoot.SetActive(false);
        currentMenuRoot = null;
    }

    // --- Lógica de Preferencia de Mano (VRSettingsMenu.SetInputAction modificado) ---

    /// <summary>
    /// Configura la InputAction activa basándose en PlayerPrefs["IsLeftHanded"].
    /// </summary>
    public void SetToggleInputAction()
    {
        // 1. Deshabilitar la acción anterior y desuscribirse
        if (currentToggleAction != null)
        {
            currentToggleAction.performed -= OnPrimaryButtonPressed;
            currentToggleAction.Disable();
            currentToggleAction = null;
        }

        // 2. Determinar la acción a usar
        int isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0); // 0 es diestro, 1 es zurdo

        if (isLeftHanded == 1) // Zurdo
        {
            // El zurdo usa el botón del controlador derecho para el menú
            currentToggleAction = rightPrimaryAction?.action;
        }
        else // Diestro
        {
            // El diestro usa el botón del controlador izquierdo para el menú
            currentToggleAction = leftPrimaryAction?.action;
        }

        // 3. Habilitar la nueva acción y suscribirse (si el script está activo)
        if (currentToggleAction != null && isActiveAndEnabled)
        {
            currentToggleAction.Enable();
            currentToggleAction.performed += OnPrimaryButtonPressed;
        }
    }

    /// <summary>
    /// Método público para actualizar la configuración de Input, por ejemplo, cuando se cambia la preferencia de mano desde un menú de opciones.
    /// </summary>
    public void UpdateInputConfiguration()
    {
        SetToggleInputAction();
    }
}