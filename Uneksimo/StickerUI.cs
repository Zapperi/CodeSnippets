using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles sticker copying.
 * </summary>
 */

public class StickerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Tracks if this sticker has been obtained.
    /// </summary>
    public bool stickerObtained;
    private bool stickerUsed = true;
    /// <summary>
    /// Sticker prefab to instantiate.
    /// </summary>
    public GameObject placedStickerPrefab;
    /// <summary>
    /// Track what gameobject is being dragged.
    /// </summary>
    public static GameObject draggedObject;
    public Sticker stickerInformation;
    // Reference to the object that holds all UI stickers.
    private Transform m_placedStickerContainer;
    /// <summary>
    /// Reference to the dragged sticker's image.
    /// </summary>
    private Image m_stickerImage;
    /// <summary>
    /// Image that shows at the bottom layer of the sticker background.
    /// </summary>
    [Tooltip("Background base Image.")]
    public Image stickerBackgroundBase;
    /// <summary>
    /// Image that shows on top layer of the sticker background.
    /// </summary>
    [Tooltip("Background border Image.")]
    public Image stickerBackgroundBorder;

    private void OnEnable()
    {
        // When sticker is created, cache reference to it's image.
        m_stickerImage = transform.GetChild(1).GetComponent<Image>();
        //stickerBackgroundBase = transform.GetChild(0).GetComponent<Image>();
        //stickerBackgroundBorder = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        // Cache reference to the gameobject that holds all sticker related elements.
        m_placedStickerContainer = StickerBookUI.Instance.placedStickerContainer.transform;
    }

    /// <summary>
    /// Creates a copy of the sticker with "PlacedSticker" prefab and updates its image.
    /// </summary>
    /// <returns>Copy of the original image.</returns>
    public GameObject CopySticker()
    {        
        GameObject copy;
        copy = Instantiate(placedStickerPrefab, transform.position, transform.rotation, m_placedStickerContainer);
        copy.GetComponent<PlacedSticker>().UpdateStickerInformation(stickerInformation);
        StickerBookUI.Instance.AddPlacedStickerToList(copy);
        return copy;
    }

    // When new sticker drag is started.
    public void OnBeginDrag(PointerEventData eventData)
    {        
        // IF sticker is not yet obtained OR stickerbook is being dragged, do nothing..
        if (!stickerObtained || StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        // ..Otherwise create a new sticker and initiate it's drag.
        else
        {
            if (!stickerUsed)
            {
                m_stickerImage.material = StickerBookUI.Instance.GetNormalStickerMaterial();
                stickerUsed = true;
            }           
            //// Turn off viewport highlight.
            //StickerRemove.instance.HighlightArea();
            // Create a copy of the sticker.
            draggedObject = CopySticker();
            // Update eventdata dragged object reference.
            eventData.pointerDrag = draggedObject;
            // Execute copied sticker's OnBeginDrag() fuction.
            ExecuteEvents.Execute(draggedObject, eventData, ExecuteEvents.beginDragHandler);
            GameController.Instance.StickerTouched();
        }     
    }

    // During new sticker drag.
    public void OnDrag(PointerEventData eventData)
    {
        // IF sticker is not yet obtained OR stickerbook is being dragged, do nothing..
        if (!stickerObtained || StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        // ..Otherwise update event data and call for copied sticker's drag.
        else
        {
            eventData.pointerDrag = draggedObject;
            ExecuteEvents.Execute(draggedObject, eventData, ExecuteEvents.dragHandler);
        }                
    }

    // When new sticker drag ends.
    public void OnEndDrag(PointerEventData eventData)
    {
        // IF sticker is not yet obtained OR stickerbook is being dragged, do nothing..
        if (!stickerObtained || StickerBookUI.Instance.GetDragBool())
        {
            return;
        }
        // ..Otherwise update event data and call for copied sticker's drag end.
        else
        {
            eventData.pointerDrag = draggedObject;
            ExecuteEvents.Execute(draggedObject, eventData, ExecuteEvents.endDragHandler);
        }               
    }

    /// <summary>
    /// Activate sticker, enabling draggin and setting material to null (default).
    /// </summary>
    public void StickerObtained()
    {
        stickerObtained = true;
        stickerUsed = false;
        m_stickerImage.material = StickerBookUI.Instance.GetNewObtainedStickerMaterial();
        m_stickerImage.raycastTarget = true;
    }

    /// <summary>
    /// Updates the Sticker component of the sticker and updates the new sprite.
    /// </summary>
    /// <param name="newInformation">New Sticker informaton to replace with.</param>
    public void UpdateStickerInformation(Sticker newInformation)
    {
        stickerInformation = newInformation;
        m_stickerImage.sprite = stickerInformation.icon;
    }

    /// <summary>
    /// Updates the sticker image background with sprite and color.
    /// </summary>
    /// <param name="newBackgroundBase">New background base image.</param>
    /// <param name="newBackgroundBorder">New background border image.</param>
    /// <param name="newColor">New background color.</param>
    public void UpdateStickerBackground(Sprite newBackgroundBorder, Sprite newBackgroundBase, Color newColor)
    {
        stickerBackgroundBorder.sprite = newBackgroundBorder;
        stickerBackgroundBase.sprite = newBackgroundBase;
        stickerBackgroundBase.color = newColor;
    }
}
