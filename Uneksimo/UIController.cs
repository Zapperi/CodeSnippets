using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles UI related events and delegates.
 * </summary>
 */

public class UIController : MonoBehaviour {
    public static UIController Instance;
    
    #region EVENTS
    public delegate void UIEventHandler();
    public delegate void UIEventHandlerWorld(WorldManager.WorldID world);
    public delegate void SelectStickerEventHandler(int id);

    /// <summary>
    /// This event is raised when Drag has started on Dragpoint (Stickerbook).
    /// </summary>
    public event UIEventHandler StickerbookDragStarted;
    /// <summary>
    /// This event is raised when Settings panel is set to visible.
    /// </summary>
    public event UIEventHandler ShowSettings;
    /// <summary>
    /// This event is raised when Settings panel is set to hidden.
    /// </summary>
    public event UIEventHandler HideSettings;
    /// <summary>
    /// This event is raised when Settings visibility state is changed.
    /// </summary>
    public event UIEventHandler ToggleSettings;
    /// <summary>
    /// This event is raised when Sticker Book is set to visible.
    /// </summary>
    public event UIEventHandler ShowStickerBook;
    /// <summary>
    /// This event is raised when Sticker Book is set to hidden.
    /// </summary>
    public event UIEventHandler HideStickerBook;
    /// <summary>
    /// This event is raised when World Selection Buttons are set to visible.
    /// </summary>
    public event UIEventHandler ShowWorldButtons;
    /// <summary>
    /// This event is raised when World Selection Buttons are set to hidden.
    /// </summary>
    public event UIEventHandler HideWorldButtons;
    /// <summary>
    /// This event is raised when visibility state of World Selection Buttons is changed.
    /// </summary>
    public event UIEventHandler ToggleWorldButtons;
    /// <summary>
    /// This event is raised when World Selection Main Button has changed. World enum as parameter.
    /// </summary>
    public event UIEventHandlerWorld UpdateMainWorldButtonEvent;
    /// <summary>
    /// This event is raised when UI buttons are set to hidden.
    /// </summary>
    public event UIEventHandler HideUIButtons;
    /// <summary>
    /// This event is raised when UI buttons are set to visible.
    /// </summary>
    public event UIEventHandler ShowUIButtons;
    /// <summary>
    /// This event is raised when a Placed Sticker is selected. Instance ID (int) as parameter.
    /// </summary>
    public event SelectStickerEventHandler SelectSticker;
    #endregion

    /// <summary>
    /// Is UI currently hidden or not.
    /// </summary>
    public bool isUIHidden;

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
        if(GameController.Instance != null)
        {
            GameController.Instance.WorldActivated += ShowUI;
            WorldManager.Instance.WorldSelected += HideUI;
            GameController.Instance.GameControllerReady += HideUI;
        }        
    }

    #region EVENT DELEGATES
    
    /// <summary>
    /// Raises SelectSticker event with given sticker instance ID.
    /// </summary>
    /// <param name="id">Instance ID of the Sticker.</param>
    public void OnSelectSticker(int id)
    {
        if(SelectSticker != null)
        {
            SelectSticker(id);
        }
    }

    /// <summary>
    /// Raises ShowSettings event.
    /// </summary>
    public void OnShowSettings()
    {
        if (ShowSettings != null)
        {
            ShowSettings();
        }
    }

    /// <summary>
    /// Raises StickerbookDragStarted event.
    /// </summary>
    public void OnStickerbookDragStarted()
    {
        if (StickerbookDragStarted != null)
        {
            StickerbookDragStarted();
        }
    }

    /// <summary>
    /// Raises HideSettings event.
    /// </summary>
    public void OnHideSettings()
    {
        if (HideSettings != null)
        {
            HideSettings();
        }
    }

    /// <summary>
    /// Raises ShowStickerBook event.
    /// </summary>
    public void OnShowStickerBook()
    {
        if (ShowStickerBook != null)
        {
            ShowStickerBook();
        }
    }

    /// <summary>
    /// Raises HideStickerBook event.
    /// </summary>
    public void OnHideStickerBook()
    {
        if (HideStickerBook != null)
        {
            HideStickerBook();
        }
    }

    /// <summary>
    /// Raises ShowWorldButtons event.
    /// </summary>
    public void OnShowWorldButtons()
    {
        if (ShowWorldButtons != null)
        {
            ShowWorldButtons();
        }
    }

    /// <summary>
    /// Raises HideWorldButtons event.
    /// </summary>
    public void OnHideWorldButtons()
    {
        if (HideWorldButtons != null)
        {
            HideWorldButtons();
        }
    }


    /// <summary>
    /// Raises ToggleWorldButtons event.
    /// </summary>
    public void OnToggleWorldButtons()
    {
        if (ToggleWorldButtons != null)
        {
            ToggleWorldButtons();
        }
    }

    /// <summary>
    /// Raises ToggleSettings event.
    /// </summary>
    public void OnToggleSettings()
    {
        if (ToggleSettings != null)
        {
            ToggleSettings();
        }
    }
    #endregion

    /// <summary>
    /// Updates currently active world button with given world.
    /// </summary>
    /// <param name="world">New world that is active.</param>
    public void UpdateMainWorldButton(WorldManager.WorldID world)
    {
        if (UpdateMainWorldButtonEvent != null) {
            UpdateMainWorldButtonEvent(world);
        }
    }

    /// <summary>
    /// Hides World Selection Buttons and StickerBook Drag object.
    /// </summary>
    public void HideUI()
    {
        if(HideUIButtons != null)
        {
            isUIHidden = true;
            HideUIButtons();
        }
    }

    /// <summary>
    /// Shows World Selection Buttons and StickerBook Drag object.
    /// </summary>
    public void ShowUI()
    {
        if (ShowUIButtons != null)
        {
            isUIHidden = false;
            ShowUIButtons();
        }
    }
}
