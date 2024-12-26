using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public static string LastSceneName;
    private void Start()
    {
        LastSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(LastSceneName);

    }

}
