using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles Gift Box functionality. Gives Gift Box after certain time intervals. Checks passed from System.DateTime.
 * </summary>
 */

public class GiftBox : MonoBehaviour
{
    public static GiftBox Instance;

    /// <summary>
    /// How many minutes it takes for a new Gift Box to appear.
    /// </summary>
    [Tooltip("How many minutes it takes for a new Gift Box to appear.")]
    public int giftInterval;
    /// <summary>
    /// Time since Gift box was last opened.
    /// </summary>
    private DateTime m_timeKeeper;
    public Button giftBoxButton;
    private List<Sticker> m_unobtainedNeutralStickerList;


    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        UIController.Instance.ShowStickerBook += ObtainGift;
        GameController.Instance.GameControllerReady += IntiGiftBox;
        giftBoxButton.onClick.AddListener(OpenGiftBox);
        giftBoxButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes Gift Box. Gets saved time data from GameController and generates unobtained neutral sticker list.
    /// </summary>
    private void IntiGiftBox()
    {   
        // Failsafe to see if saved time is below current year, resets timer to now.
        if(GameController.Instance.GetGiftReceivedTime().Year < DateTime.Now.Year)
        {
            m_timeKeeper = DateTime.Now;
        }
        else
        {
            m_timeKeeper = GameController.Instance.GetGiftReceivedTime();
        }
        m_unobtainedNeutralStickerList = StickerBook.Instance.GetUnobtainedNeutralStickers();
    }

    /// <summary>
    /// The time when Gift Box was last opened;
    /// </summary>
    /// <returns></returns>
    public DateTime GetGiftBoxTime()
    {
        return m_timeKeeper;
    }

    /// <summary>
    /// Sets current time counter to given time.
    /// </summary>
    /// <param name="newTime">New time.</param>
    public void SetCurrentTime(DateTime newTime)
    {
        m_timeKeeper = newTime;
    }

    /// <summary>
    /// Substracts given time from saved time, returns time as int.
    /// </summary>
    /// <param name="timeToSubtract">Time to substract.</param>
    /// <returns></returns>
    public int GetMinutesFromSince(DateTime timeToSubtract)
    {
        int minutes = 0;
        minutes = timeToSubtract.Subtract(m_timeKeeper).Minutes;
        return minutes;
    }

    /// <summary>
    /// Checks if gift interval time has been met.
    /// </summary>
    /// <param name="timeToCompare">Time to compare to.</param>
    /// <returns></returns>
    public bool HasHourPassed(DateTime timeToCompare)
    {
        if (GetMinutesFromSince(timeToCompare) >= giftInterval)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Enables Gift Box if enough time has passed and there are still unobtained neutral stickers left.
    /// </summary>
    public void ObtainGift()
    {
        if (HasHourPassed(DateTime.Now) && m_unobtainedNeutralStickerList.Count > 0)
        {
            giftBoxButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Gives player one neutral sticker and updates the Gift timers.
    /// </summary>
    public void OpenGiftBox()
    {
        // Randomize a sticker from unobtained neutral stickers and add it. Remove from list after adding.
        int temp = UnityEngine.Random.Range(0, m_unobtainedNeutralStickerList.Count);
        StickerBook.Instance.AddSticker(m_unobtainedNeutralStickerList[temp]);
        m_unobtainedNeutralStickerList.RemoveAt(temp);

        // Disable gift box button.
        giftBoxButton.gameObject.SetActive(false);

        // Update timers.
        m_timeKeeper = DateTime.Now; 
        GameController.Instance.SetGiftReceivedTime(m_timeKeeper);
    }

    private void OnDestroy()
    {
        UIController.Instance.ShowStickerBook -= ObtainGift;
        GameController.Instance.GameControllerReady -= IntiGiftBox;
    }
}