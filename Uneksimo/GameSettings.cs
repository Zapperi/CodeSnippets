using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles Game Settings.
 * </summary>
 */

public class GameSettings : MonoBehaviour
{
    /// <summary>
    /// Game Settings panel's gameobject.
    /// </summary>
    public GameObject resetWorldGameobject;

    public Button resetWorldButton;
    private void Start()
    {
        // Listen to Menu controller events
        UIController.Instance.ToggleSettings += this.ToggleSettingsPanel;
        UIController.Instance.ShowWorldButtons += this.HideSettings;
        UIController.Instance.ShowStickerBook += this.HideSettings;
        // --------------------------------
        resetWorldButton.onClick.AddListener(ResetWorldButton);
    }

    public void ResetWorldButton()
    {
        GameController.Instance.ResetWorldPosition();
        ToggleSettingsPanel();
    }

    /// <summary>
    /// Switches between shown and hidden states of the Game Settings panel.
    /// </summary>
    public void ToggleSettingsPanel()
    {
        // IF stickerbook is being dragged, prevent usage of this function.
        if (StickerBookUI.Instance.GetDragBool())
        {
            return;
        }

        if (resetWorldGameobject.activeSelf)
        {
            HideSettings();
        }
        else
        {
            ShowSettings();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    /// <summary>
    /// Hides the Game Settings panel, sends an event that the panel is hidden.
    /// </summary>
    public void HideSettings()
    {
        resetWorldGameobject.SetActive(false);
        UIController.Instance.OnHideSettings();
    }

    /// <summary>
    /// Shows the Game Settings panel, sends an event that the panel is shown.
    /// </summary>
    private void ShowSettings()
    {
        resetWorldGameobject.SetActive(true);
        UIController.Instance.OnShowSettings();
    }

    /// <summary>
    /// Called upon sound toggle change.
    /// </summary>
    /// <param name="change">State of the toggle.</param>
    private void SoundToggleValueChanged(Toggle change)
    {
        // Debug.Log("(!NOT YET IMPLEMENTED AT GAMESETTINGS.CS!) Sounds muted: " + change.isOn);

        AudioManager.Instance.ToggleMute(!(change.isOn));
    }
}
