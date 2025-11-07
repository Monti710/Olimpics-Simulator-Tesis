using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class UniqueWall : MonoBehaviour
{
    [Header("Componentes Asignables")]
    public PointCounter pointCounter;      // Asignar en el Inspector
    public BoxCollider boxCollider;        // Asignar el BoxCollider en el Inspector
    public TextMeshProUGUI feedbackText;   // Texto para mostrar el puntaje

    [Header("Configuración de Feedback")]
    [Tooltip("Tiempo que el puntaje permanece visible en segundos.")]
    public float feedbackDisplayTime = 2f;

    private float[] radii = new float[11];   // Radios calculados automáticamente
    private int[] pointValues = new int[11]; // Asignar los puntos para cada radio en el Inspector

    private void Start()
    {
        // Asegurar que el collider NO es trigger
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }

        // Calcular automáticamente los radios
        CalculateCircleRadii();

        // Asegurar que el texto esté oculto al inicio
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    private void CalculateCircleRadii()
    {
        // Obtener el tamaño real del BoxCollider (considerando la escala del objeto)
        float colliderWidth = boxCollider.size.z * boxCollider.transform.lossyScale.z;
        float colliderHeight = boxCollider.size.y * boxCollider.transform.lossyScale.y;

        // Calcular el radio máximo (mitad de la dimensión menor)
        float maxRadius = Mathf.Min(colliderWidth, colliderHeight) / 2f;

        // Cada paso representará 1/10 del radio máximo
        float radiusStep = maxRadius / 10f;

        // Redimensionamos los arreglos para incluir el círculo extra
        radii = new float[11];
        pointValues = new int[11];

        // Crear los 10 círculos normales (del valor 10 al 1)
        for (int i = 0; i < 10; i++)
        {
            radii[i] = (i + 1) * radiusStep;
            pointValues[i] = 10 - i; // 10, 9, 8... 1
        }

        // Crear el círculo adicional más pequeño dentro del de valor 10
        // Este círculo tiene 0.45 del tamaño del círculo más pequeño (valor 10)
        radii[10] = radii[0] * 0.45f;
        pointValues[10] = 11;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar que el objeto que colisiona sea una bala
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Obtener el punto de impacto desde la colisión
            Vector3 impactPoint = collision.GetContact(0).point;

            // Determinar en qué círculo impactó
            int pointsToAdd = GetPointsForImpact(impactPoint);

            // Sumar los puntos al contador
            pointCounter.AddPoints(pointsToAdd);

            // Mostrar retroalimentación visual
            GiveFeedback(pointsToAdd);

            // Destruir el proyectil después del impacto
            Destroy(collision.gameObject);
        }
    }

    private void GiveFeedback(int points)
    {
        if (feedbackText != null)
        {
            feedbackText.text = "+" + points.ToString();
            feedbackText.gameObject.SetActive(true);
            StartCoroutine(HideFeedbackAfterDelay());
        }
    }

    private System.Collections.IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDisplayTime);

        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    private int GetPointsForImpact(Vector3 impactPoint)
    {
        int score = 0;

        // Obtener el centro del BoxCollider
        Vector3 boxCenter = boxCollider.bounds.center;

        // Calcular la distancia entre el punto de impacto y el centro
        float distanceToCenter = Vector3.Distance(impactPoint, boxCenter);

        // 🔹 ORDENAR los radios y valores antes de evaluar
        System.Array.Sort(radii, pointValues);

        // 🔹 Ahora evaluamos de menor a mayor
        for (int i = 0; i < radii.Length; i++)
        {
            if (distanceToCenter <= radii[i])
            {
                score = pointValues[i];
                break; // ya encontramos el círculo correspondiente
            }
        }

        return score;
    }


}
