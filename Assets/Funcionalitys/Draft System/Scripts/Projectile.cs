using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    // AÑADIDO: Referencia al TrailRenderer para ajustes si fuera necesario, 
    // aunque en este caso solo se requiere que el componente exista.
    // Asegúrate de que el componente TrailRenderer esté configurado en el Prefab del proyectil.
    // [Header("Efecto de rastro")] 
    // public TrailRenderer trailRenderer; // Se puede añadir si quieres manipularlo desde código, 
    // pero si solo quieres la estela, con el RequireComponent y la configuración en el prefab es suficiente.

    [Header("Vida del proyectil")]
    public float lifeSeconds = 5f;

    [Header("Impacto")]
    public GameObject impactPrefab;
    public float impactLifetime = 1.5f;
    public LayerMask hitMask = ~0;  // qué capas puede golpear (por defecto todas)

    Rigidbody rb;
    Collider col;
    // AÑADIDO: TrailRenderer
    TrailRenderer tr;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        // AÑADIDO: Obtener el componente TrailRenderer
        tr = GetComponent<TrailRenderer>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        col.isTrigger = false;
    }

    void OnEnable()
    {
        // Reiniciar el rastro al ser activado (útil para pooling de objetos)
        if (tr != null) tr.Clear();

        // Autodestruir si no golpea nada
        if (lifeSeconds > 0f) Destroy(gameObject, lifeSeconds);
    }

    /// <summary> Asigna la velocidad inicial y (opcional) ignora colisiones con el "owner". </summary>
    public void Launch(Vector3 velocity, Transform owner = null)
    {
        rb.linearVelocity = velocity;

        if (owner != null)
        {
            var ownerCols = owner.GetComponentsInChildren<Collider>();
            foreach (var oc in ownerCols)
            {
                if (oc && col) Physics.IgnoreCollision(col, oc, true);
            }
        }
    }

    // --- CÓDIGO AÑADIDO PARA EL VIENTO ---
    // FixedUpdate se ejecuta en cada paso del motor de físicas.
    void FixedUpdate()
    {
        // Verificamos si existe un WindManager en la escena.
        if (WindManager.instance != null)
        {
            // Calculamos la fuerza del viento.
            Vector3 windForce = WindManager.instance.windDirection.normalized * WindManager.instance.windStrength;

            // Aplicamos la fuerza del viento al Rigidbody de la bala.
            // Usamos ForceMode.Acceleration para ignorar la masa de la bala y que el efecto sea consistente.
            rb.AddForce(windForce, ForceMode.Acceleration);
        }
    }
    // --- FIN DEL CÓDIGO AÑADIDO ---

    void OnCollisionEnter(Collision collision)
    {
        // Filtrado por capas
        if ((hitMask.value & (1 << collision.collider.gameObject.layer)) == 0) return;

        // Punto y normal de impacto
        var contact = collision.GetContact(0);
        if (impactPrefab)
        {
            var fx = Instantiate(
                impactPrefab,
                contact.point,
                Quaternion.LookRotation(contact.normal) // orienta el efecto hacia la superficie
            );
            Destroy(fx, impactLifetime);
        }

        Destroy(gameObject);
    }
}