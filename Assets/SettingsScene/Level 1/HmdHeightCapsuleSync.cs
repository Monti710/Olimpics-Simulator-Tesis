using UnityEngine;
using Unity.XR.CoreUtils; // XROrigin

[RequireComponent(typeof(CharacterController))]
public class HmdHeightCapsuleSync : MonoBehaviour
{
    public XROrigin xrOrigin;
    [Range(1.0f, 2.2f)] public float minHeight = 1.2f;
    [Range(1.0f, 2.2f)] public float maxHeight = 2.0f;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (xrOrigin == null) xrOrigin = GetComponent<XROrigin>();
    }

    void Update()
    {
        if (xrOrigin == null || cc == null) return;

        // Altura de la cámara dentro del origin
        Vector3 camLocal = xrOrigin.CameraInOriginSpacePos;
        float height = Mathf.Clamp(camLocal.y, minHeight, maxHeight);

        cc.height = height;
        cc.center = new Vector3(camLocal.x, height * 0.5f, camLocal.z);
    }
}
