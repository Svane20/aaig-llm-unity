using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.Dialogue.Dialogue_Json_Classes;
using PlayerControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GameSystems.Dialogue
{
    public class DialogueUIManager : Singleton<DialogueUIManager>
    {
        private List<GameObject> _dialogueBoxes;
        private GameObject PreviousDialogueBox;
        private bool TypeWriterRunning;
        private bool CanvasPresent;
        private void Awake()
        {
            DialogueUIHandler.Instance.ExitDialogue += CloseDialogue;
            DialogueUIHandler.Instance.ShowDialogue += DisplayDialogue;
            DialogueHandleUpdate.Instance.UpdateCanvas += UpdateCanvasRef;
            SceneManager.sceneLoaded += (arg0, mode) => UpdateCanvasRef();
        }

        private void OnEnable()
        {
            UpdateCanvasRef();
        }
        

        private void UpdateCanvasRef()
        {
            if (!GameObject.Find("DialogueCanvas"))
            {
                CanvasPresent = false;
                return;
            }
            CanvasPresent = true;
            _dialogueBoxes = null;
            _dialogueBoxes = new List<GameObject>();
            var dialogueCanvas = GameObject.Find("DialogueCanvas");
            for (var i = 0; i < dialogueCanvas.transform.childCount; i++)
            {
                _dialogueBoxes.Add(dialogueCanvas.transform.GetChild(i).gameObject);
            }

        }

        private IEnumerator DisplayDialogue(Node currentNode)
        {
            if(CanvasPresent)
                yield return BranchedDialogueOrNot(currentNode, currentNode.choices != null);
        }

        private IEnumerator BranchedDialogueOrNot(Node currentNode, bool hasChoices)
        {
           yield return StartCoroutine(HandleBox(currentNode,currentNode.is_box,hasChoices));
        }

        private IEnumerator HandleBox(Node currentNode, bool isBox, bool hasChoices)
        {
            if (PreviousDialogueBox != null)
            {
                PreviousDialogueBox.transform.transform.Find("Dialogue").Find("Continue").gameObject.SetActive(false);
            }
            if (PreviousDialogueBox != null && isBox)
            {
                _dialogueBoxes.Find(box => box.name == "DialogueBoxLeft").gameObject.SetActive(false);
                _dialogueBoxes.Find(box => box.name == "DialogueBoxRight").gameObject.SetActive(false);
            }

            if (PreviousDialogueBox != null && PreviousDialogueBox.name == "DialogueBox Top")
            {
                _dialogueBoxes.Find(box => box.name == "DialogueBox Top").gameObject.SetActive(false);
            }

            switch (isBox)
            {
                case true:
                {
                    var dialogueBox = _dialogueBoxes.Find(box => box.name == "DialogueBox Top");
                    PreviousDialogueBox = dialogueBox;
                    dialogueBox.gameObject.SetActive(true);
                    TurnDecisionPanelOff(dialogueBox);
                    yield return StartCoroutine(TypeWriter(currentNode.Text, dialogueBox, currentNode, hasChoices));
                    HasDecision(currentNode, hasChoices, dialogueBox, isBox);
                    break;
                }
                case false:
                {
                    GameObject dialogueBox;
                    if (currentNode.character == "Player")
                    {
                        dialogueBox = _dialogueBoxes.Find(box => box.name == "DialogueBoxLeft");
                        dialogueBox.transform.Find("Dialogue").GetComponent<RectTransform>().sizeDelta = new Vector2(219.52f,48.632f);
                        dialogueBox.transform.Find("Name").GetChild(0).GetComponent<Text>().text = currentNode.character;
                        dialogueBox.transform.Find("Name").GetComponent<RectTransform>().position = new Vector3(184.7f, 230.2f);
                    }
                    else
                    {
                        dialogueBox = _dialogueBoxes.Find(box => box.name == "DialogueBoxRight");
                        dialogueBox.transform.Find("Dialogue").GetComponent<RectTransform>().sizeDelta = new Vector2(219.52f,48.632f);
                        dialogueBox.transform.Find("Name").GetChild(0).GetComponent<Text>().text = currentNode.character;
                        dialogueBox.transform.Find("Name").position = new Vector3(1755.0f, 230.2f);
                    }
                    PreviousDialogueBox = dialogueBox;
                    dialogueBox.gameObject.SetActive(true);
                    TurnDecisionPanelOff(dialogueBox);
                    yield return StartCoroutine(TypeWriter(currentNode.Text, dialogueBox, currentNode, hasChoices));
                    HasDecision(currentNode,hasChoices, dialogueBox, isBox);
                    break;
                }
            }
        }

        private void HasDecision(Node currentNode,bool hasChoices, GameObject dialogueBox, bool isBox)
        {
            if (dialogueBox.transform.Find("Decision Panel") == true)
            {
                dialogueBox = dialogueBox.transform.Find("Decision Panel").gameObject;
                dialogueBox.gameObject.SetActive(false);
            }
            if (dialogueBox.transform.Find("Decide") == true)
            {
                
                var decision = dialogueBox.transform.Find("Decide");
                decision.gameObject.SetActive(hasChoices);
                if (hasChoices)
                {
                    dialogueBox.gameObject.SetActive(true);
                    RunTween(dialogueBox);
                    for (var i = 0; i < decision.childCount; i++)
                    { 
                        var currentChild = decision.GetChild(i);
                        var textButtonText = currentChild.GetComponentInChildren<Text>();
                        textButtonText.text = currentNode.choices[i].Text;
                        var rect = currentChild.GetComponent<RectTransform>();
                        rect.anchorMax = new Vector2(100f, 30f);
                        rect.anchorMin = new Vector2(50f, 19f);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalculateLengthOfMessage(currentNode.choices[i].Text, textButtonText) + 20);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);
                        currentChild.GetComponent<Button>().onClick.AddListener(delegate { Choice(currentChild.gameObject, currentNode, isBox, dialogueBox); });
                    }
                    PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Disable();
                }
            }
        }
        
        private int CalculateLengthOfMessage(string message, Text text)
        {
            var totalLength = 0;
 
            var myFont = text.font;  //chatText is my Text component
            var characterInfo = new CharacterInfo();
 
            var arr = message.ToCharArray();
 
            foreach(var c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);  
 
                totalLength += characterInfo.advance;
            }
 
            return totalLength;
        }
        

        private void RunTween(GameObject target)
        {
            if (target.transform.parent.name == "DialogueBox Top")
            {
                var position = target.transform.position;
                LeanTween.move(target, new Vector2(position.x, position.y - 160), 1f);
            }
        }

        private void TurnDecisionPanelOff(GameObject dialogueBox)
        {
            if (dialogueBox.transform.Find("Decision Panel") == true)
            {
                var decisionPanel = dialogueBox.transform.Find("Decision Panel").gameObject;
                decisionPanel.gameObject.SetActive(false);
            }
        }


        private void Choice(GameObject currentChild, Node currentNode, bool isBox, GameObject currentParent)
        {
            
            switch (currentChild.gameObject.name)
            {
                case "Option 1":
                    HandleChoice(0,currentChild, currentNode, isBox, currentParent);
                    break;
                case "Option 2":
                    HandleChoice(1,currentChild, currentNode, isBox, currentParent);
                    break;
            }
            
        }

        private void HandleChoice(int choice,GameObject currentChild, Node currentNode, bool isBox, GameObject currentParent)
        {
            var nodes = DialogueUIHandler.Instance.OnGetNodes();
            var nextNode = nodes.Find(node => node.node_name == currentNode.choices[choice].next);
            if (isBox)
            {
                currentParent.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                currentChild.transform.parent.gameObject.SetActive(false);
            }
            InteractionHandler.Instance.OnUpdateNode(nextNode);
            PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Enable();
            
        }

        private void CloseDialogue()
        {
            DialogueHandleUpdate.Instance.OnUnloadJson();
            if (!GameObject.Find("Player").GetComponent<Animator>().enabled)
                GameObject.Find("Player").GetComponent<Animator>().enabled = true;
            foreach (var box in _dialogueBoxes.Where(box => box.activeSelf))
            {
                if(box.gameObject.name == "EventSystem") continue;
                box.transform.Find("Dialogue").GetChild(0).GetComponent<Text>().text = string.Empty;
                box.SetActive(false);
            }
        }

        private IEnumerator TypeWriter(string text, GameObject objectToSet, Node currentNode, bool hasChoices)
        {
            if (TypeWriterRunning == false)
            {
                TypeWriterRunning = true;
                objectToSet.transform.Find("Dialogue").Find("Continue").gameObject.SetActive(false);
                var textObject = objectToSet.transform.Find("Dialogue").GetChild(0).GetComponent<Text>();
                PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Disable();
                var textToString = new StringBuilder();
                textObject.text = textToString.ToString();
                for (int i = 0; i < text.Length; i++)
                {
                    if (textObject != null)
                    {
                        textObject.text += text[i];
                        if (textObject.IsOverflowingVerticle())
                        {
                            var parentRt = objectToSet.transform.Find("Dialogue").GetComponent<RectTransform>();
                            RectTransform nameRt = null;
                            if (objectToSet.transform.Find("Name"))
                            {
                               nameRt = objectToSet.transform.Find("Name").GetComponent<RectTransform>(); 
                            }
                            var childRt = objectToSet.transform.Find("Dialogue").GetChild(0).GetComponent<RectTransform>();
                            if (nameRt is { })
                            {
                                var position = nameRt.position;
                                position = new Vector2(position.x, position.y + 0.3f);
                                nameRt.position = position;
                            }

                            var sizeDelta = parentRt.sizeDelta;
                            sizeDelta = new Vector2(sizeDelta.x ,  sizeDelta.y + 0.1f);
                            parentRt.sizeDelta = sizeDelta;
                            childRt.sizeDelta = new Vector2(sizeDelta.x - 1f, sizeDelta.y - 1f);
                        }
                        yield return new WaitForSeconds(0.04f);
                    }
                }

                if (CanvasPresent)
                {
                    objectToSet.transform.Find("Dialogue").Find("Continue").gameObject.SetActive(currentNode.choices == null);
                    if (!hasChoices)
                    {
                        PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Enable();
                    }
                    TypeWriterRunning = false;
                }
                
            }
        }
        
        
        
        
    }
}