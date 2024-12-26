using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onSignTriggerEnter;

    public void SignTriggerEnter(int id)
    {
        if (onSignTriggerEnter != null)
        {
            onSignTriggerEnter(id);
        }
    }

    public event Action<int> onSignTriggerExit;

    public void SignTriggerExit(int id)
    {
        if (onSignTriggerExit != null)
        {
            onSignTriggerExit(id);
        }
    }
}
