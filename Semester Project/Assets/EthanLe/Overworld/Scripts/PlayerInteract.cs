// Ethan Le (2/16/2026): 
using UnityEngine;
using UnityEngine.InputSystem; 

/** 
 * Script that allows player to interact with nearby objects.
**/  
public class PlayerInteract : MonoBehaviour
{
    /** 
     * Constant function to look out for left mouse click on nearby item:
     * If the player's Collider2D component enters the range of the interactable item, then the item can be picked up.
    **/ 
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Keep a lookout for left mouse click to interact with nearby item.
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // Convert mouse position (where the click happened) into game world position. 
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld); // Get the Collider2D component of the GameObject that the mouse clicked on (AKA, is overlapping). 

            if (hit != null) // If the clicked GameObject has a Collider2D component, 's Collider2D belongs to this interactable item, 
            {
                IInteractable interactable = hit.GetComponent<IInteractable>(); // then try getting the clicked GameObject's IInteractable interface script. 

                if (interactable != null) // If the clicked GameObject has an IInteractable interface script, that means it is an interactable item. 
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position); // Get the distance between the player ("transform.position") and the clicked interactable item ("hit.transform.position"). 

                    if (distance <= 4.0f) // If the interactable item is within 4.0 units of the player, 
                    {
                        interactable.Interact(GetComponent<playerOverworld>()); // then call the "Interact()" function from Interactable script (which already has IInteractable included) to interact with the item. 
                    }
                }
            }
        } 
    }
}