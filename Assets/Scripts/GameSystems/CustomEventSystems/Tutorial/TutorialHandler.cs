using System;
using UnityEngine.InputSystem;
using Utilities;

namespace GameSystems.CustomEventSystems.Tutorial
{
    public sealed class TutorialHandler : Singleton<TutorialHandler>
    {

        public event Action<InputAction.CallbackContext> TutorialButtonPressed;

        public void OnTutorialButtonPressed(InputAction.CallbackContext ctx)
        {
            TutorialButtonPressed?.Invoke(ctx);
        }
        
    }
}