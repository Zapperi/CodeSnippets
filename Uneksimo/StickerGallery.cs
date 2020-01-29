using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Controls sticker information and removal.
 * </summary>
 */

public class StickerGallery : MonoBehaviour
{
    /// <summary>
    /// List of all of the sticker gameobjects in StickerBookUI.
    /// </summary>
    [HideInInspector]
    public List<GameObject> galleryStickerList = new List<GameObject>();
    /// <summary>
    /// Rererence to the parent that holds the gallery stickers.
    /// </summary>
    [Tooltip("GameObject that holds all the gallery stickers.")]
    public GameObject contentHolder;
    /// <summary>
    /// 0 = TopCornerBorder, 1 = MiddleBorder, 2 = BottomCornerBorder, 3 = TopCornerBase, 4 = MiddleBase, 5 = BottomCornerBase
    /// </summary>
    [Tooltip("0 = TopCornerBorder, 1 = MiddleBorder, 2 = BottomCornerBorder, 3 = TopCornerBase, 4 = MiddleBase, 5 = BottomCornerBase")]
    public Sprite[] stickerBackgrounds;

    /// <summary>
    /// Background colors of the stickers, colors obtained from StickerBookUI's worldColors.
    /// </summary>
    private Color[] m_backgroundColors;

    private Dictionary<WorldManager.WorldID, List<GameObject>> stickers = new Dictionary<WorldManager.WorldID, List<GameObject>>();

    private void Awake()
    {
        // Initialize the dictionary with all the worlds found in WorldManager
        foreach (WorldManager.WorldID world in WorldManager.WorldID.GetValues(typeof(WorldManager.WorldID)))
        {
            stickers.Add(world, new List<GameObject>());
        }
    }


    /// <summary>
    /// Activates spesific sticker by name.
    /// </summary>
    /// <param name="stickerToActivate">Sticker to activate.</param>
    public void ActivateSpesificSticker(Sticker stickerToActivate)
    {
        // Cycle throught the whole sticker list.
        for(int i = 0; i < galleryStickerList.Count; ++i)
        {
            // If corresponding sticker is found, activate it and end the search.
            if(galleryStickerList[i].GetComponent<StickerUI>().stickerInformation == stickerToActivate)
            {
                galleryStickerList[i].GetComponent<StickerUI>().StickerObtained();
                return;
            }
        }
    }

    /// <summary>
    /// Activates all stickers in given list.
    /// </summary>
    /// <param name="stickerListToActivate">List of stickers to activate.</param>
    public void ActivateListOfStickers(List<Sticker> stickerListToActivate)
    {
        for(int i = 0; i < stickerListToActivate.Count; ++i)
        {
            ActivateSpesificSticker(stickerListToActivate[i]);
        }
    }

    /// <summary>
    /// Set Background colors of the stickers.
    /// </summary>
    /// <param name="newColors">New list of the colors.</param>
    public void SetBackgroundColors(Color[] newColors)
    {
        m_backgroundColors = newColors;
    }

    
    #region Sticker list initialization    

    /// <summary>
    /// Adds stickers to the sticker list.
    /// </summary>
    /// <param name="stickersToAdd">List of stickers to add.</param>
    public void InitStickerList(List<Sticker> stickersToAdd)
    {
        // IF allStickers list contains Tutorial Sticker, remove it.
        if (stickersToAdd.Contains(StickerBook.Instance.tutorialSticker))
        {
            stickersToAdd.Remove(StickerBook.Instance.tutorialSticker);
        }
        // Ensure that tutorial sticker is the very first sticker in the list.
        AddTutorialSticker();
        // Cycle throught the stickers to add list.
        for (int i = 0; i < stickersToAdd.Count; ++i)
        { 
            // Create a temp stickers, so its values can be changed.
            GameObject temp = Instantiate(StickerBookUI.Instance.galleryStickerPrefab);
            // Update the sticker component of the temp sticker and update the information.
            temp.GetComponent<StickerUI>().UpdateStickerInformation(stickersToAdd[i]);
            // Create a new name for the sticker
            temp.name = ("Sticker_" + stickersToAdd[i].name);
            // Save the temp sticker to a sticker list corresponding it's world parameter.
            AddStickerToList(temp, stickersToAdd[i].world);
        }
        UpdateStickerBackgrounds();
        PlaceStickers();
    }

    /// <summary>
    /// Add tutorial sticker as very first sticker.
    /// </summary>
    private void AddTutorialSticker()
    {
        // Create a temp stickers, so its values can be changed.
        GameObject temp = Instantiate(StickerBookUI.Instance.galleryStickerPrefab);
        // Update the sticker component of the temp sticker and update the information.
        temp.GetComponent<StickerUI>().UpdateStickerInformation(StickerBook.Instance.tutorialSticker);
        // Create a new name for the sticker
        temp.name = ("Sticker_" + StickerBook.Instance.tutorialSticker.name);
        // Save the temp sticker to a sticker list corresponding it's world parameter.
        AddStickerToList(temp, WorldManager.WorldID.Neutral);
    }

