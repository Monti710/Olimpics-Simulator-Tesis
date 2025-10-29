using UnityEngine;

public class SkyboxScroller : MonoBehaviour
{
    public float speed = 0.01f;

    void Update()
    {
        float rotation = Time.time * speed;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}

