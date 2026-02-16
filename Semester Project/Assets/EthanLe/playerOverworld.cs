// Ethan Le (2/15/2026): 
using System; 
using System.Collections;
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.InputSystem; // New modern Unity input. 
using UnityEngine.SceneManagement; 

/** Script that allows for player movement based on position of left-mouse click: 
 * Uses Vectors to calculate movement direction of the player (its Rigidbody2D component), normalizing the vector when player is moving.
 **/ 
public class playerOverworld : MonoBehaviour
{
    // Player visuals: 
    private Rigidbody2D rb; // Body of the player (component that moves). 
    private Camera mainCamera; // Convert the mouse position to game world position. 

    // Player movement: 
    [NonSerialized] public float speed = 8f; 
    private bool moving = false; 
    private Vector2 target; // For storing the target for the player's Rigidbody2D to move toward every frame. 

    bool colliding = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the sprite of the player. 
        if (rb == null) // Safety check. 
        {
            return; 
        }

        mainCamera = Camera.main; // Cache the main camera reference. 
        if (mainCamera == null) // Safety check.
        {
            return; 
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Keep a lookout for left mouse click to move character. 
        { 
            Vector3 mouseWorld3D = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // Convert mouse position (where the click happened) into game world position. 
            target = new Vector2(mouseWorld3D.x, mouseWorld3D.y); // Create a 2D vector based on the clicked world position and store it in "target" so we can track where the Rigidbody2D of the player needs to travel to. 
            moving = true; // Change flag to true so FixedUpdate() can move the Rigidbody2D of the player.  
        }
    }

    void FixedUpdate() // Performed only when a left mouse click happens. 
    {
        if (moving) 
        {
            // Calculate direction and apply velocity to move the player: 
            Vector2 direction = (target - (Vector2)transform.position); // Create direction vector from the player's current position to the clicked target area (do not normalize it yet as we need its original magnitude to check if the player's Rigidbody2D is close to the clicked target area). 
            
            if (direction.magnitude < 0.1f) // If the player's Rigidbody2D is close to the target area, 
            {
                rb.linearVelocity = Vector2.zero; // Stop the player's movement. 
                moving = false; // Change flag back to false (prevents interrupted left-mouse clicks). 
            }

            else // Otherwise, keep moving the player's Rigidbody2D toward the target area (normalize the direction vector to do this). 
            {
                rb.linearVelocity = direction.normalized * speed; // Move the player's Rigidbody2D based on the direction vector with the defined speed. 
            }
        }
    }
}