using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**
 * Author: Henri Leppänen
 * <summary>
 * A pointer finger tutorial helps new player to learn navigating in the game.
 * </summary>
 */

public class PointerTutorial : MonoBehaviour
{
    [Tooltip("At awake this RectTranform is resized to match used screen size resolution." +
        "It is used as a parent to position tutorial UI images on screen.")]
    /// <summary>
    /// At awake this RectTranform is resized to match used screen size resolution. It is used as a parent to position
    /// tutorial UI images on screen.
    /// </summary>
    public RectTransform panel;
    [Tooltip("A pointer finger image.")]
    /// <summary>
    /// A pointer finger image.
    /// </summary>
    public RectTransform dragFinger;
    // Bookmark drag initial position.
    private Vector3 initPos;
    //Sticker menu drag initial position.
    private Vector3 initPos2;
    [Header("Bookmark tutorial")]
    [Tooltip("Bookmark animation speed modifier. Animation speed is calculated multiplying modifier value and drag distance.")]
    [Range(0.5f, 2f)]
    ///Bookmark animation speed modifier. Animation speed is calculated multiplying modifier value and drag distance.
    public float drag1SpeedMod = 0.8f;
    // Bookmark drag speed.
    private float drag1Speed;
    [Tooltip("Bookmark drag distance as portion of the screen height.")]
    /// <summary>
    /// Bookmark drag distance.
    /// </summary>
    [Range(0.1f, 1f)]
    public float drag1mod = 0.5f;
    private float drag1Dist;
    [Tooltip("Bookmark drag animation overall duration. Distance and speed values determine the actual moved distance." +
        "When the set distance is reached, the animation stops to that position for the remainder of the duration.")]
    /// <summary>
    /// Bookmark drag animation overall duration. Distance and speed values determine the actual moved distance.
    /// When the set distance is reached, the animation stops to that position for the remainder of the duration.
    /// </summary>
    public float drag1Time = 2f;

    [Header ("Stickerbook tutorial")]

