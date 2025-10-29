using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WindAffected : MonoBehaviour
{
    private Rigidbody rb;

    [Tooltip("Multiplicador para que algunos objetos sean más o menos afectados por el viento.")]
    public float windSensitivity = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (WindManager.instance != null)
        {
            // La misma lógica que en la bala, pero con un multiplicador de sensibilidad.
            Vector3 windForce = WindManager.instance.windDirection.normalized * WindManager.instance.windStrength * windSensitivity;

            rb.AddForce(windForce, ForceMode.Acceleration);
        }
    }
}