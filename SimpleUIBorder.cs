using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Collections.ObjectModel;

[ExecuteAlways]
[DisallowMultipleComponent]
public class SimpleUIBorder : MonoBehaviour
{
    [SerializeField]
    private bool differentEdgeWidths = false;
    [SerializeField]
    private bool differentEdgeColors = false;

    [SerializeField]
    private float[] edgeWidths = new float[4] { 4, 4, 4, 4 };
    [SerializeField]
    private Color32[] edgeColors = new Color32[4] { Color.black, Color.black, Color.black, Color.black };
    [SerializeField]
    private E_BorderAlignment borderAlignment = E_BorderAlignment.Outside;

    [SerializeField]
    private E_BorderStyle borderStyle = E_BorderStyle.Solid;
    [SerializeField]
    private float pixelsPerUnitMultiplier = 1;

    [SerializeField]
    private E_BorderCornerStyle cornerStyle = E_BorderCornerStyle.Overlap;
    [SerializeField]
    private float cornerExtendDistance = 4;
    [SerializeField]
    private float cornerInsetDistance = 4;

    private RectTransform[] borderEdges = new RectTransform[4];
    private Image[] borderEdgeImages = new Image[4];
    private RectTransform borderRect;

    private Sprite verticalDashedBorder;
    private Sprite horizontalDashedBorder;

    private Material[] dashedMaterials = new Material[4];

    public bool DifferentEdgeWidths { get => differentEdgeWidths; }
    public bool DifferentEdgeColors { get => differentEdgeColors; }
    public E_BorderAlignment BorderAlignment { get => borderAlignment; set => borderAlignment = value; }
    public E_BorderStyle BorderStyle { get => borderStyle; set => borderStyle = value; }
    public float PixelsPerUnitMultiplier { get => pixelsPerUnitMultiplier; set => pixelsPerUnitMultiplier = value; }
    public E_BorderCornerStyle CornerStyle { get => cornerStyle; set => cornerStyle = value; }
    public float CornerExtendDistance { get => cornerExtendDistance; set => cornerExtendDistance = value; }
    public float CornerInsetDistance { get => cornerInsetDistance; set => cornerInsetDistance = value; }

    public enum E_BorderEdge { Left, Top, Right, Bottom, }

    public enum E_BorderAlignment { Outside, Inside, Centre, }

    public enum E_BorderStyle { Solid, Dashed, Custom, }

    public enum E_BorderCornerStyle { Overlap, NoOverlap, Extended, Inset, }

    private void Awake()
    {
        // Set up border rect
        borderRect = GetComponent<RectTransform>();

        // Set up dashed border sprites
        verticalDashedBorder = Resources.Load<Sprite>("Dash3-1");
        horizontalDashedBorder = Resources.Load<Sprite>("Dash3-1H");

        // Set up dashed border materials (for animation)
        dashedMaterials[0] = Resources.Load<Material>("DashLeft");
        dashedMaterials[1] = Resources.Load<Material>("DashTop");
        dashedMaterials[2] = Resources.Load<Material>("DashRight");
        dashedMaterials[3] = Resources.Load<Material>("DashBottom");

    }

    private void Start()
    {
        // Left 
        borderEdges[0] = (RectTransform)transform.GetChild(0);
        borderEdgeImages[0] = borderEdges[0].GetComponent<Image>();
        // Right
        borderEdges[1] = (RectTransform)transform.GetChild(2);
        borderEdgeImages[1] = borderEdges[1].GetComponent<Image>();
        // Top
        borderEdges[2] = (RectTransform)transform.GetChild(1);
        borderEdgeImages[2] = borderEdges[2].GetComponent<Image>();
        // Bottom
        borderEdges[3] = (RectTransform)transform.GetChild(3);
        borderEdgeImages[3] = borderEdges[3].GetComponent<Image>();

        // Update borders on start
        UpdateBorder();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (enabled)
            EditorApplication.delayCall += OnValidateCallback;
    }

    private void OnValidateCallback()
    {
        if (this == null)
        {
            EditorApplication.delayCall -= OnValidateCallback;
            return;
        }

        UpdateBorder();
    }
#endif

    /// <summary>
    /// Changes every edges' width to <paramref name="newWidth"/> (pixels).
    /// </summary>
    /// <param name="newWidth"></param>
    public void SetBorderWidth(float newWidth)
    {
        for (int i = 0; i < edgeWidths.Length; i++)
        {
            // Assign new width
            edgeWidths[i] = newWidth;

            // Update the border width
            UpdateBorderWidth((E_BorderEdge)i);
        }

        differentEdgeWidths = false;
    }

