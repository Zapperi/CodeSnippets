using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles the Inventory UI which shows player owned items.
 * </summary>
 */

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// A singleton instance of the class.
    /// </summary>
    public static InventoryUI Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Transform used as parent for spawned UI items
    /// </summary>
    [Tooltip("Transform used as parent for spawned UI items")]
    public Transform itemParent;

    /// <summary>
    /// Prefab for UI item, it must have a InventoryItemUI component
    /// </summary>
    [Tooltip("Prefab for UI item, it must have a InventoryItemUI component")]
    public GameObject itemPrefab;

    private List<InventoryItemUI> activeItems = new List<InventoryItemUI>();
    private List<InventoryItemUI> slotItems = new List<InventoryItemUI>();

    private void Start()
    {
        UIController.Instance.ShowStickerBook += HideInventory;
        UIController.Instance.HideStickerBook += ShowInventory;
    }

    private void HideInventory()
    {
        itemParent.gameObject.SetActive(false);
    }

    private void ShowInventory()
    {
        itemParent.gameObject.SetActive(true);
    }

    public void ClearItemSlots()
    {
        foreach (InventoryItemUI slot in slotItems)
        {
            slot.Retire();
        }

        slotItems.Clear();
    }

    /// <summary>
    /// Adds an empty item slot to the UI
    /// </summary>
    /// <param name="key">Key to show empty slot for</param>
    public void AddItemSlot(InteractableItemKey key)
    {
        GameObject item = Instantiate(itemPrefab, itemParent);
        InventoryItemUI itemUI = item.GetComponent<InventoryItemUI>();
        if (itemUI == null)
        {
            Debug.LogError("Item prefab in InventoryUI does not have a InventoryItemUI component");
            Destroy(item);
        }
        else
        {
            itemUI.Init(key);
            slotItems.Add(itemUI);
        }
    }

    /// <summary>
    /// Creates UI item, plays its "get" animation and adds it to list of active UI items
    /// </summary>
    /// <param name="key">Item to create a UI item from</param>
    public void AddItem(InteractableItemKey key)
    {
        // If slot already exists, make that item fill the slot
        foreach (InventoryItemUI slotItem in slotItems)
        {
            if (slotItem.itemKey == key)
            {
                slotItem.PlayItemGetAnimation();
                activeItems.Add(slotItem);
                slotItems.Remove(slotItem);
                return;
            }
        }

        // else create new item and immediately fill the slot
        GameObject item = Instantiate(itemPrefab, itemParent);
        InventoryItemUI itemUI = item.GetComponent<InventoryItemUI>();
        if (itemUI == null)
        {
            Debug.LogError("Item prefab in InventoryUI does not have a InventoryItemUI component");
            Destroy(item);
        }
        else
        {
            itemUI.Init(key);
            itemUI.PlayItemGetAnimation();
            activeItems.Add(itemUI);
        }
    }

    /// <summary>
    /// Finds item from inventory, plays its use animation and removes it from list of active UI items
    /// </summary>
    /// <param name="key">Item to find</param>
    /// <param name="targetPos">Target screen position passed to item animation</param>
    public void UseItem(InteractableItemKey key, Vector3 targetPos)
    {
        InventoryItemUI foundItem = null;
        foreach (InventoryItemUI item in activeItems)
        {
            if (item.itemKey == key)
            {
                foundItem = item;
                break;
            }
        }

        if (foundItem != null)
        {
            foundItem.PlayItemUseAnimation(targetPos);
            activeItems.Remove(foundItem);
        }
    }
}
