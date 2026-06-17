// Ethan Le (6/16/2026):
using System.Collections; // For IEnumerator. 
using UnityEngine;
using UnityEngine.SceneManagement; // For Scene management. 

/**
 * Script to toggle between Overworld Scene and Combat Scene:
**/
public class MenuScript : MonoBehaviour
{
    public void LoadScene(int scene)
    {
        StartCoroutine(LoadSceneDelayed(scene)); // Invoke method to load the appropriate scene. 
    }

    IEnumerator LoadSceneDelayed(int scene)
    {
        yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds before loading the other scene. 

        if (scene < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene); // Load the appropriate scene. 
        }

        else
        {
            Debug.LogError("Scene " + scene + " could not be found in build settings."); 
        }
    }
} 