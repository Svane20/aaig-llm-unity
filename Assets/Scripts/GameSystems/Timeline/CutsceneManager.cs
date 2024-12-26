using System;
using System.Collections.Generic;
using Dialogue.Objects;
using GameSystems.CustomEventSystems;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.Dialogue;
using PlayerControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Utilities;

namespace GameSystems.Timeline
{
    public class CutsceneManager : Singleton<CutsceneManager>
    {
        private List<TimelineAsset> _cutscenes = new List<TimelineAsset>();
        private List<TextAsset> _dialogues = new List<TextAsset>();
        
        private void OnEnable()
        {
            if (!(_cutscenes.Count > 0))
            {
                var assets = Resources.LoadAll<TimelineAsset>("Cutscenes/Timeline");
                _cutscenes.AddRange(assets);
            }
            if (!(_dialogues.Count > 0))
            {
                var assets = Resources.LoadAll<TextAsset>("DialogueDesigner/OutPut");
                _dialogues.AddRange(assets);
            }
            CutsceneHandler.Instance.StartCutsceneWithDialogue += PlayCutsceneWithDialogue;
            CutsceneHandler.Instance.StartCutsceneWithNoDialogue += PlayCutsceneWithNoDialogue;
            TriggerCutSceneHandler.Instance.TriggerCutScene += PlayCutsceneWithDialogue;
            CutsceneHandler.Instance.StartCutsceneWithDialogueName += PlayCutsceneWithDialogueName;
        }

        private void OnDisable()
        {
            if (CutsceneHandler.Instance != null)
            {
                CutsceneHandler.Instance.StartCutsceneWithDialogue -= PlayCutsceneWithDialogue;
                CutsceneHandler.Instance.StartCutsceneWithNoDialogue -= PlayCutsceneWithNoDialogue;
            }
        }


        private void PlayCutsceneWithDialogue(TextAsset json, string cutscene, InteractableDirection talkDirection)
        {
            var director = GameObject.Find("GameManagers").transform.Find("TimelineManager")
                .GetComponent<PlayableDirector>();
            director.playableAsset = _cutscenes.Find(x => x.name == cutscene);
            DialogueHandleUpdate.Instance.OnUpdateCanvas();
            director.Play();
            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            if (GameObject.Find("TutorialCanvas") != null && GameObject.Find("TutorialCanvas").activeSelf)
            {
                GameObject.Find("TutorialCanvas").gameObject.SetActive(false);
            }

            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            director.stopped += dir =>
            {
                GameObject.Find("Player").GetComponent<Animator>().enabled = false;
                switch (talkDirection)
                {
                    case InteractableDirection.Up:
                        GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite = 
                            GameObject.Find("Player").GetComponent<PlayerController>().idleSprites[0];
                        break;
                    case InteractableDirection.Down:
                        GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite = 
                            GameObject.Find("Player").GetComponent<PlayerController>().idleSprites[1];
                        break;
                    case InteractableDirection.Left:
                        GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite = 
                            GameObject.Find("Player").GetComponent<PlayerController>().idleSprites[2];
                        break;
                    case InteractableDirection.Right:
                        GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite = 
                            GameObject.Find("Player").GetComponent<PlayerController>().idleSprites[3];
                        break;
                }
                PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.performed += Interact;
                InteractionHandler.Instance.OnStartCutscene(json);
                //InteractionHandler.Instance.OnStartCutscene(json);
            };
        }

        private void PlayCutsceneWithNoDialogue(string cutscene)
        {
            var director = GameObject.Find("GameManagers").transform.Find("TimelineManager")
                .GetComponent<PlayableDirector>();
            director.playableAsset = _cutscenes.Find(x => x.name == cutscene);
            DialogueHandleUpdate.Instance.OnUpdateCanvas();
            director.Play();
            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            if (director.playableAsset.name == "imouttahere")
            {
                director.stopped += director => SceneManager.LoadScene(19);
                return;
            }
            director.stopped += dir =>
            {
                PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Enable();
            };
        }

        private void PlayCutsceneWithDialogueName(string json, string cutscene)
        {
            var director = GameObject.Find("GameManagers").transform.Find("TimelineManager")
                .GetComponent<PlayableDirector>();
            director.playableAsset = _cutscenes.Find(x => x.name == cutscene);
            DialogueHandleUpdate.Instance.OnUpdateCanvas();
            director.Play();
            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            if (GameObject.Find("TutorialCanvas") != null && GameObject.Find("TutorialCanvas").activeSelf)
            {
                GameObject.Find("TutorialCanvas").gameObject.SetActive(false);
            }

            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            director.stopped += dir =>
            {
                PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.performed += Interact;
                InteractionHandler.Instance.OnStartCutscene(_dialogues.Find(asset => asset.name == json));
                //InteractionHandler.Instance.OnStartCutscene(json);
            };
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