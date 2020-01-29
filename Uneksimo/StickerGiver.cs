using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * Attach to objects in scene that give the player a sticker. 
 * Designed to be attached to same gameObject as an InteractableItem, 
 * which will call the public functions here.
 * </summary>
 */

public class StickerGiver : MonoBehaviour, IInteractableItemEffect
{
    /// <summary>
    /// Reference to the sticker that this gives
    /// </summary>
    public Sticker sticker;

    /// <summary>
    /// Transform that is used for position to spawn Sticker notification
    /// </summary>
    [Tooltip("Transform that is used for position to spawn Sticker notification")]
    public Transform stickerSpawnTransform;
    
    /// <summary>
    /// If true, TriggerSticker must be triggered from interaction animation. 
    /// If false, automatically triggers when interacted, if there is an InteractableItem component
    /// </summary>
    [Tooltip("If true, TriggerSticker must be triggered from interaction animation. If false, automatically triggers when interacted, if there is an InteractableItem component")]
    public bool customTriggerTime = false;

    public void InteractingStarted()
    {
        
    }


    /// <summary>
    /// Triggers sticker if customTriggerTime is false
    /// </summary>
    public void InteractingStopped()
    {
        if (!customTriggerTime)
        {
            TriggerSticker();
        }
    }

    /// <summary>
    /// Gives the player the sticker and shows visuals
    /// </summary>
    public void TriggerSticker()
    {
        Debug.Log("Triggered sticker giver");
        if (sticker != null)
        {
            if (StickerBook.Instance != null && StickerBook.Instance.AddSticker(sticker))
            {
                if (stickerSpawnTransform == null)
                {
                    Debug.LogError("Sticker spawn transform not set. Notification will not appear.");
                }
                else
                {
                    // Show sticker gain notification
                    StickerNotificationManager.Instance.SpawnNotification(stickerSpawnTransform.position, sticker);
                }
            }
        }
    }
}
