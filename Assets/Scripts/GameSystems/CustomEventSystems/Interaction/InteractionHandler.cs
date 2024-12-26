using System;
using System.Collections;
using GameSystems.Dialogue;
using GameSystems.Dialogue.Dialogue_Json_Classes;
using UnityEngine;
using Utilities;

namespace GameSystems.CustomEventSystems.Interaction
{
    public class InteractionHandler : Singleton<InteractionHandler>
    {
        public event Action Interact;
        public event Action<GameObject, bool> LookingAt;
        public event Action<Node> UpdateNode;
        public event Action<TextAsset> StartCutscene;
        public event Action EndCutscene;
        public event Action<int> ShowBubble;
        
        public event Action<int> HideBubble;

        public event Action<int> LevelAnimInt;
        public event Action<string> LevelAnimName;
        public event Action LevelAnimPrevious;
        
        public  void OnInteract()
        {
            Interact?.Invoke();
        }

        public void OnLookingAt(GameObject go, bool inRange)
        {
            LookingAt?.Invoke(go, inRange);
        }

        public void OnUpdateNode(Node obj)
        {
            UpdateNode?.Invoke(obj);
        }


        public void OnStartCutscene(TextAsset json)
        {
            StartCutscene?.Invoke(json);
        }

        public void OnEndCutscene()
        {
            EndCutscene?.Invoke();
        }

        public void OnShowBubble(int id)
        {
            ShowBubble?.Invoke(id);
        }

        public void OnHideBubble(int id)
        {
            HideBubble?.Invoke(id);
        }

        public void OnLevelAnimInt(int levelIndex)
        {
            LevelAnimInt?.Invoke(levelIndex);
        }
        
        public void OnLevelAnimName(string levelName)
        {
            LevelAnimName?.Invoke(levelName);
        }

        public void OnLevelAnimPrevious()
        {
            LevelAnimPrevious?.Invoke();
        }
    }
}