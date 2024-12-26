using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class NPCSwitchScene : MonoBehaviour
{
    // Name of the scene to switch to
    private int sceneIndexToLoad = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger has the tag "player"
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}