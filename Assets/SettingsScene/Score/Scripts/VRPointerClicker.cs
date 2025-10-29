using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class VRPointerClicker : MonoBehaviour
{
    public InputActionReference triggerAction; // Acción para el botón trigger
    public Transform rayOrigin; // Punto de origen del rayo (controlador o mano)
    public float maxDistance = 5f;
    public LayerMask uiLayerMask; // Capa donde están las teclas

    private void OnEnable()
    {
        if (triggerAction != null)
            triggerAction.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        if (triggerAction != null)
            triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, uiLayerMask))
        {
            // Intentar obtener componente Button del objeto tocado
            Button btn = hit.collider.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.Invoke();
            }
        }
    }
}
