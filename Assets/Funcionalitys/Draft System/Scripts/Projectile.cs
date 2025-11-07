using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    [Header("Vida del proyectil")]
    public float lifeSeconds = 5f;

    [Header("Impacto")]
    public GameObject impactPrefab;
    public float impactLifetime = 1.5f;
    public float impactScale = 1f; // 🔹 NUEVO: Tamaño del impacto ajustable en el Inspector
    public LayerMask hitMask = ~0; // Qué capas puede golpear (por defecto todas)

    Rigidbody rb;
    Collider col;
    TrailRenderer tr;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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

    void FixedUpdate()
    {
        if (WindManager.instance != null)
        {
            Vector3 windForce = WindManager.instance.windDirection.normalized * WindManager.instance.windStrength;
            rb.AddForce(windForce, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((hitMask.value & (1 << collision.collider.gameObject.layer)) == 0) return;

        var contact = collision.GetContact(0);

        if (impactPrefab)
        {
            var fx = Instantiate(
                impactPrefab,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );
            fx.transform.localScale *= impactScale;
            Destroy(fx, impactLifetime);
        }

        // 🔹 Crear un clon visual del Trail antes de destruir la bala
        if (tr != null)
        {
            // Crear un objeto vacío para mantener el rastro
            GameObject trailClone = new GameObject("TrailClone");
            var newTrail = trailClone.AddComponent<TrailRenderer>();

            // Copiar propiedades del original
            newTrail.time = 5f; // tiempo que permanece el rastro
            newTrail.material = tr.material;
            newTrail.colorGradient = tr.colorGradient;
            newTrail.widthCurve = tr.widthCurve;
            newTrail.widthMultiplier = tr.widthMultiplier;
            newTrail.alignment = tr.alignment;
            newTrail.textureMode = tr.textureMode;

            // Posicionar el clon donde terminó el proyectil
            trailClone.transform.position = transform.position;
            trailClone.transform.rotation = transform.rotation;

            // Destruir el clon después de 5 segundos
            Destroy(trailClone, 5f);
        }

        Destroy(gameObject);
    }

}
