using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

/**
 * Author: Henri Leppänen
 * <summary>
 * Contains player behaviour.
 * </summary>
 */

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public GameObject teleporter;
    public GameObject locomotion;
    public GameObject headsetCollision;
    public VRTK_HeadsetCollisionFade headsetFade;
    public VRTK_BodyPhysics bodyPhysics;
    public VRTK_BasicTeleport VRTK_BasicTeleport;
	public Transform headsetLocation;
    public GameObject cameraRig;
	
    private Vector3 _playerOriginPosition;
    private Quaternion _playerOriginRotation;
	private bool _steamVRsetup;
    protected Transform playArea;
    protected Vector3 lastGoodStandingPosition;
    protected Vector3 lastGoodHeadsetPosition;
    public event PositionRewindEventHandler PositionRewindToSafe;

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

    // Cache components after VRTK has generated them.
    private IEnumerator Start()
    {
        // Call for coroutine that checks if footCollider and HeadsetCamera has been generated, yield after it has been found.
        yield return StartCoroutine(GamePauseController.instance.WaitForVRTK());
        // Cache components now that they exists.
        CacheComponents();
		if(VRTK_SDKManager.instance.loadedSetup.name == "SteamVR")
		{
			_steamVRsetup = true;
		}
    }

    // Find and save components for later use.
    private void CacheComponents()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
        headsetLocation = VRTK_DeviceFinder.HeadsetTransform();
    }

    /// <summary>
    /// Activate player teleporting and locomotion.
    /// </summary>
    public void EnablePlayerMovement()
    {
        //teleporter.SetActive(true);
        locomotion.SetActive(true);
    }

    /// <summary>
    /// Disable player teleporting and locomotion.
    /// </summary>
    public void DisablePlayerMovement()
    {
        //teleporter.SetActive(false);
        locomotion.SetActive(false);
    }

    /// <summary>
    /// Resets player velocities.
    /// </summary>
    public void ResetPlayerVelocities()
    {
        if (bodyPhysics != null)
        {
            bodyPhysics.ResetVelocities();
            bodyPhysics.ResetFalling();
        }
    }

    /// <summary>
    /// Move player to given transform, get position and rotation from the transform.
    /// </summary>
    /// <param name="targetLocation">position and rotation</param>
    public void MovePlayer(Transform targetLocation)
    {
        VRTK_BasicTeleport.ForceTeleport(targetLocation.position, targetLocation.rotation);
    }

    /// <summary>
    /// Disables player head collision and fade to black on collision.
    /// </summary>
    public void DisablePlayerHeadCollision()
    {
        headsetFade.enabled = false;
        headsetCollision.SetActive(false);
    }

    /// <summary>
    /// Enables player head collision and fade to black on collision.
    /// </summary>
    public void EnablePlayerHeadCollision()
    {
        headsetFade.enabled = true;
        headsetCollision.SetActive(true);
    }

    /// <summary>
    /// Saves the player location, uses different method according to SDK in use.
    /// </summary>
    public void SavePlayerLocation()
    {
        // If game is played in steamVR (Vive), save the position where the player is standing..
        if (_steamVRsetup)
        {
            SetLastGoodPosition();
        }
        // Otherwise save the position and rotation of the camerarig.
        else
        {
            _playerOriginPosition = cameraRig.transform.position;
            _playerOriginRotation = cameraRig.transform.rotation;
        }
    }

    /// <summary>
    /// Return player to the last saved location.
    /// </summary>
    public void ResetPlayerLocation()
    {
        // If SteamVR is in use, call for RewindPosition() function to return player where he was.
        if (_steamVRsetup)
        {
            if(lastGoodStandingPosition != null)
            {
                if (SceneManager.sceneCount == 1)
                {
                    MovePlayer(GameController.instance.tutorialSpawnPoint);
                }
                else
                {
                    RewindPosition();
                }                
            }
            else
            {
                MovePlayer(GameController.instance.playerSpawnpoint);
            }
        }
        // If SteamVR is not detected, reset the player location to saved cameraRig location. 
        else
        {
            cameraRig.transform.SetPositionAndRotation(_playerOriginPosition, _playerOriginRotation);
        }
    }

    /// <summary>
    /// Rewind player position to last known good position using saved playArea and headsetLocation.
    /// </summary>
    private void RewindPosition()
    {
        if (headsetLocation != null)
        {
            // Get current playArea position.
            Vector3 storedPosition = playArea.position;
            // Count normalized offset from X and Z vectors using last known position and current position.
            float resetVectorX = lastGoodHeadsetPosition.x - headsetLocation.position.x;
            float resetVectorZ = lastGoodHeadsetPosition.z - headsetLocation.position.z;
            Vector3 resetVector = new Vector3(resetVectorX, 0f, resetVectorZ);
            Vector3 moveOffset = resetVector.normalized;
            // Count new position by adding resetVector and offset.
            storedPosition += resetVector + moveOffset;
            // Send a position rewind call with storedPostion.
            OnPositionRewindToSafe(SetEventPayload(storedPosition));
        }
    }

    /// <summary>
    /// Save the player location for later use.
    /// </summary>
    private void SetLastGoodPosition()
    {
        if (playArea != null && headsetLocation != null)
        {
            lastGoodStandingPosition = playArea.position;
            lastGoodHeadsetPosition = headsetLocation.position;
        }
    }

    /// <summary>
    /// Call for VRTK_PositionRewind event delegation
    /// </summary>
    /// <param name="e"></param>
    private void OnPositionRewindToSafe(PositionRewindEventArgs e)
    {
        if (PositionRewindToSafe != null)
        {
            PositionRewindToSafe(this, e);
        }
    }

    /// <summary>
    /// Create event payload to use with position rewind.
    /// </summary>
    /// <param name="previousPosition"> Player's previous position. </param>
    /// <returns></returns>
    private PositionRewindEventArgs SetEventPayload(Vector3 previousPosition)
    {
        PositionRewindEventArgs e;
        e.collidedPosition = previousPosition;
        e.resetPosition = playArea.position;
        return e;
    }

    public void FreezePlayerBody()
    {
        bodyPhysics.FreezePlayer();
    }

    public void UnFreezePlayerBody()
    {
        bodyPhysics.UnFreezePlayer();
    }
}
