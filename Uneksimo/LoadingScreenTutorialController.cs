using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles loading screen tutorial functionality.
 * </summary>
 */

public class LoadingScreenTutorialController : MonoBehaviour
{
    public static LoadingScreenTutorialController Instance;

    /// <summary>
    /// GameObject that has the loading screen tutorial animation.
    /// </summary>
    public GameObject loadingscreenTutorial;
    private WorldManager.WorldID m_world;

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
    }

    /// <summary>
    /// Starts tutorial. Loads given world after tutorial has played.
    /// </summary>
    /// <param name="world">World enum to load.</param>
    public void StartTutorial(WorldManager.WorldID world)
    {
        m_world = world;
        loadingscreenTutorial.SetActive(true);
    }

    /// <summary>
    /// Ends tutorial, loads world and updates world buttons.
    /// </summary>
    public void EndTutorial()
    {
        GameController.Instance.OnTitlescreenClosed();
        if (m_world == WorldManager.WorldID.Neutral)
        {
            m_world = WorldManager.WorldID.Magic;
        }
        GameController.Instance.LoadLevel(m_world);
        UIController.Instance.UpdateMainWorldButton(m_world);

        GameController.Instance.StartTutorialCompleted();
    }
}
