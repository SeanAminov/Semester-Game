// Ethan Le (2/17/2026):
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

/** 
 * Script to populate Inventory UI with currently obtained items. 
**/ 
public class InventoryUI : MonoBehaviour 
{
    // Assign these UI references in the Inspector in the "InventoryUI" Canvas: 
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // Main inventory panel (show/hide).
    [SerializeField] private Transform inventoryGrid; // Grid container for items (should have a GridLayoutGroup component). 
    [SerializeField] private GameObject inventoryItemPrefab; // Prefab for each inventory item. 
    [SerializeField] private TextMeshProUGUI itemCountText; // Tracks number of items.
    [SerializeField] private TextMeshProUGUI titleText; // Title label. 

    [Header("Settings")]
    [SerializeField] private int itemsPerRow = 3; // # of items per row in the grid. 
    [SerializeField] private float itemSpacing = 10f; // Spacing between each item. 

    // Internal copy of Inventory list for this script to hold to populate UI in a future function:
    private List<InventorySlot> _allItems = new(); 

    // Event for other UI/logic to subscribe to. 
    //public System.Action<InventorySlot> OnItemSelected; // Fires when item is selected. 

    // Called when script instance is being loaded:
    private void Start()
    {
        SubscribeToEvents(); 
        ResetInventory(); 
    }

    // Called when the object is destroyed: 
    private void OnDestroy()
    {
        UnsubscribeFromEvents(); 
    }

    // Initialize Inventory UI:
    private void SubscribeToEvents()
    {
        // Subscribe to events so UI updates when inventory changes: 
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated.AddListener(ResetInventory); 
            InventoryManager.Instance.OnItemAdded.AddListener(OnItemAdded); 
            InventoryManager.Instance.OnItemRemoved.AddListener(OnItemRemoved); 
        }
    }

    // Unsubscribe from events to clean up memory after Inventory UI is closed: 
    private void UnsubscribeFromEvents()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated.RemoveListener(ResetInventory);
            InventoryManager.Instance.OnItemAdded.RemoveListener(OnItemAdded); 
            InventoryManager.Instance.OnItemRemoved.RemoveListener(OnItemRemoved); 
        }
    }

    // Refresh the entire inventory displayed (called when inventory changes):
    public void ResetInventory() // Must match Event signature of "OnInventoryUpdated" (where listener is added/removed) declared from InventoryManager.cs. 
    {
        if (InventoryManager.Instance == null) 
        {
            return;
        }
        
        // Get the public copy of the inventory List<InventorySlot> from InventoryManager instance:
        _allItems = InventoryManager.Instance.Inventory; 

        PopulateInventoryGrid(); // Populate the Inventory UI grid. 
        UpdateItemCount(); // Update number of items in your current inventory. 
    }

    // Call ResetInventory when an item is added to the player's inventory:
    private void OnItemAdded() // Must match Event signature of "OnItemAdded" (where listener is added/removed) declared from InventoryManager.cs.
    {
        ResetInventory(); // Refreshes the Inventory UI. 
    }

    // Call ResetInventory when an item has been discarded from the player's inventory: 
    private void OnItemRemoved() // Must match Event signature of "OnItemRemoved" (where listener is added/removed) declared from InventoryManager.cs. 
    {
        ResetInventory(); // Refreshes the Inventory UI. 
    }

    // Function to populate the inventory grid with obtained items: 
    private void PopulateInventoryGrid()
    {
        // Clear existing items to start anew: 
        ClearInventoryGrid(); 

        if (inventoryGrid == null || inventoryItemPrefab == null)
        {
            return; // Ensure that you set up and assigned the UI references in the "InventoryUI" Canvas's Inspector first before populating the UI. 
        }

        // Set up the grid layout: 
        var gridLayout = inventoryGrid.GetComponent<GridLayoutGroup>(); // Get the GridLayoutGroup component from the inventoryGrid grid container. 
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // Constrain the grid layout to the determined column count from the GridLayoutGroup component in the Unity Inspector. 
            gridLayout.constraintCount = itemsPerRow; // Constrain the grid layout to the assigned row count from this script. 
            gridLayout.spacing = new Vector2(itemSpacing, itemSpacing); // Space out each item by the assigned itemSpacing variable from this script. 
        }

        // Loop to spawn the items:
        foreach (var item in _allItems)
        {
            GameObject gObj = Instantiate(inventoryItemPrefab, inventoryGrid); // Create an instance of the item from the List<InventorySlot>. 

            InventoryItemUI ui = gObj.GetComponent<InventoryItemUI>(); // Get the InventoryItemUI component from the item instance. 

            if (ui != null) // Make sure the item has UI component before setting up its UI display. 
            {
                ui.Setup(item); // Call function from InventoryItemUI to set up its UI display (based on Inventory Slots). 
            }
        }
    }

    // Function to update the count of items left in the player's inventory (and display title): 
    private void UpdateItemCount()
    {
        if (itemCountText != null) 
        {
            itemCountText.text = $"Items: {_allItems.Count} / {InventoryManager.Instance.MaxSize}"; 
        }

        if (titleText != null)
        {
            titleText.text = $"Inventory"; 
        }
    }

    // Function to clear all items from the inventory grid (for when refreshing the inventory):
    private void ClearInventoryGrid()
    {
        if (inventoryGrid == null)
        {
            return;
        }

        for (int i = inventoryGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(inventoryGrid.GetChild(i).gameObject); 
        }
    }
} 