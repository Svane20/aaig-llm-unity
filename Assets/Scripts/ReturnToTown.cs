using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToScene : MonoBehaviour
{
    public void ReturnToSceneZero()
    {
        SceneManager.LoadScene(0); // Load the scene with build index 0
    }
}

