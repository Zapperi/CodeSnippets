using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles the drag functionality of the copied Sticker.
 * </summary>
 */

public class PlacedSticker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    /// <summary>
    /// Information of the sticker.
    /// </summary>
    public Sticker stickerInformation;
    /// <summary>
    /// Calculated offset between drag point and object center.
    /// </summary>
    private Vector2 m_offSet;
    /// <summary>
    /// Tracks this sticker is selected.
    /// </summary>
    public bool isSelected;
    /// <summary>
    /// Reference to the sticker image.
    /// </summary>
    private Image m_image;
    /// <summary>
    /// Instance ID of the gameobject.
    /// </summary>
    private int m_stickerID;
    /// <summary>
    /// Tracks if two finger touch is being used.
    /// </summary>
    private bool newOffSetNeeded;

    [HideInInspector]
    public bool destroyStarted;
    [HideInInspector]
    public bool isOverDestroyerObject;

    public int collisionCount;


    private void OnEnable()
    {
        // Cache the reference upon creation.
        m_image = GetComponent<Image>();
        m_stickerID = gameObject.GetInstanceID();        
    }

    private void Start()
    {
        UIController.Instance.SelectSticker += UpdateStickerSelection;
    }

    /// <summary>
    /// Calculates offset, flags as dragged and removes raycast target when copied sticker drag is initiated.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // IF another sticker is being dragged OR stickerbook is being dragged, do nothing..
        if (StickerBookUI.Instance.isStickerDragActive || StickerBookUI.Instance.GetDragBool())
        {
            return;
        }

        else if(eventData.pointerId == 0)
        {
            // Calculate offset.
            m_offSet = (Vector2)transform.position - eventData.position;
            // Select this Sticker if it's not the same as previous.
            if (StickerBookUI.Instance.selectedStickerID != m_stickerID)
            {
                SelectSticker();
            }
            // Remove raycast target for drag duration.
            m_image.raycastTarget = false;
            // Move this Sticker to render on top of other Stickers.
            transform.SetAsLastSibling();
        }        
    }

    /// <summary>
    /// Update copied stickers new position during copied sticker drag.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // If there are more than one touch detected, set flag that new offset is needed.
        if(Input.touchCount > 1 && !newOffSetNeeded)
        {
            newOffSetNeeded = true;
        }            
        // If the ammount of touches is 1 or less.
        if (Input.touchCount <= 1)
        {
            // Count a new offset if it's needed.
            if (newOffSetNeeded)
            {
                m_offSet = (Vector2)transform.position - eventData.position;
                newOffSetNeeded = false;
            }
            // Calculate new position with offset.
            transform.position = eventData.position + m_offSet;
        }
    }

    /// <summary>
    /// Destroy if ontop of sticker container, otherwise enable raycast target and flag as not dragged upon copied sticker drag end.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // IF cursor is over stickercontainer's viewport..
        if (isOverDestroyerObject)
        {
            // Destroy the sticker.
            m_image.material = StickerBookUI.Instance.GetNormalStickerMaterial();
            StickerBookUI.Instance.DestroySpesificPlacedSticker(gameObject);
            GameController.Instance.StickerOutOfCanvas();
        }
        // Otherwise..
        else
        {            
            // Enable raycast target.
            GameController.Instance.StickerPlacedOnCanvas();
            m_image.raycastTarget = true;            
        }
    }

    /// <summary>
    /// Called when a Sticker is clicked.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // If the click was first touch, check sticker selection.
        if(eventData.pointerId == 0)
        {
            // If this sticker is same as previously selected sticker, remove the selection...
            if(StickerBookUI.Instance.selectedStickerID == m_stickerID)
            {
                StickerBookUI.Instance.selectedStickerID = 0;
                UnselectSticker();
            }
            // ..Otherwise, selected this sticker.
            else
            {
                SelectSticker();
            }
        }
    }

    /// <summary>
    /// Hides the sticker if it's instance ID does not match given ID.
    /// </summary>
    /// <param name="id">Newly selected Sticker instance ID.</param>
    private void UpdateStickerSelection(int id)
    {
        if (id != m_stickerID)
        {
            UnselectSticker();
        }      
    }

    /// <summary>
    /// Update this Sticker as new selected sticker.
    /// </summary>
    private void SelectSticker()
    {
        // Trigger selection bool.
        isSelected = true;
        // Update material to "selected material"
        m_image.material = StickerBookUI.Instance.GetSelectedStickerMaterial();
        // Update selected sticker in StickerBook.
        StickerBookUI.Instance.selectedStickerID = m_stickerID;
        // Send event to hide any other selected sticker.
        UIController.Instance.OnSelectSticker(m_stickerID);
    }

    /// <summary>
    /// Remove selection from this sticker.
    /// </summary>
    private void UnselectSticker()
    {   
        // Trigger selection bool.
        isSelected = false;
        // Update material to "Normal material".
        m_image.material = StickerBookUI.Instance.GetNormalStickerMaterial();
    }

    /// <summary>
    /// Updates the sticker's Stickerinformation, sprite and name.
    /// </summary>
    /// <param name="newInformation">New information.</param>
    public void UpdateStickerInformation(Sticker newInformation)
    {
        stickerInformation = newInformation;
        m_image.sprite = stickerInformation.icon;
        gameObject.name = stickerInformation.name;
    }

    /// <summary>
    /// Changes sticker material to "On destroy object" material.
    /// </summary>
    public void UpdateStickerMaterial(Material newMaterial)
    {
        m_image.material = newMaterial;
    }

    private void OnDestroy()
    {
        // Unsubscribe events on destroy.
        UIController.Instance.SelectSticker -= UpdateStickerSelection;
    }
}
