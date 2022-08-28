using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class AnimationBorderBehaviour : A_BorderBehaviour
{
    [SerializeField]
    private E_AnimationType animationType;
    [SerializeField]
    private Color32 endColor = new Color32(0, 0, 0, 0);
    [SerializeField]
    private float endWidth = 1;
    [SerializeField]
    private E_RotationDirection rotationDirection;
    [SerializeField]
    private float animationDuration = 2;

    private bool bordersRotated = false;

    private delegate void BorderAnimation();
    private BorderAnimation borderAnimation;

    public E_AnimationType AnimationType { get { return animationType; } set { animationType = value; SetUpAnimation(); } }
    public Color32 EndColor { get => endColor; set => endColor = value; }
    public float EndWidth { get => endWidth; set => endWidth = value; }
    public E_RotationDirection RotationDirection { get => rotationDirection; set => rotationDirection = value; }
    public float AnimationDuration { get => animationDuration; set => animationDuration = value; }

    public enum E_AnimationType
    {
        ColorChange,
        WidthChange,
        Rotate,
    }

    public enum E_RotationDirection
    {
        Clockwise,
        CounterClockwise,
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

        SetUpAnimation();
    }
#endif

    protected override void Start()
    {
        base.Start();
        SetUpAnimation();
    }

    private void Update()
    {
        borderAnimation();
    }

    private void SetUpAnimation()
    {
        if (border == null)
            return;

        ResetToStart();

        // Assign corresponding method to delegate 
        switch (animationType)
        {
            case E_AnimationType.ColorChange:
                StoreStartColors();
                borderAnimation = AnimationColorChange;
                break;

            case E_AnimationType.WidthChange:
                StoreStartWidths();
                borderAnimation = AnimationWidthChange;
                break;

            case E_AnimationType.Rotate:
                bordersRotated = true;
                borderAnimation = AnimationRotate;
                break;
        }
    }

    protected override void ResetToStart()
    {
        base.ResetToStart();

        if (!bordersRotated)
            return;

        // Reset all materials to neutral offset
        foreach (var edge in border.GetBorderEdgeImages())
        {
            if (edge != null)
                edge.material.mainTextureOffset = Vector2.zero;
        }

        bordersRotated = false;
    }

    private void AnimationColorChange()
    {
        // Lerp pinpong color between start color and end color (ie. pingpong 0-1 alpha)
        var newColor = Color.Lerp(StartColors[0], endColor, Mathf.PingPong(Time.time / (animationDuration / 2), 1));

        // Set new border color
        border.SetBorderColor(newColor);
    }

    private void AnimationWidthChange()
    {
        // Get the current border width
        var newWidth = Mathf.Lerp(StartWidths[0], endWidth, Mathf.PingPong(Time.time / (animationDuration / 2), 1));

        // Set new border width
        border.SetBorderWidth(newWidth);
    }

    private void AnimationRotate()
    {
        // Calculate new offset value
        float newOffset = Time.time / animationDuration;
        Vector2 yOffset = new Vector2(0, -newOffset);
        Vector2 xOffset = new Vector2(-newOffset, 0);

        // Flip direction for counter clockwise rotation
        if (rotationDirection == E_RotationDirection.CounterClockwise)
        {
            yOffset = -yOffset;
            xOffset = -xOffset;
        }

        // Apply offset to _MainTex
        border.GetBorderEdgeImages()[0].material.mainTextureOffset = yOffset;
        border.GetBorderEdgeImages()[1].material.mainTextureOffset = xOffset;
        border.GetBorderEdgeImages()[2].material.mainTextureOffset = -yOffset;
        border.GetBorderEdgeImages()[3].material.mainTextureOffset = -xOffset;
    }

    private void OnEnable()
    {
        // Set up animation action
        SetUpAnimation();
    }

    private void OnDisable()
    {
        // Reset
        ResetToStart();
    }
}