using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit; // Necesario para la clase base del controlador

public class WindTrajectoryPredictor : MonoBehaviour
{
    [Header("Input Actions para el Predictor")]
    [Tooltip("Acción para activar la mira/predictor en la mano derecha (Botón lateral/Grip).")]
    public InputActionProperty rightPredictorAction;
    [Tooltip("Acción para activar la mira/predictor en la mano izquierda (Botón lateral/Grip).")]
    public InputActionProperty leftPredictorAction;

    [Header("Referencias de Disparo")]
    [Tooltip("El punto de salida del proyectil (Muzzle/Cañón).")]
    public Transform muzzleTransform;
    [Tooltip("La velocidad inicial del proyectil.")]
    public float projectileSpeed = 40f;

    [Header("Visualización de Trayectoria 🔮")]
    [Tooltip("Referencia al LineRenderer para dibujar la trayectoria.")]
    public LineRenderer trajectoryLine; // DEBE ASIGNARSE
    [Tooltip("Sensibilidad al viento que se aplica a la simulación (asume el valor del proyectil).")]
    public float projectileWindSensitivity = 1.0f;
    [Tooltip("Número de segmentos de la línea de trayectoria para la simulación.")]
    public int trajectorySegments = 20;
    [Tooltip("Intervalo de tiempo entre cada paso de la simulación.")]
    public float simulationTimeStep = 0.05f;

    private bool _isLeftHanded; // Caché del PlayerPref

    void Awake()
    {
        // 1. Verificar la preferencia de mano dominante (0=Derecha, 1=Izquierda)
        _isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0) == 1;

        // 2. Inicializar LineRenderer
        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = trajectorySegments;
            trajectoryLine.enabled = false;
        }
        else
        {
            Debug.LogError("El LineRenderer para la predicción de trayectoria NO está asignado. La ayuda visual no funcionará.", this);
        }

        // 3. Verificar referencias críticas
        if (muzzleTransform == null)
        {
            Debug.LogError("El Muzzle Transform (punto de salida de la bala) NO está asignado.", this);
        }
    }

    void OnEnable()
    {
        rightPredictorAction.action?.Enable();
        leftPredictorAction.action?.Enable();
    }

    void OnDisable()
    {
        rightPredictorAction.action?.Disable();
        leftPredictorAction.action?.Disable();
    }

    void Update()
    {
        if (trajectoryLine == null || muzzleTransform == null) return;

        // 4. Leer el input del botón lateral según la mano dominante
        bool predictorPressed = _isLeftHanded
            ? leftPredictorAction.action?.ReadValue<float>() > 0.1f // Si zurdo, lee el izquierdo
            : rightPredictorAction.action?.ReadValue<float>() > 0.1f; // Si diestro, lee el derecho

        // 5. Gestionar la visualización
        trajectoryLine.enabled = predictorPressed;

        if (predictorPressed)
        {
            DrawTrajectory();
        }
    }

    // 6. Simulación de Trayectoria Balística
    void DrawTrajectory()
    {
        // Se asegura de que el WindManager exista
        if (WindManager.instance == null) return;

        Vector3 startPosition = muzzleTransform.position;
        Vector3 currentVelocity = muzzleTransform.forward * projectileSpeed;
        Vector3 currentPosition = startPosition;

        // Calcula la fuerza total del viento a aplicar
        Vector3 windForce = WindManager.instance.windDirection.normalized * WindManager.instance.windStrength * projectileWindSensitivity;

        trajectoryLine.SetPosition(0, startPosition);

        for (int i = 1; i < trajectorySegments; i++)
        {
            // F = ma, donde 'a' es la aceleración
            Vector3 acceleration = Physics.gravity + windForce;

            // Integración de Euler (Simple) para la simulación física:
            currentVelocity += acceleration * simulationTimeStep; // Nueva Velocidad
            Vector3 nextPosition = currentPosition + currentVelocity * simulationTimeStep; // Nueva Posición

            // Búsqueda de colisión entre el punto actual y el siguiente
            if (Physics.Linecast(currentPosition, nextPosition, out RaycastHit hit))
            {
                // Si colisiona, acorta la línea y termina
                trajectoryLine.positionCount = i + 1;
                trajectoryLine.SetPosition(i, hit.point);
                return;
            }

            // Si no colisiona, actualiza y establece el punto
            currentPosition = nextPosition;
            trajectoryLine.SetPosition(i, nextPosition);
        }

        // Asegura que todos los segmentos se dibujen si no hay colisión
        trajectoryLine.positionCount = trajectorySegments;
    }
}