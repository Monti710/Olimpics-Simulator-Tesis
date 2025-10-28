using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform headTransform; // Cámara principal o cabeza del XR Rig.
    public Vector3 offset = new Vector3(0, 0, 2f); // Offset delante y lateral (X,Z). Y no se modifica.

    void Update()
    {
        if (headTransform == null) return;

        Vector3 newPosition = headTransform.position;

        // Calcula desplazamiento lateral (X) y frontal (Z) con base en la orientación de la cabeza
        Vector3 lateralOffset = headTransform.right * offset.x;
        Vector3 frontalOffset = headTransform.forward * offset.z;

        // Aplica offset solo en X y Z, mantiene la altura Y original
        newPosition += lateralOffset + frontalOffset;
        // newPosition.y se mantiene igual

        transform.position = newPosition;

        // El objeto mira a la cámara
        transform.LookAt(headTransform);
        transform.Rotate(0, 180f, 0); // Para que el texto no quede invertido
    }
}
