using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class KeyHighlightOnHover : MonoBehaviour
{
    private Button button;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    private Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (image != null)
            image.color = highlightColor;
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        if (image != null)
            image.color = originalColor;
    }
}
