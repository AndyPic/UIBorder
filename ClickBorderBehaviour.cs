using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ClickBorderBehaviour : A_BorderBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private E_ClickAction[] clickActions = new E_ClickAction[1] { E_ClickAction.ColorChange };
    [SerializeField]
    private Color32 newColor = new Color32(255, 128, 0, 255);
    [SerializeField]
    private float newWidth = 0;
    [SerializeField]
    private E_ActuationType actuationType = E_ActuationType.PointerDown;

    private delegate void BorderActions();
    private BorderActions borderActions;

    private bool colorApplied = false;
    private bool widthApplied = false;

    public enum E_ClickAction { ColorChange, WidthChange, }
    public enum E_ActuationType { PointerDown, PointerUp, }

    public Color32 NewColor { get => newColor; set => newColor = value; }
    public float NewWidth { get => newWidth; set => newWidth = value; }
    public E_ActuationType ActuationType { get => actuationType; set => actuationType = value; }

    /// <summary>
    /// Returns a read only collection of the click actions
    /// </summary>
    public ReadOnlyCollection<E_ClickAction> GetClickActions()
    {
        return new ReadOnlyCollection<E_ClickAction>(clickActions);
    }

    public void SetClickActions(E_ClickAction[] clickActions)
    {
        this.clickActions = clickActions;
        SetUpActions();
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

        SetUpActions();
    }
#endif

    private void OnEnable()
    {
        SetUpActions();
    }

    protected override void Start()
    {
        base.Start();
        SetUpActions();
    }

    private void SetUpActions()
    {
        borderActions = null;

        // Check if ColorChange present
        if (Array.Exists(clickActions, action => action == E_ClickAction.ColorChange))
            borderActions += ColorChange;

        // Check if WidthChange present
        if (Array.Exists(clickActions, action => action == E_ClickAction.WidthChange))
            borderActions += WidthChange;
    }

    private void ColorChange()
    {
        if (colorApplied)
        {
            // Undo color change
            ResetColorsToStart();
            colorApplied = false;
            return;
        }

        StoreStartColors();
        border.SetBorderColor(newColor);
        colorApplied = true;
    }

    private void WidthChange()
    {
        if (widthApplied)
        {
            // Undo color change
            ResetWidthsToStart();
            widthApplied = false;
            return;
        }

        StoreStartWidths();
        border.SetBorderWidth(newWidth);
        widthApplied = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        var newTarget = raycastResults[2].gameObject;
        print($"Passing on click to {newTarget}");
        ExecuteEvents.Execute(newTarget, eventData, ExecuteEvents.pointerDownHandler);

        if (actuationType != E_ActuationType.PointerDown)
            return;

        borderActions?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        var newTarget = raycastResults[2].gameObject;
        print($"Passing on click to {newTarget}");
        ExecuteEvents.Execute(newTarget, eventData, ExecuteEvents.pointerUpHandler);

        if (actuationType != E_ActuationType.PointerUp)
            return;

        borderActions?.Invoke();
    }
}
