// Ethan Le (2/18/2026):

/**
 * Script to create an inventory slot for a unique item. 
**/ 
[System.Serializable]
public class InventorySlot 
{
    public ItemData UniqueItem; // Data of a unique item. 
    public int Quantity; // # of this unique item player has. 

    // Get info of the NEWLY obtained item to be displayed in Inventory Slot: 
    public InventorySlot(ItemData newItem, int quantity = 1)
    {
        UniqueItem = newItem; 
        Quantity = quantity; // Store one copy of this NEWLY obtained item. 
    }
}