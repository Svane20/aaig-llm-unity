using System;
using GameSystems.Combat;
using UnityEngine;
using Utilities;

namespace GameSystems.CustomEventSystems
{
    public class SceneLoadHandler : Singleton<SceneLoadHandler>
    {
        public event Action<string> StoreLastSceneName;
        public event Action<Vector3> StoreLastPosition;
        public event Func<string> GetLastSceneName;
        public event Func<Vector3?> GetLastPosition;
        public event Action<LeaveDirection> StoreLeaveDirection;
        public event Func<LeaveDirection?> GetLastLeaveDirection;

        public event Action<Vector3> storeCamera;
        
        public event Func<Vector3?> getCamera; 

        public void OnStoreLastSceneName(string lastSceneName)
        {
            StoreLastSceneName?.Invoke(lastSceneName);
        }

        public void OnStoreLastPosition(Vector3 obj)
        {
            StoreLastPosition?.Invoke(obj);
        }

        public string OnGetLastSceneName()
        {
            return GetLastSceneName?.Invoke();
        }

        public Vector3? OnGetLastPosition()
        {
            return GetLastPosition?.Invoke();
        }

        public void OnStoreLeaveDirection(LeaveDirection obj)
        {
            StoreLeaveDirection?.Invoke(obj);
        }

        public LeaveDirection? OnGetLastLeaveDirection()
        {
            return GetLastLeaveDirection?.Invoke();
        }

        public void OnStoreCamera(Vector3 obj)
        {
            storeCamera?.Invoke(obj);
        }

        public Vector3? OnGetCamera()
        {
            return getCamera?.Invoke();
        }
    }
}