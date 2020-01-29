using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Script attached to the title screen, plays title animation when game controller is initialized.
 * </summary>
 */

public class TitleScreen : MonoBehaviour
{
    public bool m_buttonAnimationCompleted = false;
    public Transform worldButtonParent;
    public List<Button> m_worldButtons = new List<Button>();
    public float targetSpacing = 40f;
    public float fadeSpeed = 0.8f;
    public HorizontalLayoutGroup layoutGroup;
    private bool m_loadDone = false;
    public Toggle creditsToggle;
    public GameObject creditsObjects;
    public GameObject titlescreenObjects;
    public Image titlescreenBackgroundImage;

    private void Start()
    {
        GameController.Instance.GameControllerReady += StartFade;
        foreach(Transform go in worldButtonParent)
        {
            m_worldButtons.Add(go.GetComponent<Button>());
        }
        creditsToggle.onValueChanged.AddListener(delegate { OnCreditsToggleValueChanged(creditsToggle); });
    }

    private void Update()
    {
        if (m_loadDone && !m_buttonAnimationCompleted)
        {
            ShowWorldButtons();
        }
    }

    /// <summary>
    /// Shows the World Selection Button with a fade.
    /// </summary>
    public void ShowWorldButtons()
    {
        if (layoutGroup.spacing >= targetSpacing - 5f)
        {
            m_buttonAnimationCompleted = true;
        }
        if (!m_buttonAnimationCompleted)
        {
            layoutGroup.spacing = Mathf.Lerp(layoutGroup.spacing, layoutGroup.spacing + 100f, fadeSpeed * Time.deltaTime);
            foreach (Button button in m_worldButtons)
            {
                float value = Mathf.Lerp(button.image.color.a, 1f + 0.5f, fadeSpeed / 2 * Time.deltaTime);
                button.image.color = new Color(1f, 1f, 1f, value);
            }
        }
        else
        {
            layoutGroup.spacing = targetSpacing;
            foreach (Button button in m_worldButtons)
            {
                button.image.color = new Color(1f, 1f, 1f, 1f);
                button.image.raycastTarget = true;
            }
        }
    }

    private void StartFade()
    {
        m_loadDone = true;
    }

    private void OnDestroy()
    {
        GameController.Instance.GameControllerReady -= StartFade;
    }

    public void OnCreditsToggleValueChanged(Toggle creditsToggleValue)
    {
        creditsObjects.SetActive(creditsToggleValue.isOn);
        titlescreenObjects.SetActive(!creditsToggleValue.isOn);
        titlescreenBackgroundImage.enabled = !creditsToggleValue.isOn;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void TurnOffTitlescreen()
    {
        gameObject.SetActive(false);
    }
}
