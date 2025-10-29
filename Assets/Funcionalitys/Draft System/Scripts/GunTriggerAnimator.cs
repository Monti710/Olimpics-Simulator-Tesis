using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers; // XRInputValueReader

/// Anima el gatillo de una pistola rot�ndolo seg�n el valor del trigger (0..1).
public class GunTriggerAnimator : MonoBehaviour
{
    [Header("Referencia del gatillo")]
    [SerializeField] Transform m_TriggerTransform;

    [Header("Rango de rotaci�n en X (grados)")]
    [Tooltip("x = �ngulo con trigger suelto, y = �ngulo con trigger apretado")]
    [SerializeField] Vector2 m_TriggerXAxisRotationRange = new Vector2(0f, -15f);

    [Header("Lectura de entrada")]
    // Igual que en el sample: por defecto lee el canal "Trigger".
    // Si usas otro, c�mbialo en el Inspector o aqu� mismo.
    [SerializeField] XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");

    [Header("Opcionales")]
    [Tooltip("Suaviza el movimiento del gatillo (grados/seg)")]
    [SerializeField] float m_SmoothDegreesPerSec = 720f;
    [Tooltip("Curva de respuesta: 1 = lineal, >1 m�s duro al inicio, <1 m�s sensible al inicio")]
    [Range(0.25f, 3f)][SerializeField] float m_ResponsePower = 1f;
    [Tooltip("Invierte el valor del trigger (1 - v)")]
    [SerializeField] bool m_Invert;

    float m_CurrentAngle;

    void OnEnable()
    {
        if (m_TriggerTransform == null)
        {
            enabled = false;
            Debug.LogWarning($"[GunTriggerAnimator] Falta asignar m_TriggerTransform en {name}", this);
            return;
        }

        // Habilita la lectura directa si el Reader est� en modo "Direct Action".
        m_TriggerInput?.EnableDirectActionIfModeUsed();

        // Inicializa �ngulo actual coherente con la posici�n "suelto".
        m_CurrentAngle = m_TriggerXAxisRotationRange.x;
        ApplyAngle(m_CurrentAngle);
    }

    void OnDisable()
    {
        m_TriggerInput?.DisableDirectActionIfModeUsed();
    }

    void Update()
    {
        // 1) Leer valor del gatillo (0..1)
        var raw = m_TriggerInput != null ? Mathf.Clamp01(m_TriggerInput.ReadValue()) : 0f;
        if (m_Invert) raw = 1f - raw;

        // 2) Aplicar �curva� opcional (potencia)
        var t = (m_ResponsePower == 1f) ? raw : Mathf.Pow(raw, m_ResponsePower);

        // 3) Mapear a �ngulo
        var targetAngle = Mathf.Lerp(m_TriggerXAxisRotationRange.x, m_TriggerXAxisRotationRange.y, t);

        // 4) Suavizar y aplicar
        if (m_SmoothDegreesPerSec > 0f)
            m_CurrentAngle = Mathf.MoveTowards(m_CurrentAngle, targetAngle, m_SmoothDegreesPerSec * Time.deltaTime);
        else
            m_CurrentAngle = targetAngle;

        ApplyAngle(m_CurrentAngle);
    }

    void ApplyAngle(float angleX)
    {
        var e = m_TriggerTransform.localEulerAngles;
        m_TriggerTransform.localRotation = Quaternion.Euler(angleX, 0f, 0f);
    }
}
