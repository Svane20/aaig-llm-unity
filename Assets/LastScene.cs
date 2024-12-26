using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LastScene : MonoBehaviour
{
    private PlayableDirector playdir;
    private void OnEnable()
    {
        playdir = GameObject.Find("GameManagers").transform.GetChild(0).GetComponent<PlayableDirector>();
        if (playdir.playableAsset.name == "imouttahere")
        {
            playdir.stopped += director => SceneManager.LoadScene(19);
        }
    }
    
}
