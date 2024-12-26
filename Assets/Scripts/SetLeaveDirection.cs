using System;
using System.Collections;
using System.Collections.Generic;
using GameSystems.Combat;
using GameSystems.CustomEventSystems;
using UnityEngine;

public class SetLeaveDirection : MonoBehaviour
{
    public LeaveDirection leaveDirection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoadHandler.Instance.OnStoreLeaveDirection(leaveDirection);
        }
    }
}
