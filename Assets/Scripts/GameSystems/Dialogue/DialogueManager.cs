using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dialogue.Objects;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.Dialogue.Dialogue_Json_Classes;
using GameSystems.Timeline;
using PlayerControl;
using DialogueClass = GameSystems.Dialogue.Dialogue_Json_Classes.Dialogue;
using UnityEngine;
using UnityEngine.Playables;
using Utilities;
using Random = System.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace GameSystems.Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private TextAsset _json;
        private DialogueClass Dialogue { get; set; }
        private Connections[] Connections => Dialogue?.connections;
        private List<Node> Nodes
        {
            get
            {
                return Dialogue?.nodes;
            }
        }

        private Node _currentNode;
        private Dictionary<string, Variables> _globalVariables;
        private Variables _currentVariable;
        private List<Node> _lastNode;
        private bool _endNodeRan = false;
        private bool _isCutscene = false;


        private void Awake()
        {
            _globalVariables = new Dictionary<string, Variables>();
            InteractionHandler.Instance.Interact += StartShowDialogue;
            InteractionHandler.Instance.UpdateNode += ChoiceUpdateNode;
            InteractionHandler.Instance.StartCutscene += StartCutscene;
            DialogueHandleUpdate.Instance.UpdateJson += LoadJson;
            DialogueHandleUpdate.Instance.UnloadJson += UnloadJson;
            DialogueUIHandler.Instance.GETNodes += SendNodes;
        }

        private void OnDisable()
        {
            if (InteractionHandler.Instance != null)
            {
                InteractionHandler.Instance.Interact -= StartShowDialogue;
                InteractionHandler.Instance.UpdateNode -= ChoiceUpdateNode;
                InteractionHandler.Instance.StartCutscene -= StartCutscene;
                DialogueHandleUpdate.Instance.UpdateJson -= LoadJson;
                DialogueHandleUpdate.Instance.UnloadJson -= UnloadJson;
                DialogueUIHandler.Instance.GETNodes -= SendNodes;
                
            }
        }

        private List<Node> SendNodes()
        {
            return Nodes;
        }

        private void StartShowDialogue()
        {
            if (Nodes != null)
            {
                StartCoroutine(ShowDialogue());
            }
        }
        
        private void StartCutscene(TextAsset json)
        {
            PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Disable();
            _isCutscene = true;
            LoadJson(json);
            StartCoroutine(ShowDialogue());
        }


        private void ChoiceUpdateNode(Node newNode)
        {
            _currentNode = Nodes.Find(x => x.node_name == newNode.node_name);
            StartCoroutine(ShowDialogue());
        }

        private IEnumerator ShowDialogue()
        {
            _currentNode ??= Nodes.Find(x => x.node_name == "START");
            if (_currentNode!=null)
            {
                while (_currentNode?.NodeType != NodeTypes.ShowMessage)
                {
                    if (_endNodeRan) yield break;
                    if (_currentNode==null) yield break;
                    switch (_currentNode?.NodeType)
                    {
                        case NodeTypes.Execute:
                            if (_lastNode.Contains(_currentNode))
                            {
                                _currentNode = _lastNode.Find(x => x.node_name == _currentNode.node_name);
                                yield return StartCoroutine(HandleEndNodeType());
                                break;
                            }
                            yield return StartCoroutine(HandleNodeExecute());
                            if (_currentNode!=null) continue;
                            yield break;
                        case NodeTypes.ConditionBranch:
                            yield return StartCoroutine(HandleNodeConditions());
                            if (_currentNode!=null) continue;
                            yield break;
                        case NodeTypes.SetLocalVariable:
                            yield return StartCoroutine(SetVariable());
                            if (_currentNode!=null) continue;
                            yield break;
                        case NodeTypes.Wait:
                            yield return StartCoroutine(Wait(_currentNode));
                            if (_currentNode!=null) continue;
                            yield break;
                        case NodeTypes.Start:
                            yield return StartCoroutine(GetNextNode());
                            break;
                        case NodeTypes.ChanceBranch:
                            yield return StartCoroutine(HandleChance());
                            break;
                        case NodeTypes.RandomBranch:
                            yield return StartCoroutine(HandleRandom());
                            break;
                        case NodeTypes.Repeat:
                            yield return StartCoroutine(HandleRepeat());
                            break;
                        default:
                            yield return StartCoroutine(GetNextNode());
                            break;
                    }
                }
                yield return StartCoroutine(GetNextNode());
                yield break;
            }
        }
        
        

        
        private IEnumerator HandleRepeat()
        {
            if (_currentNode.value == 0)
            {
                _currentNode = Nodes.Find(x => x.node_name == _currentNode?.next_done);
                yield break;
            }

            int nodeValue = _currentNode.value;
            nodeValue--;
            _currentNode.value = nodeValue;
            _currentNode = Nodes.Find(x => x.node_name == _currentNode?.next);
        }

        
        private IEnumerator HandleRandom()
        {
            var rnd = new Random();
            var result = rnd.Next(1,_currentNode.possibilities+1);
            var branch = _currentNode?.branches[result.ToString()];
            _currentNode = Nodes.Find(x => x.node_name == branch);
            yield break;
        }

        
        private IEnumerator HandleChance()
        {
            var chance = 100 - _currentNode?.chance_1;
            var rnd = new Random();
            var result = rnd.Next(100+1);
            _currentNode = result<=chance 
                ? Nodes.Find(x => x.node_name == _currentNode?.branches["1"]) 
                : Nodes.Find(x => x.node_name == _currentNode?.branches["2"]);
            yield break;
        }

        private IEnumerator GetNextNode()
        {
            bool currentNodeIsLast = _lastNode.Contains(_currentNode);
            if (_endNodeRan)
            {
                yield return CloseDialogue();
                _endNodeRan = false;
                _currentNode = null;
                if (!GameObject.Find("Player").GetComponent<Animator>().enabled)
                {
                    GameObject.Find("Player").GetComponent<Animator>().enabled = true;
                }
                yield break;
            }
            if (currentNodeIsLast)
            {
                StartCoroutine(HandleEndNodeType());
            }
            if (_currentNode?.NodeType == NodeTypes.ShowMessage && !currentNodeIsLast)
            {
                yield return DialogueUIHandler.Instance.OnShowDialogue(_currentNode);
            }
            if (!currentNodeIsLast ||  _currentNode.branches!=null || _currentNode.choices != null)
            {
                _currentNode = Nodes?.Find(x => x.node_name == _currentNode?.next);
                yield break;
            }
        }
        
        private IEnumerator HandleEndNodeType()
        {
            _endNodeRan = true;
            switch (_currentNode.NodeType)
            {
                case NodeTypes.ShowMessage:
                    yield return DialogueUIHandler.Instance.OnShowDialogue(_currentNode);
                    break;
                case NodeTypes.Execute:
                    yield return StartCoroutine(HandleNodeExecute());
                    break;
                case NodeTypes.ConditionBranch:
                    yield return StartCoroutine(HandleNodeConditions());
                    break;
                case NodeTypes.Start:
                    StartCoroutine(GetNextNode());
                    break;
                case NodeTypes.SetLocalVariable:
                    yield return StartCoroutine(SetVariable());
                    break;
                case NodeTypes.Wait:
                    yield return StartCoroutine(Wait(_currentNode));
                    break;
                case NodeTypes.ChanceBranch:
                    StartCoroutine(HandleChance());
                    break;
                case NodeTypes.RandomBranch:
                    StartCoroutine(HandleRandom());
                    break;
                case NodeTypes.Repeat:
                    StartCoroutine(HandleRepeat());
                    break;
            }
        }

        private IEnumerator CloseDialogue()
        {
            StopAllCoroutines();
            _currentNode = null;
            DialogueUIHandler.Instance.OnExitDialogue();
            if (_isCutscene)
            {
                foreach (var canvas in Resources.FindObjectsOfTypeAll<Canvas>())
                {
                    if (canvas.name == "TutorialCanvas")
                    {
                        canvas.gameObject.SetActive(true);
                        InteractionHandler.Instance.OnEndCutscene();
                    }
                }

                PlayerActionControlsManager.Instance.PlayerControls.Land.Movement.Enable();
            }

            if (!PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.enabled)
            {
                PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Enable();
            }
            _endNodeRan = false;
            yield break;
        }

        private IEnumerator Wait(Node currentNode)
        {
            PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Disable();
            yield return new WaitForSeconds(currentNode.time);
            StartCoroutine(GetNextNode());
            PlayerActionControlsManager.Instance.PlayerControls.Land.Interact.Enable();
        }

        private IEnumerator SetVariable()
        {
            yield return _currentVariable.variables[_currentNode.var_name].VariableData = _currentNode.value;
            StartCoroutine(GetNextNode());
        }

        private IEnumerator HandleNodeExecute()
        {
            var methodName = _currentNode.Text
                .Substring(0,_currentNode.Text.IndexOf("(", StringComparison.Ordinal)).Trim('"',' ',':');
            var parametersString = _currentNode.Text
                .Substring(_currentNode.Text.IndexOf("(", StringComparison.Ordinal));
            var parameters = new List<object>();
            for (int i = 0; i < parametersString.Length; ++i)
            {
                if (parametersString[i] == ')')
                {
                    break;
                }

                if (parametersString[i] == '(' && parametersString[i + 1] == ')')
                {
                    break;
                }
                if (parametersString[i] == '(' && !parametersString.Substring(i).Contains(','))
                {
                    parameters.Add(parametersString.Substring(i, parametersString.IndexOf(')')).Trim('(', ')'));
                    break;
                }
                if (parametersString[i] == '(' && parametersString.Substring(i).Contains(','))
                {
                    parameters.Add(parametersString.Substring(i, parametersString.IndexOf(',') - 1).Trim('('));
                    i = parametersString.IndexOf(',')-1;
                    continue;
                }
                if (parametersString[i] == ',' && parametersString.Substring(i + 1).Contains(','))
                {
                    parameters.Add(parametersString.Substring(i + 1, parametersString.Substring(i + 1).IndexOf(',')).Trim(' '));
                    continue;
                }
                if (parametersString[i] == ',' && !parametersString.Substring(i + 1).Contains(','))
                {
                    parameters.Add(parametersString.Substring(i + 1, parametersString.Substring(i + 1).IndexOf(')')).Trim(' '));
                    continue;
                }
            }

            for (var i = 0; i < parameters.Count; ++i)
            {
                var isBool = bool.TryParse(parameters[i] as string, out var boolResult);
                var isInt = int.TryParse(parameters[i] as string, out var intResult);
                if (isBool)
                {
                    parameters[i] = boolResult;
                    continue;
                }

                if (isInt)
                {
                    parameters[i] = intResult;
                    continue;
                }

                if (parameters[i] is string)
                {
                    parameters[i] = parameters[i].ToString().Trim('\\','\"');
                }
            }
            ExecutedMethod(methodName, parameters.ToArray());

            if(!_lastNode.Contains(_currentNode)) {
                var afterExecution = Connections.ToList().Find(x=> x.@from == _currentNode.node_name);
                if (afterExecution.to != null)
                {
                    _currentNode = Nodes.Find(x => x.node_name == afterExecution.to);
                    yield break;
                }
            }

            if (_lastNode.Exists(x => x.node_name == _currentNode.node_name))
            {
                _endNodeRan = true;
            }
            StartCoroutine(GetNextNode());
            yield break;
        }

        private void ExecutedMethod(string method, object[] parameters)
        {
            var type = GetType();
            var theMethod = type.GetMethod(method);
            if (theMethod == null)
            {
                Debug.LogWarning("Method specified in dialogue designer not available");
                return;
            }
            
            theMethod?.Invoke(this, parameters);
        }

        public void Knock()
        {
            GameObject.Find("knock").GetComponent<AudioSource>().Play();
        }

        public void RemoveVillian()
        {
            Destroy(GameObject.Find("Villain"));
        }

        public void ChangeScene(int levelIndex)
        {
            StopAllCoroutines();
            InteractionHandler.Instance.OnLevelAnimInt(levelIndex);
        }

        public void StartCutscene(string cutScene)
        {
            StopAllCoroutines();
            Debug.Log("StartCutscene ran");
            CutsceneHandler.Instance.OnStartCutsceneWithNoDialogue(cutScene);
        }

        public void StartCutsceneWithDialogue(string scene, string json)
        {
            StopAllCoroutines();
            CutsceneHandler.Instance.OnStartCutsceneWithDialogueName(json, scene);
        }

        public void SavingThePrincess()
        {
            StopAllCoroutines();
            InteractionHandler.Instance.OnLevelAnimName("CombatScene");
        }

        public void FightRedKnight()
        {
            StopAllCoroutines();
            InteractionHandler.Instance.OnLevelAnimName("CombatScene");
        }

        public void DisableTrigger()
        {
            Destroy(GameObject.Find("CutsceneTrigger"));
        }

        public void CatDogFight()
        {
            StopAllCoroutines();
            InteractionHandler.Instance.OnLevelAnimName("CombatScene");
        }

        public void ExecuteFinalBattle()
        {
            StopAllCoroutines();
            InteractionHandler.Instance.OnLevelAnimName("CombatScene");
        }
        
        public void Seinfeld()
        {
            GameObject.Find("Seinfeld").GetComponent<AudioSource>().Play();
            GameObject.Find("Player").GetComponent<Animator>().enabled = true;
            StartCutscene("imouttahere");
        }

        public void Pew()
        {
            StartCoroutine(laserBeam());
        }

        public void DyingSoldier()
        {
            //GameObject.FindGameObjectWithTag("Deadsoldier").GetComponent<PolygonCollider2D>().enabled = false;
        }

        public IEnumerator laserBeam()
        {
            var sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            sound.PlayPew();
            sound.gameObject.GetComponent<AudioSource>().loop = false;
            yield return new WaitForSeconds(1f);
            playExplosion();
        }

        public void playExplosion()
        {
            var sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            sound.PlayExplosion();
        }

        private IEnumerator HandleNodeConditions()
        {
            var variable = _currentNode.Text.Substring(
                _currentNode.Text.IndexOf("\\", StringComparison.Ordinal),
                _currentNode.Text.LastIndexOf("\\", StringComparison.Ordinal) 
                           - _currentNode.Text.IndexOf("\\", StringComparison.Ordinal)).Trim('\"', '\\');
            var variableData = _currentVariable.variables[variable].VariableData;
            var nextBranch = _currentNode.branches[variableData.ToString()];
            _currentNode = _currentNode = Nodes.Find(x => x.node_name == nextBranch);
            Debug.Log(_currentNode.nodeType);
            yield break;
        }


        private void LoadJson(TextAsset json)
        {
            _json = json;
            var df = new DialogueFetcher(_json);
            Dialogue = df.Dialogue;
            _currentNode = Nodes.Find(x => x.node_name == "START");
            if (!_globalVariables.ContainsKey(json.name))
            {
                _globalVariables.Add(json.name, Dialogue.Variables);
            }
            _currentVariable = _globalVariables[json.name];
            SetLastConnectionNode();
        }

        private void SetLastConnectionNode()
        {
            _endNodeRan = false;
            _lastNode = new List<Node>();
            foreach (var connection in Connections)
            {
                var exists = Connections.ToList().Exists(x => x.@from == connection.to);
                if (!exists)
                {
                    _lastNode.Add(Nodes.Find(y=> y.node_name == connection.to));
                }
            }
        }

        private void UnloadJson()
        {
            _json = null;
            Dialogue = null;
            _currentNode = null;
            _currentVariable = null;
        }
    }
}