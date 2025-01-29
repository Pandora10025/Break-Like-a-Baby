using UnityEngine;
using UnityEngine.InputSystem;

public class OutlineManager : MonoBehaviour
{
    public Outline[] outlines;  // Array of Outline components
    private bool isFadedOut = true;  // Track if the outline is currently faded out (initially true)
    public Color fadeInColor = new Color(1f, 1f, 1f, 1f);  // Full white color (solid)
    public Color fadeOutColor = new Color(1f, 1f, 1f, 0f);  // Fully transparent color

    void Start()
    {
        // Start with a basic solid white outline, no fade for all outlines
        foreach (Outline outline in outlines)
        {
            outline.AnimateOutline(6f, fadeOutColor, 0.1f);  // Start with transparent outline
        }
    }

    void Update()
    {

    }

    // Public method to toggle the outline fade
    public void ToggleOutline()
    {
        if (isFadedOut)
        {
            // Fade in all outlines (alpha = 1, visible white)
            SetOutlineColor(fadeInColor);
        }
        else
        {
            // Fade out all outlines (alpha = 0, transparent white)
            SetOutlineColor(fadeOutColor);
        }

        // Toggle the faded state
        isFadedOut = !isFadedOut;

    }

    // Helper method to set the color for all outlines
    private void SetOutlineColor(Color targetColor)
    {
        foreach (Outline outline in outlines)
        {
            outline.AnimateOutline(6f, targetColor, 0.1f);
        }
    }
}
