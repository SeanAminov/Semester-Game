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
    [SerializeField] private int maxInventorySize = 1; // Max number of Interactable items the player can hold. 

    // Events for UI and other systems to listen to: 
    [System.Serializable] 
    public class InventoryUpdatedEvent : UnityEvent { }
    public InventoryUpdatedEvent OnInventoryUpdated = new InventoryUpdatedEvent(); // Trigger Unity Event when inventory changes. 

    [System.Serializable]
    public class ItemAddedEvent : UnityEvent { }
    public ItemAddedEvent OnItemAdded = new ItemAddedEvent(); // Triggered when player collects a new item. 
    
    // Inventory list (not serialized by Unity, but saved/loaded manually). 
    private List<Interactable> _inventory = new List<Interactable>(); 

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
    public List<Interactable> Inventory => new List<Interactable>(_inventory); // Return a copy of the private _inventory List for safety. 
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
     * Add an interactable item into the player's inventory if not full. 
    **/ 
    public bool AddItem(Interactable item) 
    {
        if (IsFull)
        {
            return false; 
        }

        _inventory.Add(item); 
        return true; 
    }

    /** 
     * Discard an interactable item from the player's inventory. 
    **/
    public bool DiscardItem(Interactable item)
    {
        if (_inventory.Contains(item))
        {
            _inventory.Remove(item); 
            return true; 
        }

        return false; 
    }

}