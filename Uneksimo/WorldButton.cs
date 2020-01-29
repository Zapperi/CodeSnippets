using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * World Selection Buttons.
 * </summary>
 */

public class WorldButton : MonoBehaviour, IWorldButton
{
    /// <summary>
    /// What island enum does the button represent.
    /// </summary>
    public WorldManager.WorldID island;

    /// <summary>
    /// Tracks the current sibling index.
    /// </summary>
    private int m_index;
    /// <summary>
    /// Reference to the gameobject's parent.
    /// </summary>
    private GameObject m_parent;
    /// <summary>
    /// Reference to this gameobject's image component.
    /// </summary>
    private Image m_image;
    /// <summary>
    /// Reference to this gameobject's button component.
    /// </summary>
    private Button m_thisButton;
    /// <summary>
    /// Reference to the currently active floor button.
    /// </summary>
    private GameObject m_activeButton;
    /// <summary>
    /// How long the fade takes.
    /// </summary>
    private float m_fadeDuration;
    /// <summary>
    /// Integer of the last button.
    /// </summary>
    private int m_lastButtonInteger;

    private bool buttonFading;

    void Start()
    {
        // Cache the reference to the parent.
        m_parent = transform.parent.gameObject;
        // Cache the reference to the button component.
        m_thisButton = GetComponent<Button>();
        // Cache the reference to the image component.
        m_image = GetComponent<Image>();
        // Create a listener for the button.
        m_thisButton.onClick.AddListener(SelectWorld);
        // Get fade duration from the World Button Grid.
        m_fadeDuration = WorldButtonsGridLayout.Instance.buttonFadeDuration;
        // Cache the last button integer
        m_lastButtonInteger = m_parent.transform.childCount - 1;

        // ---------Listen to MenuController events.---------
        UIController.Instance.ShowStickerBook += this.HideButtonInstant;
        UIController.Instance.HideStickerBook += this.ShowMainButtonInstant;
        UIController.Instance.UpdateMainWorldButtonEvent += this.UpdateMainButton;
        UIController.Instance.HideUIButtons += this.HideButtonInstant;
        UIController.Instance.ShowUIButtons += this.ShowMainButtonInstant;
        UIController.Instance.ShowWorldButtons += this.ShowButtonWithFade;
        UIController.Instance.HideWorldButtons += this.HideButtonWithFade;
        UIController.Instance.ShowSettings += this.HideButtonWithFade;

        PlayerInput.Instance.TouchBegan += this.OneFingerTouchOutsideButtons;
        // ---------------------------------------------------
    }

    private void Update()
    {
        if(buttonFading && m_image.canvasRenderer.GetAlpha() <= 0.1)
        {
            buttonFading = false;
            m_image.enabled = false;
        }
    }

