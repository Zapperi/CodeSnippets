using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles player "owned" stickers and contains list of all stickers in the game.
 * </summary>
 */

public class StickerBook : MonoBehaviour
{
    public static StickerBook Instance = null;
    /// <summary>
    /// Sticker from finishin loading screen tutorial.
    /// </summary>
    [Tooltip("Sticker that is obtained from loadingscreen tutorial.")]
    public Sticker tutorialSticker;
    /// <summary>
    /// All stickers in the game. Set in editor. 
    /// </summary>
    public List<Sticker> allStickers = new List<Sticker>();

    /// <summary>
    /// Stickers "owned" by current player. 
    /// </summary>
    private List<Sticker> playerStickers = new List<Sticker>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if(tutorialSticker == null)
        {
            Debug.LogWarning("ASSING A TUTORIAL STICKER AT STICKERBOOK PREFAB!");
        }
    }

    //private void Start()
    //{
    //    //GameController.Instance.GameControllerReady += ReceiveEverySticker;
    //}

    /// <summary>
    /// Adds sticker to player if the player doesn't have it yet
    /// </summary>
    /// <param name="sticker"></param>
    /// <returns>True, if sticker was added. False, if sticker was not added.</returns>
    public bool AddSticker(Sticker sticker)
    {
        if (playerStickers.Contains(sticker))
        {
            Debug.Log("playerStickers contains this sticker!");
            // If repeatable sticker, only return true without readding to list, because it is already activated
            // This will result in the sticker giver still playing the sticker gain animations
            if (sticker.repeatable)
            {
                return true;
            }

            return false;
        }
        else
        {
            playerStickers.Add(sticker);

            // Update save
            int index = allStickers.IndexOf(sticker);
            if (index < 0)
            {
                if(sticker != tutorialSticker)
                {
                    Debug.LogWarning("Sticker not found in allStickers list in StickerBook");
                }
                else
                {
                    // Tutorial sticker obtained, has no index.
                   GameController.Instance.OnStickerObtained();
                }
            }
            else
            {
                GameController.Instance.UpdateStickers(index, true);
            }
            
            // Update sticker book UI
            StickerBookUI.Instance.ObtainSticker(sticker);
            return true;
        }
    }

    /// <summary>
    /// Adds list of stickers to player
    /// </summary>
    /// <param name="stickers"></param>
    public void AddStickers(List<Sticker> stickers)
    {
        foreach(Sticker sticker in stickers)
        {
            AddSticker(sticker);
        }
    }

    /// <summary>
    /// Adds stickers to player based on index in allStickers list and bool value
    /// </summary>
    /// <param name="stickers"></param>
    public void InitStickers(bool[] stickerValues)
    {
        StickerBookUI.Instance.InitializeStickerGallery(allStickers);

        if (stickerValues == null || stickerValues.Length == 0)
        {
            return;
        }
        
        for(int i = 0; i < stickerValues.Length && i < allStickers.Count; i++)
        {
            if (stickerValues[i] == true)
            {
                AddSticker(allStickers[i]);
            }
        }
    }

    /// <summary>
    /// Removes every place sticker with a fade.
    /// </summary>
    public void RemovePlacedStickers()
    {
        StickerBookUI.Instance.RemoveAllPlacedStickers();
    }

    /// <summary>
    /// Returns list of Neutral stickers that the player does not have.
    /// </summary>
    /// <returns>List of unobtained Neutral Stickers.</returns>
    public List<Sticker> GetUnobtainedNeutralStickers()
    {
        List<Sticker> temp = new List<Sticker>();
        foreach(Sticker sticker in allStickers)
        {
            if (!playerStickers.Contains(sticker) && sticker.world == WorldManager.WorldID.Neutral)
            {
                temp.Add(sticker);
            }
        }
        return temp;
    }

    private void ReceiveEverySticker()
    {
        AddStickers(allStickers);
        Debug.LogWarning("RECEIVE ALL STICKERS ENABLED AT StickerBook.cs START FUNCTION!");
    }
}
