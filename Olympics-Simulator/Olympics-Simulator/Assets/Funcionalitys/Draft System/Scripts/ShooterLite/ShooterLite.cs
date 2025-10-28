using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShooterLite : MonoBehaviour
{
    [Header("Input (XRI Right Hand)")]
    public InputActionProperty triggerAction; // XRI RightHand / Activate O Activate Value

    [Header("Disparo")]
    public Projectile projectilePrefab; // Prefab del proyectil
    public Transform muzzle; // Puntero de salida del proyectil
    public float projectileSpeed = 40f; // Velocidad del proyectil
    public float fireCooldown = 0.15f; // Cooldown entre disparos
    public bool autoFireWhileHeld = false; // Disparo continuo si está activado
    [Range(0f, 1f)] public float pressThreshold = 0.5f; // Umbral para activar el gatillo

    [Header("Colisiones")]
    public Transform ignoreCollisionsRoot; // Normalmente XR Origin (XR Rig)

    [Header("Audio")]
    public AudioSource audioSource; // Fuente de audio para el disparo
    public AudioClip fireClip; // Sonido del disparo
    public float fireVolume = 1f; // Volumen del sonido
    public Vector2 pitchJitter = new Vector2(0.98f, 1.02f); // Variación del pitch del sonido

    [Header("Vibración del mando")]
    public float vibrationIntensity = 0.5f; // Intensidad de la vibración (0 a 1)
    public float vibrationDuration = 0.1f; // Duración de la vibración

    private float _cooldown;
    private bool _wasPressed;

    void OnEnable()
    {
        triggerAction.action?.Enable(); // Activa la acción del trigger
    }

    void OnDisable()
    {
        triggerAction.action?.Disable(); // Desactiva la acción del trigger
    }

    void Update()
    {
        _cooldown -= Time.deltaTime; // Resta el tiempo del cooldown

        // Lee el valor del trigger (0 a 1)
        float val = triggerAction.action?.ReadValue<float>() ?? 0f;
        bool pressed = val >= pressThreshold; // El gatillo está presionado si el valor es mayor al umbral

        bool shouldFire = autoFireWhileHeld
            ? (pressed && _cooldown <= 0f) // Disparo automático si está activado
            : (pressed && !_wasPressed && _cooldown <= 0f); // Disparo único si no se estaba presionando

        if (shouldFire)
        {
            Fire(); // Llamamos al método Fire cuando se debe disparar
            _cooldown = fireCooldown; // Reinicia el cooldown
        }

        _wasPressed = pressed; // Guardamos el estado actual del gatillo
    }

    void Fire()
    {
        if (!projectilePrefab || !muzzle) return; // Verifica que tengamos el prefab y la posición del cañón

        // Instancia el proyectil y lo lanza
        var proj = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        proj.Launch(muzzle.forward * projectileSpeed, ignoreCollisionsRoot); // Lanza el proyectil

        // Reproduce el sonido del disparo
        if (audioSource && fireClip)
        {
            audioSource.pitch = Random.Range(pitchJitter.x, pitchJitter.y); // Variación en el pitch
            audioSource.PlayOneShot(fireClip, fireVolume); // Reproduce el sonido
        }

        // Vibración en el mando (Right Hand) usando XRBaseInputInteractor
        XRBaseInputInteractor controllerInteractor = GetComponentInParent<XRBaseInputInteractor>(); // Obtiene el XRBaseInputInteractor de la mano derecha
        if (controllerInteractor != null)
        {
            controllerInteractor.SendHapticImpulse(vibrationIntensity, vibrationDuration); // Envía la vibración
        }
    }
}
