using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles Stickerbook panel dragging.
 * </summary>
 */

public class DragPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Button that opens the sticker book
    /// </summary>
    public Button openStickerBookButton;
    /// <summary>
    /// Reference to the Stickerbook panel gameobject.
    /// </summary>
    public GameObject stickerBookPanel;
    // Track if panel is hidden or not.
    private bool m_StickerBookIsHidden = true;
    // Track the position where panel is hidden.
    private Vector3 m_hiddenPosition;
    // Track the start position on drag start.
    private Vector3 m_startPosition;
    /// <summary>
    /// Position where Stickerbook panel is visible.
    /// </summary>
    public Transform visiblePosition;
    public Image bookmarkImage;
    // Track the offset from drag point to center of the object.
    private Vector3 m_panelOffSet;
    private Vector3 m_deltaY;

    private void Start()
    {
        // ---------Listen to UI events---------
        UIController.Instance.HideUIButtons += this.HideDragButton;
        UIController.Instance.ShowUIButtons += this.ShowDragButton;
        UIController.Instance.HideStickerBook += HideStickerBook;
        // -------------------------------------

        // Add functionality to button element.
        openStickerBookButton.onClick.AddListener(ShowStickerBook);

        // Save the origin local position as hidden position.
        m_hiddenPosition = transform.localPosition;
        // Calculate Y-axis offset from center to the grab point.
        m_panelOffSet = stickerBookPanel.transform.localPosition - transform.localPosition;
    }

    // When pointer begins to track..
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_StickerBookIsHidden)
        {
            return;
        }
        // IF event is either first touch or Left mouse click..
        if (eventData.pointerId == 0 || eventData.pointerId == -1)
        {
            StickerBookUI.Instance.SetStickerBookDragBool(true);
            UIController.Instance.OnStickerbookDragStarted();
            // Save current local position as start position.
            m_startPosition = transform.localPosition;
        }

    }

    // During object drag
    public void OnDrag(PointerEventData eventData)
    {
        if (!m_StickerBookIsHidden)
        {
            return;
        }
        // IF event is either first touch or Left mouse click..
        if(eventData.pointerId == 0 || eventData.pointerId == -1)
        {
            m_deltaY = new Vector3(0f, eventData.delta.y, 0f);
            // Update location of the dragged object, add Y-axis offset.
            transform.position += m_deltaY;
            stickerBookPanel.transform.localPosition += m_deltaY;
        }
    }

    // At the end of the drag
    public void OnEndDrag(PointerEventData eventData)
    {
        // IF event is either first touch or Left mouse click..
        if (eventData.pointerId == 0 ||eventData.pointerId == -1)
        {
            StickerBookUI.Instance.SetStickerBookDragBool(false);
            // If panel is hidden..
            if (m_StickerBookIsHidden)
            {
                // If panel has moved more than 100 pixels..
                if (m_startPosition.y - transform.localPosition.y >= 100f)
                {
                    // Show panel.
                    ShowStickerBook();
                }
                // ..otherwise revert panel position back to origin position.
                else
                {
                    transform.localPosition = m_hiddenPosition;
                    stickerBookPanel.transform.localPosition = transform.localPosition + m_panelOffSet;
                }
            }
            // .. otherwise panel is shown, do..
            else
            {
                // If panel has moved over 50pixels..
                if (transform.localPosition.y >= 50f)
                {
                    // Hide the stickerbook
                    HideStickerBook();
                }
                // ..otherwise revert panel location back to shown position.
                else
                {
                    transform.position = visiblePosition.position;
                    stickerBookPanel.transform.localPosition = transform.localPosition + m_panelOffSet;
                }
            }
        }
    }

    /// <summary>
    /// Hides StickerBook, moves the object to hidden position and sets IsHidden boolean to true.
    /// </summary>
    public void HideStickerBook()
    {
        // IF stickerbook is being dragged, prevent usage of this function.
        if (StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        // Move stickerbook and drag point to a hidden position.
        transform.localPosition = m_hiddenPosition;
        stickerBookPanel.transform.localPosition = transform.localPosition + m_panelOffSet;
        
        // Deselect any selected stickers by sending a 0 and setting current sticker to 0 in StickerBookUI.
        UIController.Instance.OnSelectSticker(0);
        StickerBookUI.Instance.selectedStickerID = 0;

        m_StickerBookIsHidden = true;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        StickerBookUI.Instance.SetCullingMaskToEverything();
    }

    /// <summary>
    /// Shows StickerBook, moves the object to visible position and sets IsHidden boolean to false.
    /// </summary>
    public void ShowStickerBook()
    {
        // IF stickerbook is being dragged, prevent usage of this function.
        if (StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        transform.position = visiblePosition.position;
        stickerBookPanel.transform.localPosition = transform.localPosition + m_panelOffSet;
        // Send event that stickerbook is now shown.
        UIController.Instance.OnShowStickerBook();
        m_StickerBookIsHidden = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        StickerBookUI.Instance.SetCullingMaskToUI();
    }

    /// <summary>
    /// Hides the Drag button element.
    /// </summary>
    private void HideDragButton()
    {
        Color temp = bookmarkImage.color;
        temp.a = 0.001f;
        bookmarkImage.color = temp;
        bookmarkImage.raycastTarget = false;
    }

    /// <summary>
    /// Shows the Drag button element.
    /// </summary>
    private void ShowDragButton()
    {
        Color temp = bookmarkImage.color;
        temp.a = 1;
        bookmarkImage.color = temp;
        bookmarkImage.raycastTarget = true;
    }
}
