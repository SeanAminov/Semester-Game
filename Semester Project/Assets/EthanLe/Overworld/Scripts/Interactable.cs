// Ethan Le (2/16/2026):
using UnityEngine;
using TMPro; // For TextMeshPro component. 

/** 
 * Script that allows player to interact with interactable items (e.g., pieces of paper). 
 * Flexible for interacting with any object (doors, papers, etc.) -- just add additional functions. 
**/ 
public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    [SerializeField] private ItemData itemData; // For the data of the actual item. 

    // Create public copy of the item's data to reference in other files:
    public ItemData dupItemData => itemData;

    private GameObject glow; // For visual effect to indicate an interactable item is nearby. 
    
    private playerOverworld player; // playerOverworld reference. 
    public GameObject popupTextPrefab; // Assign Message prefab through the Inspector. 
    public Transform popupContainer; // Assign UI PopupContainer through the Inspector. 

    void Start()
    {
        glow = transform.Find("Glow")?.gameObject; // Find the Interactable GameObject's child named "Glow. 
        if (glow != null)
        {
            glow.SetActive(false); // The glow effect starts out as disabled. 
        }

        player = GameObject.FindObjectOfType<playerOverworld>(); // Automatically get the playerOverworld reference. 
        if (player == null) // Safety check. 
        {
            return; 
        }
    }

    void Update()
    {
        if (glow != null && player != null) // Check to ensure that the script found the "Glow" GameObject and the "playerOverworld" GameObject.
        {
            float distance = Vector2.Distance(player.transform.position, transform.position); // Get the distance between the player ("player.transform.position") and this interactable item ("transform.position"). 

            glow.SetActive(distance <= 2.5f); // Turn on the glow effect if player is nearby the interactable item.
        } 
    }

    public void Interact() // Implement the "Interact()" function defined by the IInteractable.cs interface. 
    {
        if (!InventoryManager.Instance.IsFull) // If the player's inventory (one single instance across the whole game) is not yet full, 
        { 
            InventoryManager.Instance.AddItem(itemData); // Add the data of the obtained item to the player's inventory (as an Inventory slot). 
            ShowGainedPopup(); // Call function to show the text after gaining with the item. 
            Destroy(gameObject); // Remove item from the world after picking it up. 
        }

        else // Otherwise, if player's inventory is full, 
        {
            ShowFullPopup(); // Call function to show text with FULL warning message. 
        }
    }

    private void ShowGainedPopup()
    {
        if (popupTextPrefab != null && popupContainer != null)
        {
            // Create an instanceof the Message prefab as a child of the PopupContainer: 
            GameObject message = Instantiate(popupTextPrefab, popupContainer); 

            // Set the text:
            TMP_Text textBox = message.GetComponent<TMP_Text>(); // Get the TMP_Text component of the Message prefab. 
            if (textBox != null)
            {
                textBox.text = "You obtained an object! This works."; 
            }

            // Destroy the Message prefab after two seconds: 
            Destroy(message, 4f); 
        }
    }

    private void ShowFullPopup()
    {
        if (popupTextPrefab != null && popupContainer != null)
        {
            // Create an instanceof the Message prefab as a child of the PopupContainer: 
            GameObject message = Instantiate(popupTextPrefab, popupContainer); 

            // Set the text:
            TMP_Text textBox = message.GetComponent<TMP_Text>(); // Get the TMP_Text component of the Message prefab. 
            if (textBox != null)
            {
                textBox.text = "Your inventory is full! Discard an item!"; 
            }

            // Destroy the Message prefab after two seconds: 
            Destroy(message, 4f); 
        }
    }
}