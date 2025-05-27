using UnityEngine;

public class FlashController : MonoBehaviour
{
    [Tooltip("The renderer using the Sway shader")]
    public Renderer targetRenderer;

    [Tooltip("Auto-detect renderer on this GameObject if not assigned")]
    public bool useThisRenderer = true;

    private Material material;
    private bool wasFlashing = false;
    private float lastLinePos = 0f;
    private bool completedCycle = false;

    // Property names in shader
    private readonly string enableFlashProperty = "_EnableFlash";
    private readonly string lineSpeedProperty = "_LineSpeed";

    void Start()
    {
        // Auto-assign renderer if needed
        if (useThisRenderer && targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            material = targetRenderer.material;
    }

    void Update()
    {
        if (material == null)
            return;

        bool isFlashing = material.GetFloat(enableFlashProperty) > 0.5f;

        // If flash was just turned on, reset cycle tracking
        if (isFlashing && !wasFlashing)
        {
            completedCycle = false;
            lastLinePos = 0f;
        }

        // If flashing, check for cycle completion
        if (isFlashing)
        {
            float lineSpeed = material.GetFloat(lineSpeedProperty);
            float currentLinePos = Mathf.Repeat(Time.time * lineSpeed, 1.0f);

            // Detect when the line has completed a full cycle
            // (crossed from high value back to low value)
            if (lastLinePos > 0.9f && currentLinePos < 0.1f)
            {
                completedCycle = true;
            }

            // Turn off flashing after cycle completes
            if (completedCycle)
            {
                material.SetFloat(enableFlashProperty, 0f);
            }

            lastLinePos = currentLinePos;
        }

        wasFlashing = isFlashing;
    }

    // Public method to trigger flash from other scripts
    public void TriggerFlash()
    {
        if (material != null)
        {
            material.SetFloat(enableFlashProperty, 1f);
            completedCycle = false;
        }
    }
}