    /// <summary>
    /// Selects the island tied to the button. Swaps sibling index with the active button and pressed button.
    /// </summary>
    private void SelectWorld()
    {
        if (StickerBookUI.Instance.isWorldButtonPressed)
        {
            return;
        }
        else
        {
            StickerBookUI.Instance.isWorldButtonPressed = true;
        }

        // IF stickerbook is being dragged, prevent usage of this function.
        if (StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        // IF button is the active island button..
        if (transform.GetSiblingIndex() == m_lastButtonInteger)
        {
            // Toggle between visible and hidden states of the floor buttons.
            WorldButtonsGridLayout.Instance.ToggleButtonVisibility();
        }
        // ...otherwise swap between active and self and call for button visibility toggle.
        else
        {
            // Get current sibling index.
            m_index = transform.GetSiblingIndex();
            // Get current active floor button.
            m_activeButton = m_parent.transform.GetChild(m_lastButtonInteger).gameObject;
            // Set this button as active button.
            transform.SetAsLastSibling();
            // Set active button's index to this button.
            m_activeButton.transform.SetSiblingIndex(m_index);
            // Toggle floor selection button visibility.
            WorldButtonsGridLayout.Instance.ToggleButtonVisibility();
            LoadWorld(island);
        }

        StickerBookUI.Instance.isWorldButtonPressed = false;
    }

    /// <summary>
    /// Disables button by fading the image out and removing raycast target.
    /// </summary>
    public void HideButtonCustomDuration(float duration)
    {
        // Use crossfade for image's alpha.
        m_image.CrossFadeAlpha(0.001f, duration, false);
        // Remove raycast target.
        m_image.raycastTarget = false;
        buttonFading = true;
    }

    /// <summary>
    /// Hides the World Selection Button with a fade. If button is main button, don't hide it.
    /// </summary>
    public void HideButtonWithFade()
    {
        if (transform.GetSiblingIndex() == m_lastButtonInteger)
        {
            return;
        }
        m_image.CrossFadeAlpha(0.001f, m_fadeDuration, false);
        m_image.raycastTarget = false;
        buttonFading = true;
    }

    /// <summary>
    /// Hides the World Selection Button with a fade. If button is main button, don't hide it.
    /// </summary>
    public void OneFingerTouchOutsideButtons(Touch touch)
    {
        if(Input.touchCount <= 1)
        {
            if (transform.GetSiblingIndex() == m_lastButtonInteger)
            {
                if (!WorldButtonsGridLayout.Instance.buttonsAreHidden){
                    WorldButtonsGridLayout.Instance.ToggleButtonVisibility();
                }
                //return;
            }
            //m_image.CrossFadeAlpha(0.001f, m_fadeDuration, false);
            //m_image.raycastTarget = false;
            //buttonFading = true;
        }
    }

    /// <summary>
    /// Hides the World Selection Button instantly. If button is main button, don't hide it.
    /// </summary>
    private void HideButtonInstant()
    {
        m_image.CrossFadeAlpha(0.001f, 0, false);
        // Remove raycast target.
        m_image.raycastTarget = false;
        m_image.enabled = false;
    }

    /// <summary>
    /// Enables button by fading the image in and adding raycast target.
    /// </summary>
    public void ShowButtonCustomDuration(float duration)
    {
        if (UIController.Instance.isUIHidden)
        {
            return;
        }
        m_image.enabled = true;
        m_image.CrossFadeAlpha(1f, duration, false);
        m_image.raycastTarget = true;
    }

    /// <summary>
    /// Shows the World Selection Button with a fade. If UI is supposed to be hidden, does nothing.
    /// </summary>
    public void ShowButtonWithFade()
    {
        if (UIController.Instance.isUIHidden)
        {
            return;
        }
        m_image.enabled = true;
        m_image.CrossFadeAlpha(1f, m_fadeDuration, false);
        m_image.raycastTarget = true;
    }

    /// <summary>
    /// Shows only the main World Selection Button with a fade. If UI is supposed to be hidden, do nothing..
    /// </summary>
    public void ShowMainButtonWithFade()
    {
        if (transform.GetSiblingIndex() == m_lastButtonInteger && !UIController.Instance.isUIHidden)
        {
            m_image.enabled = true;
            m_image.CrossFadeAlpha(1, m_fadeDuration, false);
            m_image.raycastTarget = true;
            WorldButtonsGridLayout.Instance.buttonsAreHidden = true;
        }
    }

    /// <summary>
    /// Shows only the main World Selection Button instantly. If UI is supposed to be hidden, do nothing..
    /// </summary>
    private void ShowMainButtonInstant()
    {
        if (transform.GetSiblingIndex() == m_lastButtonInteger && !UIController.Instance.isUIHidden)
        {
            m_image.enabled = true;
            m_image.CrossFadeAlpha(1, 0, false);
            m_image.raycastTarget = true;
            WorldButtonsGridLayout.Instance.buttonsAreHidden = true;
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Calls for GameController to load a new island.
    /// </summary>
    /// <param name="islandToLoad">Island type to load (Enum).</param>
    public void LoadWorld(WorldManager.WorldID islandToLoad)
    {
        GameController.Instance.LoadLevel(islandToLoad);
    }

    public void UpdateMainButton(WorldManager.WorldID newWorld)
    {
        if (newWorld == island)
        {
            transform.SetAsLastSibling();
        }
    }
}
