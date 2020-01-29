using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles game pause behaviour and logic.
 * </summary>
 */

public class GamePauseController : MonoBehaviour {
    public static GamePauseController instance;

    public GameObject uiPointers;
    public GameObject voidBall;
    [HideInInspector]    
    public GameObject sceneLights;
    private Camera activeCamera;
    private Vector3 playerOriginPosition;
    private Quaternion playerOriginRotation;
    [HideInInspector]
    public bool pauseButtonEnabled;
    [HideInInspector]
    public CapsuleCollider footCollider;
    private MedusaController medusa;
    private Animator voidBallAnimator;
    private bool flashLightWasOn;

    public LayerMask PauseLayerMask;
    private LayerMask defaultLayerMask;
    protected Transform headsetLocation;

    // Initiate instance, if copy found, keep the first one.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use Start() as a coroutine
    IEnumerator Start() {
        // Call for coroutine that checks if footCollider and HeadsetCamera has been generated, yield after it has been found.
        yield return WaitForVRTK();
        // Cache components now that they exists.
        CacheComponents();
        // Make sure that the "VoidBall" is active at the game launch.
        if(SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            ActivateVoidBall();
        }
        else
        {
            GamesceneSetup();
            DeactivateVoidBall();
        }
        
    }

    // Cache components for later use.
    private void CacheComponents()
    {
        footCollider = transform.FindDeepChild("[VRTK][AUTOGEN][FootColliderContainer]").GetComponent<CapsuleCollider>();
        activeCamera = VRTK_DeviceFinder.HeadsetCamera().GetComponent<Camera>();
        headsetLocation = VRTK_DeviceFinder.HeadsetTransform();
        voidBallAnimator = voidBall.GetComponent<Animator>();
    }
    /// <summary>
    /// Waits for VRTK to generate its components. Yields after they are found.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForVRTK()
    {
        while (!VRTK_DeviceFinder.HeadsetCamera() && !transform.FindDeepChild("[VRTK][AUTOGEN][FootColliderContainer]"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    /// <summary>
    /// Switch between pause and unpause when pausebutton is enabled.
    /// Upon unpause, reset player location to the point where pause was called.
    /// </summary>
    public void PauseSwitch()
    {
        if (pauseButtonEnabled)
        {
            // If game is not paused..
            if (!GameController.instance.gamePaused)
            {
                // Activate pause menu.
                PauseGame();
            }
            // If the game was paused..
            else
            {
                // Deactivate pause menu.
                UnPause();
            }
        }        
    }

    /// <summary>
    /// Pause the game by deactivating "VoidBall".
    /// </summary>
    public void UnPause()
    {
        // Return player to last saved location.
        PlayerController.instance.ResetPlayerLocation();
        // Deactivate pause menu.
        DeactivateVoidBall();
        if (SceneManager.sceneCount != 1){
        // Unpause medusa
        GameController.instance.medusaController.OnUnpause();
        }        
        // Unmute sounds
        SoundManager.instance.UnmuteAll();

        GameController.instance.gamePaused = false;
    }

    /// <summary>
    /// Pause the game by activating "VoidBall".
    /// </summary>
    public void PauseGame()
    {
        UIStoneScript.instance.OpenPausePage();
        // Save player location for later use.
        PlayerController.instance.SavePlayerLocation();
        // Activate pause menu.        
        ActivateVoidBall();
        if (SceneManager.sceneCount != 1){
        // Pause medusa
        GameController.instance.medusaController.OnPause();
        }        
        // Mute sounds
        SoundManager.instance.OnlyMusic();

        GameController.instance.gamePaused = true;
    }

     

    /// <summary>
    /// Activates "VoidBall". Resets player velocities, changes cullingmask, activates pointers and disables scenelights and player locomotion.
    /// </summary>
    public void ActivateVoidBall()
    {
        PlayerController.instance.FreezePlayerBody();
        PlayerController.instance.ResetPlayerVelocities();
        //// Activate voidBall.        
        RelocateVoidball();        
        voidBallAnimator.Play("VoidBallShowUIStone");
        UIStoneScript.instance.PlayParticleEffect();

        // Save the current culling mask.
        defaultLayerMask = activeCamera.cullingMask;
        // Check if mask is set to "nothing", revert it to "everything"
        if (defaultLayerMask.value == 0)
        {
            defaultLayerMask = ~0;
        }

        // If game is still at MainMenu, skip this part.
        if (SceneManager.sceneCount != 1)
        {
            // Disable scene lights
            sceneLights.SetActive(false);
            if (FlashLight.instance.flashlightIsOn)
            {                
                flashLightWasOn = false;
            }
            else
            {
                flashLightWasOn = true;
            }
            FlashLight.instance.TurnOff();
        }
        // Change camera cullingmask to pause UI layer mask.
        activeCamera.cullingMask = PauseLayerMask;
        ObjectivesController.instance.ShowObjectiveDisplay();
        // Activate straight pointers for UI usage.
        uiPointers.SetActive(true);
        PlayerController.instance.DisablePlayerMovement();
        PlayerController.instance.DisablePlayerHeadCollision();
        
        // Play UI stone up sound
        UIStoneSounds.instance.PlayStoneUpSound();
    }

    /// <summary>
    /// Move voidball to underneath the player and make sure the stoneslab is in front of the player.
    /// </summary>
    public void RelocateVoidball()
    {
        // Position the voidBall so that it's floor is right under the player's feet.
        voidBall.transform.position = new Vector3(headsetLocation.position.x, footCollider.bounds.min.y, headsetLocation.position.z);
        // Make sure that the "UI stone" is infront of the player.
        voidBall.transform.eulerAngles = new Vector3(0f, activeCamera.transform.eulerAngles.y, 0f);
    }

    /// <summary>
    /// Deactivates "VoidBall". Changes culling mask back, disables ui pointers, enables scenelights and player locomotion.
    /// </summary>
    public void DeactivateVoidBall()
    {
        //PlayerController.instance.ResetPlayerLocation();
        
        if (SceneManager.sceneCount != 1)
        {
            sceneLights.SetActive(true);
            if (flashLightWasOn)
            {
                FlashLight.instance.TurnOn();
            }            
        }
        if (defaultLayerMask.value == 0)
        {
            defaultLayerMask = ~0;
        }
        voidBallAnimator.Play("VoidBallHideUIStone");
        UIStoneScript.instance.PlayParticleEffect();
        activeCamera.cullingMask = defaultLayerMask;        
        uiPointers.SetActive(false);
        ObjectivesController.instance.HideObjectiveDisplay();
        PlayerController.instance.UnFreezePlayerBody();
        PlayerController.instance.EnablePlayerMovement();
        PlayerController.instance.EnablePlayerHeadCollision();        
        
        // Play UI stone down sound
        UIStoneSounds.instance.PlayStoneDownSound();
    }

    /// <summary>
    /// Find parent GameObject that has all the scene lights under it.
    /// </summary>
    public void FindSceneLights()
    {
        if(sceneLights == null)
        {
            sceneLights = GameObject.Find("LightSetup");
        }        
    }

    /// <summary>
    /// Activates the "VoidBall" and opens the GameOver Page on the UIstone
    /// </summary>
    public void GameOverMenu()
    {        
        ActivateVoidBall();
        ObjectivesController.instance.HideObjectiveDisplay();
        UIStoneScript.instance.OpenGameOverPage();
    }

    /// <summary>
    /// Activates the "VoidBall" and opens the GameWin Page on the UIstone
    /// </summary>
    public void GameWinMenu()
    {
        ActivateVoidBall();
        ObjectivesController.instance.HideObjectiveDisplay();
        UIStoneScript.instance.OpenGameWinPage();
    }

    /// <summary>
    /// Sets up GamePauseController on Gamescene load.
    /// </summary>
    public void GamesceneSetup()
    {
        FindSceneLights();
        pauseButtonEnabled = true;
        UIStoneScript.instance.OpenPausePage();           
    }
}
