using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Controls the StickerBook UI element.
 * </summary>
 */

public class StickerBookUI : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of the class.
    /// </summary>
    public static StickerBookUI Instance;
    /// <summary>
    /// The main camera (Camera).
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// Mask that shows everything. Set in inspector.
    /// </summary>
    public LayerMask everythingMask;
    /// <summary>
    /// Mask that shows only UI elements. Set in inspector.
    /// </summary>
    public LayerMask uiMask;
    /// <summary>
    /// Object that can be dragged.
    /// </summary>
    public DragPoint dragPoint;
    /// <summary>
    /// Tracks if Stickerbook is being dragged.
    /// </summary>
    private bool m_dragIsOn;
    /// <summary>
    /// Tracks if a world button is pressed.
    /// </summary>
    [HideInInspector]
    public bool isWorldButtonPressed;
    /// <summary>
    /// Button that hides the sticker book.
    /// </summary>
    public Button closeStickerBookButton;
    /// <summary>
    /// What is instance ID of currently selected placed sticker.
    /// </summary>
    [HideInInspector]
    public int selectedStickerID = 0;
    /// <summary>
    /// Tracks if a sticker is currently being dragged.
    /// </summary>
    [HideInInspector]
    public bool isStickerDragActive;
    /// <summary>
    /// Prefab for Placed Stickers. Use this on for sticker copy.
    /// </summary>
    [Tooltip("Prefab for Placed Stickers")]
    public GameObject placedStickerPrefab;
    /// <summary>
    /// Prefab for Gallery Stickers. Use this for Gallery initialization.
    /// </summary>
    [Tooltip("Prefab for Gallery Stickers.")]
    public GameObject galleryStickerPrefab;
    /// <summary>
    /// 0 = Neutral, 1 = Magic, 2 = Mountain, 3 = Space, 4 = Star.
    /// </summary>
    [Tooltip("0 = Neutral, 1 = Magic, 2 = Mountain, 3 = Space, 4 = Star.")]
    public Color[] worldColors;

    /// <summary>
    /// GameObject that holds all of the placed stickers.
    /// </summary>
    [Tooltip("GameObject that holds all of the placed stickers.")]
    public GameObject placedStickerContainer;
    /// <summary>
    /// Reference to the placedStickerContainerScript
    /// </summary>
    public PlacedStickerContainer placedStickerContainerScript;
    /// <summary>
    /// Sticker Gallery script.
    /// </summary>
    [Tooltip("Sticker Gallery script.")]
    public StickerGallery stickerGallery;
    /// <summary>
    /// Lenght of the fade out on remove.
    /// </summary>
    [Tooltip("Lenght of the fade out on sticker remove.")]
    public float stickerRemoveSpeed = 3;

    public Image viewportImage;

    #region STICKER MATERIALS
#pragma warning disable CS0649

    [SerializeField]
    [Tooltip("Material used in Sticker Gallery on stickers not yet obtained.")]
    private Material m_unobtainedStickerMaterial;

    [SerializeField]
    [Tooltip("Material used in Sticker Gallery on stickers freshly obtained.")]
    private Material m_newObtainedStickerMaterial;

    [SerializeField]
    [Tooltip("Material used in Sticker Gallery on stickers that are obtained.")]
    private Material m_normalStickerMaterial;

    [SerializeField]
    [Tooltip("Material used on selected sticker.")]
    private Material m_selectedStickerMaterial;

    [SerializeField]
    [Tooltip("Material used when sticker is on destroyer object.")]
    private Material m_destroyStickerMaterial;