    /// <summary>
    /// Changes multiple edges' width to <paramref name="newWidth"/> in the order: [0]Left, [1]Top, [2]Right, [3]Bottom.<br></br>
    /// </summary>
    /// <param name="newColors"></param>
    public void SetBorderWidth(float[] newWidth)
    {
        for (int i = 0; i < newWidth.Length; i++)
        {
            // Assign new width
            edgeWidths[i] = newWidth[i];

            // Update the border width
            UpdateBorderWidth((E_BorderEdge)i);
        }

        UpdateDifferentEdgeWidths();
    }

    /// <summary>
    /// Changes <paramref name="edge"/>'s width to <paramref name="newWidth"/> (pixels).
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="newWidth"></param>
    public void SetBorderWidth(E_BorderEdge edge, float newWidth)
    {
        // Assign new width
        edgeWidths[(int)edge] = newWidth;

        // Update the border width
        UpdateBorderWidth(edge);

        UpdateDifferentEdgeWidths();
    }

    /// <summary>
    /// Returns a read only collection of the border widths.
    /// </summary>
    public ReadOnlyCollection<float> GetBorderWidths()
    {
        return new ReadOnlyCollection<float>(edgeWidths);
    }

    /// <summary>
    /// Changes every edges' color to <paramref name="newColor"/>.
    /// </summary>
    /// <param name="newColor"></param>
    public void SetBorderColor(Color32 newColor)
    {
        for (int i = 0; i < edgeColors.Length; i++)
        {
            // Assign new color
            edgeColors[i] = newColor;

            // Update the border color
            UpdateBorderColor((E_BorderEdge)i);
        }

        differentEdgeColors = false;
    }

    /// <summary>
    /// Changes multiple edges' color to <paramref name="newColors"/> in the order: [0]Left, [1]Top, [2]Right, [3]Bottom.
    /// </summary>
    /// <param name="newColors"></param>
    public void SetBorderColor(Color32[] newColors)
    {
        for (int i = 0; i < newColors.Length; i++)
        {
            // Assign new color
            edgeColors[i] = newColors[i];

            // Update the border color
            UpdateBorderColor((E_BorderEdge)i);
        }

        UpdateDifferentEdgeColors();
    }

    /// <summary>
    /// Changes <paramref name="edge"/>'s color to <paramref name="newColor"/>.
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="newColor"></param>
    public void SetBorderColor(E_BorderEdge edge, Color32 newColor)
    {
        // Assign new color
        edgeColors[(int)edge] = newColor;

        // Update the border color
        UpdateBorderColor(edge);

        UpdateDifferentEdgeColors();
    }

    /// <summary>
    /// Returns a read only collection of the border widths.
    /// </summary>
    public ReadOnlyCollection<Color32> GetBorderColors()
    {
        return new ReadOnlyCollection<Color32>(edgeColors);
    }

    /// <summary>
    /// Returns a read only collection of the border widths.
    /// </summary>
    public ReadOnlyCollection<RectTransform> GetBorderEdges()
    {
        return new ReadOnlyCollection<RectTransform>(borderEdges);
    }

    /// <summary>
    /// Returns a read only collection of the border widths.
    /// </summary>
    public ReadOnlyCollection<Image> GetBorderEdgeImages()
    {
        return new ReadOnlyCollection<Image>(borderEdgeImages);
    }

    /// <summary>
    /// Updated the color of <paramref name="edge"/>.
    /// </summary>
    /// <param name="edge"></param>
    private void UpdateBorderColor(E_BorderEdge edge)
    {
        if (borderEdgeImages[(int)edge] == null)
            return;

        borderEdgeImages[(int)edge].color = edgeColors[(int)edge];
    }

