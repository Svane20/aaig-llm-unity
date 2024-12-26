using System;
using Dialogue.Objects;
using GameSystems.Combat;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.Dialogue;
using PlayerControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Utilities;

namespace GameSystems.Timeline
{
    public class CutsceneController : MonoBehaviour
    {
        public bool isStartOfGame, playOnAwake;
        public InteractableDirection talkDirection;
        public PlayableDirector _director;
        public TextAsset json;
        private TextAsset _previousJson;
        private PlayerActionControls _playerActionControls;
        private DialogueUIManager _dialogueUIManager;
        private DialogueManager _dialogueManager;
        private SceneLoadManager _sceneManager;
        private CutsceneManager _cutsceneManager;

        private void Awake()
        {
            _playerActionControls = PlayerActionControlsManager.Instance.PlayerControls;
            //InteractionHandler.Instance.EndCutscene += () => _playerActionControls.Land.Interact.performed -= Interact;
            _dialogueUIManager = DialogueUIManager.Instance;
            _dialogueManager = DialogueManager.Instance;
            _sceneManager = SceneLoadManager.Instance;
            _cutsceneManager = CutsceneManager.Instance;
        }

        private void OnEnable()
        {
            if (playOnAwake || isStartOfGame)
            {
                CutsceneHandler.Instance.OnStartCutsceneWithDialogue(json, _director.playableAsset.name, talkDirection);
            }
        }

        private void OnValidate()
        {
            UpdateJson();
        }
        
        public void UpdateJson()
        {
            if (json != null)
            {
                if (_previousJson == null) _previousJson = new TextAsset();
                if (json.GetType() != typeof(TextAsset))
                    throw new InvalidOperationException("File can only be Text Assets");
                if (_previousJson.ToString().Equals(json.ToString()) ^ _previousJson.name == json.name) return;
                _previousJson = json;
                CustomUtils.PrettifyJson(json);
            }
            
        }
        
        private void Interact(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                InteractionHandler.Instance.OnInteract();
            }
        }
    }
}
