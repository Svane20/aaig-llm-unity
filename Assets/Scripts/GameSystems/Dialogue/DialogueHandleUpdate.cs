using System;
using UnityEngine;
using Utilities;

namespace GameSystems.Dialogue
{
    public class DialogueHandleUpdate : Singleton<DialogueHandleUpdate>
    {
        public event Action UpdateCanvas;
        public event Action<TextAsset> UpdateJson;
        public event Action UnloadJson;
        

        public void OnUpdateCanvas()
        {
            UpdateCanvas?.Invoke();
        }

        public void OnUpdateJson(TextAsset asset)
        {
            UpdateJson?.Invoke(asset);
        }

        public void OnUnloadJson()
        {
            UnloadJson?.Invoke();
        }
    }
}