    /// <summary>
    /// Updates the width of <paramref name="edge"/>.
    /// </summary>
    /// <param name="edge"></param>
    private void UpdateBorderWidth(E_BorderEdge edge)
    {
        if (borderEdgeImages[(int)edge] == null)
            return;


        // Handle border alignment
        switch (borderAlignment)
        {
            case E_BorderAlignment.Outside:
                borderRect.offsetMin = new Vector2(-edgeWidths[(int)E_BorderEdge.Left], -edgeWidths[(int)E_BorderEdge.Bottom]);
                borderRect.offsetMax = new Vector2(edgeWidths[(int)E_BorderEdge.Right], edgeWidths[(int)E_BorderEdge.Top]);
                break;

            case E_BorderAlignment.Inside:
                borderRect.offsetMin = Vector2.zero;
                borderRect.offsetMax = Vector2.zero;
                break;

            case E_BorderAlignment.Centre:
                borderRect.offsetMin = new Vector2(0.5f * -edgeWidths[(int)E_BorderEdge.Left], 0.5f * -edgeWidths[(int)E_BorderEdge.Bottom]);
                borderRect.offsetMax = new Vector2(0.5f * edgeWidths[(int)E_BorderEdge.Right], 0.5f * edgeWidths[(int)E_BorderEdge.Top]);
                break;
        }

        // Set up Corner style
        float top = 0;
        float bottom = 0;

        float left = 0;
        float right = 0;

        float pos = 0;

        float width = edgeWidths[(int)edge];

        if (edge == E_BorderEdge.Left || edge == E_BorderEdge.Right)
        {
            switch (cornerStyle)
            {
                case E_BorderCornerStyle.Overlap:
                    // Leave as 0 for overlap
                    break;
                case E_BorderCornerStyle.NoOverlap:
                    top = edgeWidths[(int)E_BorderEdge.Top];
                    bottom = edgeWidths[(int)E_BorderEdge.Bottom];
                    break;
                case E_BorderCornerStyle.Extended:
                    top = -cornerExtendDistance;
                    bottom = -cornerExtendDistance;
                    break;
                case E_BorderCornerStyle.Inset:
                    top = cornerInsetDistance;
                    bottom = cornerInsetDistance;
                    break;
            }
        }
        else
        {
            switch (cornerStyle)
            {
                case E_BorderCornerStyle.Overlap:
                    // Leave as 0 for overlap
                    break;
                case E_BorderCornerStyle.NoOverlap:
                    // Leave as 0 for no overlap
                    break;
                case E_BorderCornerStyle.Extended:
                    left = -cornerExtendDistance;
                    right = -cornerExtendDistance;
                    break;
                case E_BorderCornerStyle.Inset:
                    left = cornerInsetDistance;
                    right = cornerInsetDistance;
                    break;
            }
        }

        // Handle edge width & corner style
        switch (edge)
        {
            case E_BorderEdge.Left:
                borderEdges[0].sizeDelta = new Vector2(width, -top + -bottom);
                borderEdges[0].offsetMin = new Vector2(pos, bottom);
                borderEdges[0].offsetMax = new Vector2(pos + width, -top);
                break;

            case E_BorderEdge.Top:
                borderEdges[1].sizeDelta = new Vector2(-left + -right, width);
                borderEdges[1].offsetMin = new Vector2(left, pos - width);
                borderEdges[1].offsetMax = new Vector2(-right, pos);
                break;

            case E_BorderEdge.Right:
                borderEdges[2].sizeDelta = new Vector2(width, -top + -bottom);
                borderEdges[2].offsetMin = new Vector2(pos - width, bottom);
                borderEdges[2].offsetMax = new Vector2(pos, -top);
                break;

            case E_BorderEdge.Bottom:
                borderEdges[3].sizeDelta = new Vector2(-left + -right, width);
                borderEdges[3].offsetMin = new Vector2(left, pos);
                borderEdges[3].offsetMax = new Vector2(-right, pos + width);
                break;
        }
    }

    private void UpdateBorderStyle(E_BorderEdge edge)
    {
        if (borderEdgeImages[(int)edge] == null)
            return;

        switch (borderStyle)
        {
            case E_BorderStyle.Solid:
                // Remove sprite and material 
                borderEdgeImages[(int)edge].sprite = null;
                borderEdgeImages[(int)edge].material = null;
                break;

            case E_BorderStyle.Dashed:
                // Update to dashed border
                borderEdgeImages[(int)edge].sprite = edge == E_BorderEdge.Left || edge == E_BorderEdge.Right ? verticalDashedBorder : horizontalDashedBorder;
                borderEdgeImages[(int)edge].material = dashedMaterials[(int)edge];

                // Update pixels per unit multiplier
                borderEdgeImages[(int)edge].pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
                break;

            default:
                // Do nothing by default - ie. for Custom style
                break;
        }
    }

    private void UpdateDifferentEdgeWidths()
    {
        // Update edge width bool
        if (edgeWidths.Distinct().Count() == 1)
        {
            differentEdgeWidths = false;
        }
        else
        {
            differentEdgeWidths = true;
        }
    }

    private void UpdateDifferentEdgeColors()
    {
        // Update edge color bool
        if (edgeColors.Distinct().Count() == 1)
        {
            differentEdgeColors = false;
        }
        else
        {
            differentEdgeColors = true;
        }
    }

    /// <summary>
    /// Updates color, width and style of all edges.
    /// </summary>
    private void UpdateBorder()
    {
        for (int i = 0; i < borderEdges.Length; i++)
        {
            UpdateBorderColor((E_BorderEdge)i);
            UpdateBorderWidth((E_BorderEdge)i);
            UpdateBorderStyle((E_BorderEdge)i);
        }
    }
}
