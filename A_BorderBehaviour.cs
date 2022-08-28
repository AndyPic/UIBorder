using UnityEngine;

public abstract class A_BorderBehaviour : MonoBehaviour
{
    protected SimpleUIBorder border;

    protected bool startColorsSet;
    private Color32[] startColors = new Color32[4];

    protected bool startWidthsSet;
    private float[] startWidths = new float[4];

    protected Color32[] StartColors { get { return startColors; } }

    protected float[] StartWidths { get { return startWidths; } }

    protected virtual void Start()
    {
        // Get reference to the border
        border = GetComponent<SimpleUIBorder>();
    }

    protected void StoreStartColors()
    {
        startColorsSet = true;
        border.GetBorderColors().CopyTo(startColors, 0);
    }

    protected void StoreStartWidths()
    {
        startWidthsSet = true;
        border.GetBorderWidths().CopyTo(startWidths, 0);
    }

    protected void ResetColorsToStart()
    {
        // Reset to starting color
        if (startColorsSet)
        {
            border.SetBorderColor(startColors);
            startColorsSet = false;
        }
    }

    protected void ResetWidthsToStart()
    {
        // Reset to starting width
        if (startWidthsSet)
        {
            border.SetBorderWidth(startWidths);
            startWidthsSet = false;
        }
    }

    protected virtual void ResetToStart()
    {
        ResetColorsToStart();
        ResetWidthsToStart();
    }
}