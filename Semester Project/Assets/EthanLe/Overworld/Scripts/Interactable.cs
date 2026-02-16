// Ethan Le (2/16/2026):
using UnityEngine;

/** 
 * Script that allows player to interact with interactable items (e.g., pieces of paper). 
 * Flexible for interacting with any object (doors, papers, etc.) -- just add additional functions. 
**/ 
public class Interactable : MonoBehaviour, IInteractable
{
    public void Interact(playerOverworld player) // Implement the "Interact()" function defined by the IInteractable.cs interface. 
    {
        player.addToInventory(this); // Add the interactable item to the player's inventory. 
        Destroy(gameObject); // Remove item from the world after picking it up. 
    }
}