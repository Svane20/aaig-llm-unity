using System;
using UnityEngine;
using Utilities;

namespace PlayerControl
{
    public class PlayerActionControlsManager : Singleton<PlayerActionControlsManager>
    {
        public PlayerActionControls PlayerControls;

        private void Awake()
        {
            PlayerControls = new PlayerActionControls();
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }

        private void OnEnable()
        {
            PlayerControls.Enable();
        }

    }
}