    /// <summary>
    /// Places new sticker to the list corresponding it's world parameter.
    /// </summary>
    /// <param name="stickerToAdd">New sticker gameobject to add.</param>
    /// <param name="world">The world list it belongs into.</param>
    private void AddStickerToList(GameObject stickerToAdd, WorldManager.WorldID world)
    {
        stickers[world].Add(stickerToAdd);
    }

    /// <summary>
    /// Cycles through all of the sticker lists and call for UpdateBackground function.
    /// </summary>
    private void UpdateStickerBackgrounds()
    {        
        foreach(WorldManager.WorldID world in stickers.Keys)
        {
            UpdateBackground(stickers[world], (int)world);
        }   
    }

    /// <summary>
    /// Creates backgrounds for the stickers, rounded edges at top and bottom of the lists. Color from list.
    /// </summary>
    /// <param name="listToUpdate">Sticker list to update.</param>
    /// <param name="color">Color to use, same integer as with sticker list integer.</param>
    private void UpdateBackground(List<GameObject> listToUpdate, int color)
    { 
        Color newColor;
        RectTransform tempRT = new RectTransform();
        #region ANCHOR/PIVOT VECTOR2 VALUES
        Vector2 topLeft = new Vector2(1,0);
        Vector2 topRight = new Vector2(0,0);
        Vector2 middleLeft = new Vector2(1, 0.5f);
        Vector2 middleRight = new Vector2(0, 0.5f);
        Vector2 bottomLeft = new Vector2(1,1);
        Vector2 bottomRight = new Vector2(0,1);
        #endregion


        // If background lenght does not match WorldID count, color overflow to Black and send a debug log message.
        if (color >= m_backgroundColors.Length)
        {
            newColor = new Color(0, 0, 0, 1);
            Debug.LogError("!!! MISSING BACKGROUND COLOR AT StickerBookUI! SET COLOR FOR: " + (WorldManager.WorldID)color + " !!!");
        }
        else
        {
            // Cache the color for easier use.
            newColor = m_backgroundColors[color];
        }

        // If the list has less than 4 cells, add until got atleast 4.
        if (listToUpdate.Count < 4)
        {
            // Create empty stickers without sticker image and add them to the list.
            while (listToUpdate.Count < 4)
            {
                GameObject tempGO = Instantiate(StickerBookUI.Instance.galleryStickerPrefab);
                tempGO.transform.GetChild(1).gameObject.SetActive(false);
                listToUpdate.Add(tempGO);
            }
        }

        // If there is no even number of stickers in the list, create one empty and add it to the list.
        else if (listToUpdate.Count % 2 != 0)
        {
            GameObject tempGO = Instantiate(StickerBookUI.Instance.galleryStickerPrefab);
            tempGO.transform.GetChild(1).gameObject.SetActive(false);
            listToUpdate.Add(tempGO);
        }

        // Update background for the Stickers.
        for(int i = 0; i < listToUpdate.Count; ++i)
        {
            // Get reference to the current sticker's RectTransform
            tempRT = listToUpdate[i].transform.GetChild(1).GetComponent<RectTransform>();
            // TOP_LEFT corner
            if (i == 0)
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[0], stickerBackgrounds[3], newColor);
                SetRectTransform(tempRT, topLeft);
            }
            // TOP_RIGHT corner
            else if (i == 1)
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[0], stickerBackgrounds[3], newColor);
                listToUpdate[i].transform.GetChild(0).Rotate(0f, 180f, 0f, 0f);
                SetRectTransform(tempRT, topRight);

            }
            // BOTTOM_LEFT corner
            else if (i == listToUpdate.Count - 2)
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[2], stickerBackgrounds[5], newColor);
                SetRectTransform(tempRT, bottomLeft);

            }
            // BOTTOM_RIGHT corner
            else if (i == listToUpdate.Count - 1)
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[2], stickerBackgrounds[5], newColor);
                listToUpdate[i].transform.GetChild(0).Rotate(0f, 180f, 0f, 0f);
                SetRectTransform(tempRT, bottomRight);

            }
            // MIDDLE_RIGHT piece
            else if (i % 2 == 1)
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[1], stickerBackgrounds[4], newColor);
                listToUpdate[i].transform.GetChild(0).Rotate(0f, 180f, 0f, 0f);
                SetRectTransform(tempRT, middleRight);
            }
            // MIDDLE_LEFT piece
            else
            {
                listToUpdate[i].GetComponent<StickerUI>().UpdateStickerBackground(stickerBackgrounds[1], stickerBackgrounds[4], newColor);
                SetRectTransform(tempRT, middleLeft);
            }            
        }
    }

    /// <summary>
    /// Sets anchorMax, anchorMin and pivot point to given Vector2 and sets anchored position to 0.
    /// </summary>
    /// <param name="target">Object to change.</param>
    /// <param name="location">new Vector2 value.</param>
    private void SetRectTransform(RectTransform target, Vector2 location)
    {
        target.anchorMax = location;
        target.anchorMin = location;
        target.pivot = location;
        target.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Finds stickers by world and adds them into the list in order.
    /// </summary>
    private void PlaceStickers()
    {
        foreach (WorldManager.WorldID world in stickers.Keys)
        {
            foreach (GameObject sticker in stickers[world])
            {
                sticker.transform.SetParent(contentHolder.transform, false);
                galleryStickerList.Add(sticker);
            }
        }
    }

    #endregion
}
