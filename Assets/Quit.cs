using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{
    private PlayableDirector playdir;
    private void OnEnable()
    {
        playdir = GameObject.Find("GameManagers").transform.GetChild(0).GetComponent<PlayableDirector>();
        playdir.stopped += director => Application.Quit();
    }
    
}