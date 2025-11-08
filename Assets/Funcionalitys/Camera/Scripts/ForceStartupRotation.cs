using System.Collections;
using UnityEngine;

public class ForceStartupRotation : MonoBehaviour
{
    [SerializeField]
    private Transform mainCameraTransform;

    private float initialOriginYaw;

    void Awake()
    {
        initialOriginYaw = transform.rotation.eulerAngles.y;
    }

    IEnumerator Start()
    {
        yield return null;

        float cameraLocalYaw = mainCameraTransform.localRotation.eulerAngles.y;

        float newOriginYaw = initialOriginYaw - cameraLocalYaw;

        transform.rotation = Quaternion.Euler(0, newOriginYaw, 0);
    }
}