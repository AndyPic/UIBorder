using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class MouseoverBorderBehaviour : A_BorderBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private E_MouseoverAction[] mouseoverActions = new E_MouseoverAction[1] { E_MouseoverAction.ColorChange };
    [SerializeField]
    private Color32 newColor = new Color32(255, 128, 0, 255);
    [SerializeField]
    private float newWidth = 20;

    private delegate void BorderActions(bool apply);
    private BorderActions borderActions;

    public enum E_MouseoverAction { ColorChange, WidthChange, AnimationBehaviour, }

    public Color32 NewColor { get => newColor; set => newColor = value; }
    public float NewWidth { get => newWidth; set => newWidth = value; }

    /// <summary>
    /// Returns a read only collection of the mouseover actions
    /// </summary>
    public ReadOnlyCollection<E_MouseoverAction> GetClickActions()
    {
        return new ReadOnlyCollection<E_MouseoverAction>(mouseoverActions);
    }

    public void SetClickActions(E_MouseoverAction[] mouseoverActions)
    {
        this.mouseoverActions = mouseoverActions;
        SetUpActions();
    }

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
        // Clear delegate
        borderActions = null;

        // Check if ColorChange present
        if (Array.Exists(mouseoverActions, action => action == E_MouseoverAction.ColorChange))
            borderActions += ColorChange;

        // Check if WidthChange present
        if (Array.Exists(mouseoverActions, action => action == E_MouseoverAction.WidthChange))
            borderActions += WidthChange;

        // Check if AnimationBehaviour present
        if (Array.Exists(mouseoverActions, action => action == E_MouseoverAction.AnimationBehaviour))
            borderActions += AnimationBehaviour;

        /*foreach (var action in mouseoverActions)
        {
            borderActions += (BorderActions)Delegate.CreateDelegate(typeof(BorderActions), this, action.ToString());
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        borderActions(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderActions(false);
    }

    private void ColorChange(bool apply)
    {
        if (apply)
        {
            // Store previous edge colors
            StoreStartColors();

            // Change edge color to specified
            border.SetBorderColor(newColor);
        }
        else
        {
            // Reset to the stored colors
            border.SetBorderColor(StartColors);
        }
    }

    private void WidthChange(bool apply)
    {
        if (apply)
        {
            // Store previous edge colors
            StoreStartWidths();

            // Change edge color to specified
            border.SetBorderWidth(newWidth);
        }
        else
        {
            // Reset to the stored colors
            border.SetBorderWidth(StartWidths);
        }
    }

    private void AnimationBehaviour(bool apply)
    {
        var animBehaviour = GetComponent<AnimationBorderBehaviour>();

        if (animBehaviour == null)
        {
            gameObject.AddComponent<AnimationBorderBehaviour>();
            animBehaviour = GetComponent<AnimationBorderBehaviour>();
        }

        if (apply)
        {
            // Enable the animation
            animBehaviour.enabled = true;
        }
        else
        {
            // Disable
            animBehaviour.enabled = false;
        }
    }
}