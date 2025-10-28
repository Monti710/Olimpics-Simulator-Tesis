using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class UniqueWall : MonoBehaviour
{
    [Header("Componentes Asignables")]
    public PointCounter pointCounter;      // Asignar en el Inspector
    public BoxCollider boxCollider;        // Asignar el BoxCollider en el Inspector
    public TextMeshProUGUI feedbackText;   // AÑADIDO: TextView/Text para mostrar el puntaje

    [Header("Configuración de Feedback")]
    [Tooltip("Tiempo que el puntaje permanece visible en segundos.")]
    public float feedbackDisplayTime = 2f; // AÑADIDO: Tiempo de visualización configurable

    // ... (El resto de las variables se mantienen igual)
    private float[] radii = new float[11];   // Radios calculados automáticamente
    private int[] pointValues = new int[11]; // Asignar los puntos para cada radio en el Inspector

    private void Start()
    {
        // Asegurar que el collider NO es trigger
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;  // Confirmamos que el BoxCollider no sea un trigger
        }

        // Calcular automáticamente los radios
        CalculateCircleRadii();

        // AÑADIDO: Asegurar que el texto esté oculto al inicio
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    private void CalculateCircleRadii()
    {
        // ... (Tu lógica para calcular radios se mantiene igual)
        // Obtener el tamaño del BoxCollider (considerando la escala del objeto)
        float colliderWidth = boxCollider.size.z * boxCollider.transform.lossyScale.z;
        float colliderHeight = boxCollider.size.y * boxCollider.transform.lossyScale.y;

        // Calcular el radio máximo (mitad de la dimensión menor)
        float maxRadius = Mathf.Min(colliderWidth, colliderHeight) / 2f;
        float radiusStep = maxRadius / 11f;

        // Calcular los radios (de mayor a menor) y asignar los puntajes (1 a 11)
        for (int i = 0; i < 11; i++)
        {
            radii[i] = (i + 1) * radiusStep;

            // Asignar puntajes: el círculo más pequeño tiene el puntaje más alto (11) y el más grande tiene 1
            pointValues[i] = 11 - i;  // Puntaje creciente de 1 a 11
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar que el objeto que colisiona sea una bala
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Obtener el punto de impacto desde la colisión
            Vector3 impactPoint = collision.GetContact(0).point;

            // Verificar en qué círculo impactó
            int pointsToAdd = GetPointsForImpact(impactPoint);

            // Sumar los puntos al contador
            pointCounter.AddPoints(pointsToAdd);

            // AÑADIDO: Pasar los puntos a la función de retroalimentación
            GiveFeedback(pointsToAdd);

            // Destruir el proyectil después del impacto
            Destroy(collision.gameObject);
        }
    }

    private void GiveFeedback(int points) // MODIFICADO: Ahora recibe los puntos
    {
        if (feedbackText != null)
        {
            // 1. Mostrar el puntaje en el texto
            feedbackText.text = "+" + points.ToString();

            // 2. Activar el objeto de texto si no está activo
            feedbackText.gameObject.SetActive(true);

            // 3. Iniciar la corrutina para ocultar el texto
            StartCoroutine(HideFeedbackAfterDelay());
        }
    }

    // AÑADIDO: Corrutina para ocultar el texto después de un tiempo
    private System.Collections.IEnumerator HideFeedbackAfterDelay()
    {
        // Esperar el tiempo especificado en el Inspector
        yield return new WaitForSeconds(feedbackDisplayTime);

        // Ocultar el objeto de texto (desactivar)
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    // Método para determinar los puntos según el impacto
    private int GetPointsForImpact(Vector3 impactPoint)
    {
        // ... (Tu lógica para determinar los puntos se mantiene igual)
        int score = 0;

        // Obtener la posición real del centro del BoxCollider
        Vector3 boxCenter = boxCollider.bounds.center;

        // Calcular la distancia entre el punto de impacto y el centro del BoxCollider
        // Nota: Esto debería ser la distancia en el plano del objetivo si es un muro 2D/3D
        // Para precisión, podrías proyectar el impacto y el centro en el plano del muro.
        float distanceToCenter = Vector3.Distance(impactPoint, boxCenter);

        // Verificar en qué círculo impactó en base a la distancia al centro
        // Se asume que los radios están ordenados de menor a mayor.
        for (int i = 0; i < radii.Length; i++)
        {
            if (distanceToCenter <= radii[i])
            {
                score = pointValues[i];
                break;
            }
        }

        return score;
    }
}