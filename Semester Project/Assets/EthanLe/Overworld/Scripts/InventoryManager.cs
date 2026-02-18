// Ethan Le (2/17/2026): 
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 
using UnityEngine.Events; 

/** 
 * Script for handling the player's inventory (holds hints, items, etc.).
 * A ScriptableObject singleton, so only one instance exists in the game. 
**/ 
//[CreateAssetMenu(fileName = "InventoryManager", menuName = "Scriptable Objects/Inventory Manager")]
public class InventoryManager : MonoBehaviour  
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxInventorySize = 10; // Max number of unique items the player can hold. 

    // Events for UI and other systems to listen to: 
    [System.Serializable] 
    public class InventoryUpdatedEvent : UnityEvent { }
    public InventoryUpdatedEvent OnInventoryUpdated = new InventoryUpdatedEvent(); // Trigger Unity Event when inventory changes. 

    [System.Serializable]
    public class ItemAddedEvent : UnityEvent { }
    public ItemAddedEvent OnItemAdded = new ItemAddedEvent(); // Triggered when player collects a new item. 
    
    [System.Serializable]
    public class ItemRemovedEvent : UnityEvent { }
    public ItemRemovedEvent OnItemRemoved = new ItemRemovedEvent(); // Triggered when a player discards an item. 

    // Inventory list (a list of slots filled with DATA of obtained items). 
    private List<InventorySlot> _inventory = new(); 

    // Singleton instance pattern for easy access: 
    private static InventoryManager _instance; 
    
    public static InventoryManager Instance 
    {
        get 
        {  
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>(); 
                if (_instance == null)
                {
                    Debug.LogError("InventoryManager is null!"); 
                }
            }

            return _instance; 
        }
    } 

    // Create public versions of the private UI and logic properties: 
    // This is to ensure we do not directly manipulate the original private variables immediately (safer). 
    public List<InventorySlot> Inventory => new List<InventorySlot>(_inventory); // Return a copy of the private _inventory List for safety. 
    public int MaxSize => maxInventorySize; // Return a copy of the private maxInventorySize int for safety. 
    public int ItemCount => _inventory.Count; // Return a copy of the number of items currently in the private _inventory List for safety. 
    public bool IsFull => _inventory.Count >= maxInventorySize; // Set Full flag as true if player is at the max inventory limit. 
    // 0 <= -1 

    // Called when the ScriptableObject is loaded (if using persistent storage):
    /*private void Awake() 
    {
        LoadInventory(); 
    }*/

    /**
     * Add/update the item's Inventory Slot in the player's inventory if not full. 
    **/ 
    public void AddItem(ItemData item) 
    {
        if (item.Stackable) // Make sure more than 1 of this item can be stored. 
        {
            // If we can store more than 1 of this item, find its appropriate existing slot in the current Inventory. 
            var existingSlot = _inventory.Find(s => s.UniqueItem == item && s.Quantity < item.MaxStack); 

            if (existingSlot != null) // If the existing slot exists, update its quantity count. 
            {
                existingSlot.Quantity++; // Increment the quantity of this existing item. 
                OnInventoryUpdated?.Invoke(); // Fire event that is being listened to in the InventoryUI.cs script that resets the inventory. 
                return; 
            }
        
        }
        
        if (_inventory.Count < maxInventorySize) // If it is a brand new unique item being obtained, 
        {
            _inventory.Add(new InventorySlot(item)); // then add it into a new SEPARATE Inventory slot. 
            OnInventoryUpdated?.Invoke(); // Fire event that is being listened to in the InventoryUI.cs script that resets the inventory. 
        }
    }

    /** 
     * Discard a copy of a unique item from the player's inventory. 
    **/
    public bool DiscardItem(InventorySlot item)
    {
        if (_inventory.Contains(item))
        {
            _inventory.Remove(item); 
            OnItemRemoved?.Invoke();
            OnInventoryUpdated?.Invoke(); 
            return true; 
        }

        return false; 
    }

}