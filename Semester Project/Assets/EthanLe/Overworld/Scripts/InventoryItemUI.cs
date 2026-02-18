// Ethan Le (2/18/2026):
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** 
 * Script to initialize the UI for any item. 
**/ 
public class InventoryItemUI : MonoBehaviour 
{
    [SerializeField] private Image iconImage; 
    [SerializeField] private TextMeshProUGUI itemNameText; 
    [SerializeField] private TextMeshProUGUI quantityText; 

    public void Setup(InventorySlot slot) // Called in InventoryUI.cs. 
    {
        // Attributes of "slot" were defined in ItemData.cs, containing a unique item's data.
       itemNameText.text = slot.UniqueItem.ItemName; // Set up name of unique item.
       iconImage.sprite = slot.UniqueItem.Icon; // Set up icon of unique item. 
       quantityText.text = slot.Quantity.ToString(); // Get quantity from slot's Quantity attribute defined in InventorySlot.cs.  

       if (quantityText != null)
       {
            // If player has obtained a duplicate of an item, display the incremented count. Otherwise, display nothing (no number = we know there is only 1 of that item). 
            quantityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : ""; 
       }
    }
}