#pragma warning restore CS0649
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }        
        else
        {
            Destroy(gameObject);
            return;
        }
        // Send background colors to sticker gallery, uses world colors.
        SetStickerGalleryBackgroundColors();
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //// Add functionality to button element.
        closeStickerBookButton.onClick.AddListener(UIController.Instance.OnHideStickerBook);
        UIController.Instance.HideStickerBook += SavePlacedStickers;
    }

    /// <summary>
    /// Update stickerbook drag state.
    /// </summary>
    /// <param name="state">New state.</param>
    public void SetStickerBookDragBool(bool state)
    {
        m_dragIsOn = state;
    }

    #region STICKER GALLERY

    /// <summary>
    /// Initializes the Sticker Gallery with given list of Stickers. Sorts stickers by world and creates elements for them. Sticker material set to unobtainedStickerMaterial.
    /// </summary>
    /// <param name="stickersToAdd">List of Stickers to initialize with.</param>
    public void InitializeStickerGallery(List<Sticker> stickersToAdd)
    {
        stickerGallery.InitStickerList(stickersToAdd);

        List<StickerData> saved = GameController.Instance.LoadPlacedStickers();
        foreach (StickerData s in saved)
        {
            for (int i=0; i < stickersToAdd.Count; i++)
            {
                if (stickersToAdd[i].name.Equals(s.name))
                {
                    GameObject temp = Instantiate(placedStickerPrefab, Vector3.zero, Quaternion.identity, placedStickerContainer.transform);
                    temp.GetComponent<PlacedSticker>().UpdateStickerInformation(stickersToAdd[i]);
                    AddPlacedStickerToList(temp);
                    temp.GetComponent<PlacedSticker>().isOverDestroyerObject = false;
                    temp.GetComponent<RectTransform>().localScale = new Vector3(s.scale, s.scale, s.scale);
                    temp.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, s.rotation);
                    temp.GetComponent<RectTransform>().localPosition = new Vector3(s.xCoord, s.yCoord, 0f);

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Activates obtained sticker in Sticker Gallery. Sets material to newObtainedMaterial and enables interaction.
    /// </summary>
    /// <param name="obtainedSticker">Sticker that has been obtained.</param>
    public void ObtainSticker(Sticker obtainedSticker)
    {
        stickerGallery.ActivateSpesificSticker(obtainedSticker);
    }

    /// <summary>
    /// Sends worldColors to Sticker Gallery to use as background colors for the stickers.
    /// </summary>
    private void SetStickerGalleryBackgroundColors()
    {
        stickerGallery.SetBackgroundColors(worldColors);
    }

    #endregion

    #region PLACED STICKERS

    /// <summary>
    /// Destroys all placed stickers with fade out speed set in StickerBookUI.
    /// </summary>
    public void RemoveAllPlacedStickers()
    {
        placedStickerContainerScript.RemoveAllPlacedStickers(stickerRemoveSpeed);
    }

    /// <summary>
    /// Destroys a given placed sticker GameObject with fade out speed set in StickerBookUI.
    /// </summary>
    /// <param name="stickerToDestroy">Placed Sticker GameObject to destroy. </param>
    public void DestroySpesificPlacedSticker(GameObject stickerToDestroy)
    {
        placedStickerContainerScript.DestroySticker(stickerToDestroy, stickerRemoveSpeed);
    }

    /// <summary>
    /// Adds Placed Sticker to Placed Sticker List.
    /// </summary>
    /// <param name="placedStickerToAdd">Placed Sticker to add.</param>
    public void AddPlacedStickerToList(GameObject placedStickerToAdd)
    {
       placedStickerContainerScript.AddStickerToList(placedStickerToAdd);
    }

    private void SavePlacedStickers()
    {
        List<GameObject> placed = GetPlacedStickerList();
        List<StickerData> data = new List<StickerData>();
        for (int i=placed.Count-1; i >= 0; i--)
        {
            if (placed[i])
            {
                StickerData s = new StickerData();
                s.name = placed[i].GetComponent<PlacedSticker>().stickerInformation.name.ToString();
                s.scale = placed[i].GetComponent<RectTransform>().localScale.x;
                s.rotation = placed[i].GetComponent<RectTransform>().localEulerAngles.z;
                s.xCoord = placed[i].GetComponent<RectTransform>().localPosition.x;
                s.yCoord = placed[i].GetComponent<RectTransform>().localPosition.y;
                data.Add(s);
            }
            if (data.Count == 30)
            {
                break;
            }
        }
        GameController.Instance.SavePlacedStickers(data);
    }

    #endregion

    #region GETTERS

    /// <summary>
    /// Get Placed Sticker GameObject list.
    /// </summary>
    /// <returns>List of placed sticker GameObjects.</returns>
    public List<GameObject> GetPlacedStickerList()
    {
        return placedStickerContainerScript.GetPlacedStickerList();
    }

    /// <summary>
    /// Material that is used on unobtained Stickers.
    /// </summary>
    /// <returns>unobtainedStickerMaterial.</returns>
    public Material GetUnobtainedStickerMaterial()
    {
        return m_unobtainedStickerMaterial;
    }

    /// <summary>
    /// Material that is used on newly obtained Stickers.
    /// </summary>
    /// <returns>newObtainedStickerMaterial.</returns>
    public Material GetNewObtainedStickerMaterial()
    {
        return m_newObtainedStickerMaterial;
    }

    /// <summary>
    /// Material that is used on obtained Stickers.
    /// </summary>
    /// <returns>normalStickerMaterial.</returns>
    public Material GetNormalStickerMaterial()
    {
        return m_normalStickerMaterial;
    }

    /// <summary>
    /// Material that is used when a Sticker is selected.
    /// </summary>
    /// <returns>selectedStickerMaterial</returns>
    public Material GetSelectedStickerMaterial()
    {
        return m_selectedStickerMaterial;
    }

    /// <summary>
    /// Material that is used when a Sticker is on destroyer object.
    /// </summary>
    /// <returns>destroyStickerMaterial</returns>
    public Material GetDestroyStickerMaterial()
    {
        return m_destroyStickerMaterial;
    }

    /// <summary>
    /// Tracks if Stickerbook is being dragged.
    /// </summary>
    /// <returns></returns>
    public bool GetDragBool()
    {
        return m_dragIsOn;
    }
    #endregion
    #region SETTERS
    /// <summary>
    /// Sets main camera to use UI culling mask.
    /// </summary>
    public void SetCullingMaskToUI()
    {
        mainCamera.cullingMask = uiMask;
    }
    /// <summary>
    /// Sets main camera to use default culling mask.
    /// </summary>
    public void SetCullingMaskToEverything()
    {
        mainCamera.cullingMask = everythingMask;
    }
    #endregion

    public void MoveStickerbook(Transform newPos)
    {
        if(newPos.localPosition != transform.localPosition)
        {
            viewportImage.enabled = !viewportImage.enabled;
        }
        transform.localPosition = newPos.localPosition;
    }
}
