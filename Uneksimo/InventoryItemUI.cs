using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Represents a item in the inventory UI. Handles animating the item image. 
 * </summary>
 */

public class InventoryItemUI : MonoBehaviour
{
    private static WaitForEndOfFrame frame = new WaitForEndOfFrame();

    /// <summary>
    /// RectTransform that will be animated
    /// </summary>
    public RectTransform itemTransform;
    /// <summary>
    /// Image for empty item slot
    /// </summary>
    public Image emptySlotImage;
    /// <summary>
    /// Image for owned item icon
    /// </summary>
    public Image iconImage;
    /// <summary>
    /// Key that is shown
    /// </summary>
    public InteractableItemKey itemKey;

    /// <summary>
    /// Time that the scale transition in animation will take. 
    /// </summary>
    public float scaleTransitionInTime;

    // public float moveToInventoryTime;

    /// <summary>
    /// Time that the scale transition out animation will take. 
    /// </summary>
    public float scaleTransitionOutTime;

    // public float moveFromInventoryTime;

    private Vector3 startPosition;
    private Vector3 origScale;

    /// <summary>
    /// Initializes the UI item
    /// </summary>
    /// <param name="key">Key to show in this UI</param>
    public void Init(InteractableItemKey key)
    {
        emptySlotImage.sprite = key.icon;
        iconImage.sprite = key.icon;
        itemKey = key;

        // Start in inventory slot position
        itemTransform.anchoredPosition = Vector3.zero;

        origScale = itemTransform.localScale;
        itemTransform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Plays item "get" animation
    /// </summary>
    public void PlayItemGetAnimation()
    {
        StartCoroutine(ItemGetAnimationSequence());
    }

    IEnumerator ItemGetAnimationSequence()
    {
        float timer = 0f;

        while(timer < scaleTransitionInTime)
        {
            itemTransform.localScale = Vector3.Lerp(Vector3.zero, origScale, timer / scaleTransitionInTime);
            yield return frame;
            timer += Time.deltaTime;
        }

        itemTransform.localScale = origScale;
        timer = 0f;

        // while (timer < moveToInventoryTime)
        // {
        //     itemTransform.anchoredPosition = Vector3.Lerp(startPosition, Vector3.zero, timer / moveToInventoryTime);
        //     yield return frame;
        //     timer += Time.deltaTime;
        // }

        // itemTransform.anchoredPosition = Vector3.zero;
    }

    /// <summary>
    /// Lerps scale to zero, then destroys gameObject
    /// </summary>
    /// <param name="targetPos"></param>
    public void PlayItemUseAnimation(Vector3 targetPos)
    {
        StartCoroutine(ItemUseAnimationSequence(targetPos));
    }

    IEnumerator ItemUseAnimationSequence(Vector3 targetPos)
    {
        float timer = 0f;

        // Movement (commented out because we just want to scale transition without movement)
        // while (timer < moveFromInventoryTime)
        // {
        //     itemTransform.anchoredPosition = Vector3.Lerp(Vector3.zero, targetPos, timer / moveFromInventoryTime);
        //     yield return frame;
        //     timer += Time.deltaTime;
        // }

        // itemTransform.anchoredPosition = targetPos;
        // timer = 0f;

        while(timer < scaleTransitionOutTime)
        {
            itemTransform.localScale = Vector3.Lerp(origScale, Vector3.zero, timer / scaleTransitionOutTime);
            yield return frame;
            timer += Time.deltaTime;
        }

        itemTransform.localScale = Vector3.zero;

        Retire();
    }

    public void Retire()
    {
        Destroy(gameObject);
    }
}
