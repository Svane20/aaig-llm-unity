using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int id;
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameEvents.current.SignTriggerEnter(id);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameEvents.current.SignTriggerExit(id);
    }
}
