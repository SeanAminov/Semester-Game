// Ethan Le (2/15/2026): 
using UnityEngine; 

/** Script is to allow for ANY object with a "...Collider2D" component to:
 * a. Be drawn in front if it has a lower Y position. 
 * b. Be drawn behind if it has a higher Y position. 
**/ 
public class YSort : MonoBehaviour 
{
    private SpriteRenderer sr; // SpriteRenderer component of an object with collision (has "...Collider2D" component). 

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // Get object's SpriteRenderer component. 
    }

    /** Function that determines what layer the object should be on:
     * a. Higher sortingOrder = drawn in front. 
     * b. Lower sortingOrder = drawn behind. 
    **/
    void LateUpdate()
    {
        // "transform.position.y" is the object's vertical position in the game world. 
        // Top-down games: object being lower than the player's feet = object should appear in front. 
        // 1. LOWER Y = HIGHER sortingOrder = drawn in front. 
        // 2. HIGHER Y = LOWER sortingOrder = drawn behind. 
        // "transform.position.y" does the opposite of previous two statements, so flip it with negative sign. 
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100); // Lower Y = higher sortingOrder (int) = drawn in front. 
        // The *100 is to prevent numbers like "1.01", "1.02", "1.03" that get rounded to int 1 (which would result in being the same layer). 
    }
}