using System;
using Dialogue.Objects;
using UnityEngine;
using Utilities;

namespace GameSystems.CustomEventSystems
{
    public class TriggerCutSceneHandler : Singleton<TriggerCutSceneHandler>
    {
        public event Action<TextAsset, string, InteractableDirection> TriggerCutScene;

        public void OnTriggerCutScene(TextAsset json, string cutscene, InteractableDirection direction)
        {
            TriggerCutScene?.Invoke(json, cutscene, direction);
        }
    }
}
