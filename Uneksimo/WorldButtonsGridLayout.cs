using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Manages button showing and hiding in grid layout.
 * </summary>
 */

public class WorldButtonsGridLayout : MonoBehaviour
{
    /// <summary>
    /// A singleton instance of the class.
    /// </summary>
    public static WorldButtonsGridLayout Instance;
    /// <summary>
    /// How long does the button fade animation take.
    /// </summary>
    public float buttonFadeDuration;
    /// <summary>
    /// Tracks if floor selection buttons are visible or not.
    /// </summary>
    public bool buttonsAreHidden = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // Listen to Menu Control events
        UIController.Instance.ShowSettings += this.HideWorldButtons;
        // ----------------------------
    }

    /// <summary>
    /// Switches between hidden and shown states for floor selection buttons. Sends event by Menucontroller.
    /// </summary>
    public void ToggleButtonVisibility()
    {
        if (buttonsAreHidden)
        {
            buttonsAreHidden = false;
            UIController.Instance.OnShowWorldButtons();
        }
        else
        {
            buttonsAreHidden = true;
            UIController.Instance.OnHideWorldButtons();
        }
    }

    /// <summary>
    /// Hides the World Selection Buttons, expect the Main Button. Sends event by MenuController.
    /// </summary>
    private void HideWorldButtons()
    {
        buttonsAreHidden = true;
        UIController.Instance.OnHideWorldButtons();        
    }

    /// <summary>
    /// Shows the World Selection Buttons, sends event by MenuController.
    /// </summary>
    private void ShowWorldButtons()
    {
        buttonsAreHidden = false;
        UIController.Instance.OnShowWorldButtons();        
    }

    /// <summary>
    /// Hides every button immediately.
    /// </summary>
    private void HideAllWorldButtons()
    {
        buttonsAreHidden = true;
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<WorldButton>().HideButtonCustomDuration(0f);
        }
    }

    /// <summary>
    /// Shows the Main Button (bottom right corner).
    /// </summary>
    private void ShowMainButton()
    {
        transform.GetChild(transform.childCount -1).GetComponent<WorldButton>().ShowButtonCustomDuration(buttonFadeDuration);
        buttonsAreHidden = true;
    }
}
