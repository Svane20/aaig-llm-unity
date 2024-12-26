using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : MonoBehaviour
{
    public int id;

    void Start()
    {
        GameEvents.current.onSignTriggerEnter += OnSignPopUp;
        GameEvents.current.onSignTriggerExit += OnSignRemove;
    }

    private void OnSignPopUp(int id)
    {
        if (id == this.id)
        {
            Debug.Log("Popup message on sign here");
        }
    }

    private void OnSignRemove(int id)
    {
        if (id == this.id)
        {
            Debug.Log("Popup message removed");
        }
    }
}
