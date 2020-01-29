using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Keeps and handles information regarding to the Stickers placed by the player.
 * </summary>
 */

public class PlacedStickerContainer : MonoBehaviour
{
    /// <summary>
    /// List of placed stickers
    /// </summary>
    [HideInInspector]
    private List<GameObject> placedStickers = new List<GameObject>();

    /// <summary>
    /// Destroys every child object under "PlacedStickers" gameobject with a fade coroutine.
    /// </summary>
    public void RemoveAllPlacedStickers(float destroySpeed)
    {
        for (int i = 0; i < placedStickers.Count; ++i)
        {
            StartCoroutine(DestroyWithFade(placedStickers[i], destroySpeed));
        }
        placedStickers.Clear();
    }

    /// <summary>
    /// Destroys a spesific sticker under PlacedSticker with a fade coroutine.
    /// </summary>
    /// <param name="target">Sticker gameobject to destroy.</param>
    public void DestroySticker(GameObject target, float destroySpeed)
    {
        if (placedStickers.Contains(target))
        {
            placedStickers.Remove(target);
            target.GetComponent<PlacedSticker>().destroyStarted = true;
            StartCoroutine(DestroyWithFade(target, destroySpeed));
        }
    }

    /// <summary>
    /// Destroys a gameobject after a certain time while reducing its Image's alpha to 0.
    /// </summary>
    /// <param name="target">Target GameObject to destroy.</param>
    /// <param name="speed">How fast the GameObject is destroyed.</param>
    IEnumerator DestroyWithFade(GameObject target, float speed)
    {        
        Image targetImage = target.GetComponent<Image>();
        float alpha = targetImage.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / speed)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
            targetImage.color = newColor;
            yield return null;
        }        
        DestroyImmediate(target);       
    }

    /// <summary>
    /// Adds given sticker to the Placed Stickers list.
    /// </summary>
    /// <param name="placedStickerToAdd">Sticker to add.</param>
    public void AddStickerToList(GameObject placedStickerToAdd)
    {
        placedStickers.Add(placedStickerToAdd);
    }

    /// <summary>
    /// Get list of Placed Stickers.
    /// </summary>
    /// <returns>List of Placed Stickers.</returns>
    public List<GameObject> GetPlacedStickerList()
    {
        return placedStickers;
    }
}