    [Tooltip("Animation curve for y-position of the finger in stickerbook tutorial animation.")]
    /// Animation curve for y-position of the finger in stickerbook tutorial animation.
    public AnimationCurve canvasDragHeight;
    [Tooltip("Scales the effect of the y-axis animation curve.")]
    /// <summary>
    /// Scales the effect of the y-axis animation curve as a portion of the screen height.
    /// </summary>
    [Range(0f,1f)]
    public float drag2HeightMod = 0.3f;
    private float heightModifier;
    [Tooltip("Stickerbook animation speed modifier. Animation speed is calculated multiplying modifier value and drag distance.")]
    /// <summary>
    /// Stickerbook animation speed modifier. Animation speed is calculated multiplying modifier value and drag distance.
    /// </summary>
    [Range(0.5f, 2f)]
    public float drag2SpeedMod = 0.8f;
    private float drag2Speed;
    [Tooltip("Stickerbook drag animation distance as portion of the screen width.")]
    /// Stickerbook drag animation distance.
    [Range(0.1f, 1f)]
    public float drag2DistMod = 0.5f;
    private float drag2Dist;
    [Tooltip("Stickerbook drag animation overall duration. Distance and speed values determine the actual moved distance. " +
        "When the set distance is reached, the animation stops to that position for the remainder of the duration.")]
    /// <summary>
    /// Stickerbook drag animation overall duration. Distance and speed values determine the actual moved distance.
    /// When the set distance is reached, the animation stops to that position for the remainder of the duration.
    /// </summary>
    public float drag2Time = 3f;
    // This value is used to track which events are active / inactive to release them accordingly.
    private bool touched = false;
    // This value is used to keep stickerbook animation on pause while sticker is handled.
    private bool onPause = false;
    // This value is used to track if wait is on.
    private bool waitEnded = false;
    // Timer value.
    private float timer = 0f;
    // State machine state enum
    private enum State
    {
        off,
        dragPointer,
        stickerbook,
        touching,
        bookClosed
    }
    //Current state machine state
    private State state = State.off;
    /// <summary>
    /// A singleton instance of the class.
    /// </summary>
    public static PointerTutorial Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Initializes the pointer tutorial.
    /// </summary>
    /// <param name="tutorialCompleted">If true (tutorial is already completed), destroys this gameobject.</param>
    public void Init(bool tutorialCompleted)
    {
        if (!tutorialCompleted)
        {
            panel.sizeDelta.Set(Screen.width, Screen.height);
            panel.SetParent(this.gameObject.transform);
            panel.anchorMin = new Vector2(0f, 0f);
            panel.anchorMax = new Vector2(1f, 1f);
            panel.pivot = new Vector2(0f, 0f);
            drag1Dist = Screen.height * drag1mod;
            drag1Speed = drag1Dist * drag1SpeedMod;
            drag2Dist = Screen.width * drag2DistMod;
            drag2Speed = drag2Dist * drag2SpeedMod;
            heightModifier = Screen.height * drag2HeightMod;
            dragFinger.SetParent(panel.gameObject.transform);
            Vector3 size = dragFinger.sizeDelta;
            size.x = Screen.width / 5f;
            size.y = Screen.height / 5f;
            dragFinger.sizeDelta = size;

            initPos = new Vector3(Screen.width - dragFinger.sizeDelta.x / 2f, Screen.height - dragFinger.sizeDelta.y, 0f);
            initPos2 = new Vector3(dragFinger.sizeDelta.x, Screen.height - dragFinger.sizeDelta.y / 2f, 0f);
            dragFinger.localPosition = initPos;
            dragFinger.gameObject.SetActive(false);
            GameController.Instance.StickerObtained += DragFingerOn;
            UIController.Instance.ShowStickerBook += OpenStickerBook;
        }
        else
        {
            // Also add tutorial sticker which is outside of allsticker list.
            StickerBook.Instance.AddSticker(StickerBook.Instance.tutorialSticker);
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// Tutorial is mainly controlled by events. Animation state machine is controlled by Update.
    /// </summary>
    void Update()
    {
        switch (state)
        {
            case (State.dragPointer):
                if (!waitEnded)
                {
                    timer += Time.deltaTime;
                    if (timer > 3f)
                    {
                        waitEnded = true;
                        timer = 0f;
                        dragFinger.gameObject.SetActive(true);
                    }
                    break;
                }
                timer += Time.deltaTime;
                if (timer > drag1Time)
                {
                    timer = 0f;
                }
                SetFingerPositionOnBookmark(timer);
                break;
            case (State.stickerbook):
                timer += Time.deltaTime;
                if (!waitEnded)
                {
                    if (timer > 1f)
                    {
                        waitEnded = true;
                        timer = 0f;
                        dragFinger.gameObject.SetActive(true);
                    }
                    break;
                }
                timer %= drag2Time;
                SetFingerPositionOnStickerbook(timer);
                break;
            case (State.touching):
                if (onPause)
                {
                    break;
                }
                timer += Time.deltaTime;
                if (!waitEnded)
                {
                    if (timer > 2f)
                    {
                        waitEnded = true;
                        timer = 0f;
                        dragFinger.localPosition = initPos2;
                        dragFinger.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (timer > drag2Time*2f)
                    {
                        timer = 0f;
                        waitEnded = false;
                        dragFinger.gameObject.SetActive(false);
                        break;
                    }
                    SetFingerPositionOnStickerbook(timer % drag2Time);
                }
                break;
        }
    }
    // Called by an event. When first sticker is obtained, removes its listener and changes state machine status.
    // (bookmark drag animation starts)
    private void DragFingerOn()
    {
        GameController.Instance.StickerObtained -= DragFingerOn;
        state = State.dragPointer;

    }

    // Calculates current position of animated finger on stickerbook tutorial.
    private void SetFingerPositionOnStickerbook(float timerValue)
    {
        float xValue = Mathf.Clamp(drag2Speed * timerValue, 0f, drag2Dist);
        float yValue = canvasDragHeight.Evaluate(Mathf.Clamp(xValue / drag2Dist, 0f, 1f)) * heightModifier;
        Vector3 pos = initPos2;
        pos.x += xValue;
        pos.y += (yValue);
        dragFinger.localPosition = pos;
    }
    // Calculates current position of animated finger on bookmark tutorial.
    private void SetFingerPositionOnBookmark(float timerValue)
    {
        float yValue = Mathf.Clamp(drag1Speed * timer, 0f, drag1Dist);
        Vector3 pos = initPos;
        pos.y -= yValue;
        dragFinger.localPosition = pos;
    }
    // Called by an event. Stickerbook is opened. Reassigns listeners depending on the current state.
    // Resets animation position and timer.
    private void OpenStickerBook()
    {
        if (state == State.off)
        {
            //if stickerbook is opened before the first obtained sticker, dragpointer phaze is skipped
            GameController.Instance.StickerObtained -= DragFingerOn;
        }
        if (state != State.bookClosed)
        {
            //eli avataan ekan kerran
            dragFinger.transform.Rotate(dragFinger.transform.forward, 90f);
        }
        dragFinger.transform.position = initPos2;
        state = State.stickerbook;
        UIController.Instance.ShowStickerBook -= OpenStickerBook;
        GameController.Instance.StickerIsTouched += StickerTouched;
        dragFinger.gameObject.SetActive(false);
        timer = 0f;
        waitEnded = false;
        UIController.Instance.HideStickerBook += StickerBookClosed;
    }
    // Called by an event (sticker is touched for the first time). Reassigns listener, and puts tutorial animation
    // on pause. (Tutorial timer starts again if sticker is dropped outside the canvas area.)
    private void StickerTouched()
    {
        onPause = true;
        touched = true;
        timer = 0f;
        waitEnded = false;
        dragFinger.gameObject.SetActive(false);
        state = State.touching;
        GameController.Instance.StickerIsTouched -= StickerTouched;
        GameController.Instance.StickerIsTouched += ResetTouchTimer;
        GameController.Instance.StickerOutside += ResumeTouchTimer;
        GameController.Instance.StickerIsPlaced += StickerDropped;
    }

    // Called by an event. This is used for the consecutive sticker touches.
    private void ResetTouchTimer()
    {
        dragFinger.gameObject.SetActive(false);
        waitEnded = false;
        timer = 0f;
        onPause = true;
    }

    // Called by an event. Dragging to the canvas has failed, sets tutorial back on.
    private void ResumeTouchTimer()
    {
        onPause = false;
    }

    // Called by an event, used when stickerbook is closed before tutorial is completed. Resets values and
    // reassigns listeners.
    private void StickerBookClosed()
    {
        timer = 0f;
        waitEnded = false;
        onPause = false;
        if (touched == false)
        {
            GameController.Instance.StickerIsTouched -= StickerTouched;
        }
        else
        {
            GameController.Instance.StickerIsTouched -= ResetTouchTimer;
            GameController.Instance.StickerOutside -= ResumeTouchTimer;
        }
        touched = false;
        GameController.Instance.StickerIsPlaced -= StickerDropped;
        UIController.Instance.HideStickerBook -= StickerBookClosed;
        UIController.Instance.ShowStickerBook += OpenStickerBook;
        dragFinger.gameObject.SetActive(false);
        state = State.bookClosed;
    }

    // Called by event. Tutorial is completed when sticker is succesfully dropped on canvas. Releases listeners
    // and destroys this tutorial gameobject.
    private void StickerDropped()
    {
        UIController.Instance.HideStickerBook -= StickerBookClosed;
        GameController.Instance.StickerIsTouched -= ResetTouchTimer;
        GameController.Instance.StickerOutside -= ResumeTouchTimer;
        GameController.Instance.StickerIsPlaced -= StickerDropped;
        GameController.Instance.PointerTutorialCompleted();
        Destroy(this.gameObject);
    }
}
