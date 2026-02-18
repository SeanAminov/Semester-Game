// Ethan Le (2/18/2026):
using UnityEngine; 

/** 
 * Script to store the items' data PERMANENTLY instead of just being a scene object. 
**/ 
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject 
{
    public string ItemName; // Name of the item. 
    public Sprite Icon; // Sprite component for the icon of the item.
    public bool Stackable; // Stackable = duplicates of the same item can be stored in one inventory slot. 
    public int MaxStack = 1; // Max # of this particular item that can be stored in one inventory slot. 
}