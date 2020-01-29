using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles UI behaviour and logic.
 * </summary>
 */
// TODO; rework pagelist to work as Enum instead for code readability.

public class UIStoneScript : MonoBehaviour
{
    public static UIStoneScript instance;
    public GameObject canvas;
    public GameObject pausePage;
    public GameObject optionsPage;
    public GameObject quitPage;
    public GameObject gameOverPage;
    public GameObject mainMenuPage;
    public GameObject gameWinPage;
    public GameObject tutorialPage;
    public Material normalMaterial;
    public Material glowMaterial;
    public Material gameOverMaterial;
    private int previousPage = 0;
    private int currentPage = 0;
    private GameObject[] pageList;
    public ParticleSystem stoneDustParticles;

    // Initiate instance, if copy found, keep the first one.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitiatePageList();
        // If first active scene is MainMenu, set starting page to MainMenu, otherwise to PauseMenu.
        if (SceneManager.sceneCount == 1)
        {
            currentPage = 4;
            OpenMainMenuPage();
        }
        else
        {
            currentPage = 0;
            OpenPausePage();
        }
    }

    // Get all the pages available and create an array out of them.
    private void InitiatePageList()
    {
        pageList = new GameObject[canvas.transform.childCount];
        for (int i = 0; i < pageList.Length; ++i)
        {
            pageList[i] = canvas.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Disables all pages and opens given page.
    /// </summary>
    /// <param name="pageToOpen">Index of the page to open.</param>
    private void OpenPage(int pageToOpen)
    {
        previousPage = currentPage;
        foreach (GameObject page in pageList)
        {
            page.SetActive(false);
        }

        if (pageToOpen < pageList.Length && pageToOpen >= 0)
        {
            pageList[pageToOpen].SetActive(true);
            currentPage = pageToOpen;
        }
    }

    /// <summary>
    /// Opens Pause Menu main page, close others.
    /// </summary>
    public void OpenPausePage()
    {
        OpenPage(0);
    }

    /// <summary>
    /// Opens Options menu page, close others.
    /// </summary>
    public void OpenOptionsPage()
    {
        OpenPage(1);
    }

    /// <summary>
    /// Opens Quit menu page, close others.
    /// </summary>
    public void OpenQuitPage()
    {
        OpenPage(2);
    }

    /// <summary>
    /// Opens Game Over page, close others.
    /// </summary>
    public void OpenGameOverPage()
    {
        GamePauseController.instance.pauseButtonEnabled = false;
        OpenPage(3);
    }

    /// <summary>
    /// Opens Main Menu main page, close others.
    /// </summary>
    public void OpenMainMenuPage()
    {
        GamePauseController.instance.pauseButtonEnabled = false;
        OpenPage(4);
    }

    /// <summary>
    /// Opens Game Win page, close others.
    /// </summary>
    public void OpenGameWinPage()
    {
        GamePauseController.instance.pauseButtonEnabled = false;
        OpenPage(5);
    }

    /// <summary>
    /// Opens tutorial page, close others.
    /// </summary>
    public void OpenTutorialPage()
    {
        OpenPage(6);
    }

    /// <summary>
    /// Opens previous page, close others.
    /// </summary>
    public void OpenPreviousPage()
    {
        OpenPage(previousPage);
    }

    /// <summary>
    /// Restarts game scene
    /// </summary>
    public void RestartGame()
    {
        GameController.instance.RestartGame();
    }

    /// <summary>
    /// Closes the game if in Main Menu, otherwise returns to the MainMenu.
    /// </summary>
    public void QuitButtonYes()
    {
        if (SceneManager.sceneCount == 1 && !GameController.instance.inTutorial)
        {
            GameController.instance.QuitGame();
        }
        else
        {
            GamePauseController.instance.pauseButtonEnabled = false;
            // Close all pages while returning to main menu
            OpenPage(-1);            
            GameController.instance.ReturnToMainMenu();
        }
    }

    /// <summary>
    /// Loads up Game Scene.
    /// </summary>
    public void StartGameButton()
    {
        GameController.instance.StartGame();
    }

    public void StartTutorialButton()
    {
        GameController.instance.StartTutorial();
        OpenPausePage();
    }

    /// <summary>
    /// Closes down the pause menu.
    /// </summary>
    public void ResumeButton()
    {
        GamePauseController.instance.UnPause();
    }

    /// <summary>
    /// Plays ui click sound
    /// </summary>
    public void PlayUIClickSound()
    {
        SoundManager.instance.PlayUIClickSound();
    }

    public void PlayParticleEffect()
    {
        stoneDustParticles.Play();
    }
}