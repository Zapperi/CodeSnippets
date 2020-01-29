using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Activates "isOverDestroyerObject" on Placed Sticker, enables Sticker destroy sequence.
 * </summary>
 */

public class StickerDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlacedSticker")
        {
            var temp = other.gameObject.GetComponent<PlacedSticker>();
            temp.collisionCount++;
            temp.GetComponent<PlacedSticker>().isOverDestroyerObject = true;
            temp.UpdateStickerMaterial(StickerBookUI.Instance.GetDestroyStickerMaterial());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        {
            var temp = other.gameObject.GetComponent<PlacedSticker>();
            temp.collisionCount--;
            if(temp.collisionCount <= 0)
            {
                temp.isOverDestroyerObject = false;
                temp.UpdateStickerMaterial(StickerBookUI.Instance.GetSelectedStickerMaterial());
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
        
    //    if (other.tag == "PlacedSticker")
    //    {
    //        var temp = other.gameObject.GetComponent<PlacedSticker>();
    //        temp.GetComponent<PlacedSticker>().isOverDestroyerObject = true;
    //        temp.UpdateStickerMaterial(StickerBookUI.Instance.GetDestroyStickerMaterial());
    //    }
    //}
    }
