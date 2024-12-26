using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSystems.Dialogue.Dialogue_Json_Classes;
using UnityEngine;
using Utilities;
using DialogueClass = GameSystems.Dialogue.Dialogue_Json_Classes.Dialogue;

namespace GameSystems.Dialogue
{
    
    public class DialogueFetcher
    {
        private TextAsset _json;

        public DialogueClass Dialogue { get; private set; }
        

        public DialogueFetcher(TextAsset json)
        {
            if (json == null)
            {
                Debug.LogError("The file your were trying to load was empty");
                return;
            }
            _json = json;
            DialogueSetup();
        }
        

        private async void DialogueSetup()
        {
            var dialogueArray = JsonHelper.GETJsonArray<Dialogue_Json_Classes.Dialogue>(_json.text);
            Dialogue = dialogueArray[0];
            var filePath = CustomUtils.GETDialogueFilePathFromAsset(_json);
            await GETInconsistentlyDataAsync(filePath);
        }


        private async Task GETInconsistentlyDataAsync(string filePath)
        {
            var tasks = new List<Task>
            {
                Task.Run(() => GETVariablesAsync(filePath)),
                Task.Run(() => GETBranchesAsync(filePath)),
                Task.Run(() => GETCharacterNameAsync(filePath)),
                Task.Run(() => GETTextAsync(filePath)),
                Task.Run(() => GETValueAsync(filePath))
            };

            await Task.WhenAll(tasks);
        }

        private void GETValueAsync(string filePath)
        {
            if (Dialogue == null)
            {
                return;
            }

            var file = new StreamReader(filePath);
            string readLine;
            var insideNodes = false;
            var nodeNumber = 0;
            var insideArray = 0;
            var insideNodeObject = 0;
            var lineNum = 0;
            while ((readLine = file.ReadLine()) != null)
            {
                lineNum++;
                if (readLine.Contains("nodes") && readLine.Contains("["))
                {
                    insideNodes = true;
                    insideArray++;
                }

                if (insideNodes)
                {
                    if (readLine.Contains("[") && !readLine.Contains("]")) insideArray++;
                    if (readLine.Contains("]") && !readLine.Contains("[")) insideArray--;
                    if (insideArray == 1 && readLine.Contains("],"))
                    {
                        insideNodes = false;
                    }
                    if (readLine.Contains("{") && !readLine.Contains("}"))
                    {
                        insideNodeObject++;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{"))
                    {
                        insideNodeObject--;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{") &&
                        insideNodeObject == 0)
                    {
                        nodeNumber++;
                    }
                }
                
                if (readLine.Contains("value") && insideNodeObject == 0)
                {
                    continue;
                }
                
                if (readLine.Contains("value") &&  !readLine.Contains("{"))
                {
                    var trimmed = readLine.Trim(' ', '"');
                    var value = new StringBuilder();
                    if (trimmed.IndexOf(",", StringComparison.Ordinal) == -1)
                    {
                        value.Append(trimmed.Substring(
                            startIndex: trimmed.IndexOf(":", StringComparison.Ordinal) + 1,
                            length: trimmed.Length - 1 -
                                    (trimmed.IndexOf(":", StringComparison.Ordinal))
                        ));
                    }
                    else
                    {
                        value.Append(trimmed.Substring(
                            startIndex: trimmed.IndexOf(":", StringComparison.Ordinal) + 1,
                            length: trimmed.IndexOf(",", StringComparison.Ordinal) - 1 -
                                    (trimmed.IndexOf(":", StringComparison.Ordinal))
                        ));
                    }
                    var boolCouldPass = bool.TryParse(value.ToString(), out var isBool);
                    var intCouldPass = int.TryParse(value.ToString(), out var isInt);
                    if (boolCouldPass)
                    {
                        Dialogue.nodes[nodeNumber].value = isBool;
                    }
                    if (intCouldPass)
                    {
                        Dialogue.nodes[nodeNumber].value= isInt;
                    }

                    if (!boolCouldPass && !intCouldPass)
                    {
                        Dialogue.nodes[nodeNumber].value= value;
                    }
                }
            }
            file.Close();
        }

        private void GETTextAsync(string filePath)
        {
            if (Dialogue == null)
            {
                return;
            }

            var file = new StreamReader(filePath);
            string readLine;
            var insideNodes = false;
            var nodeNumber = 0;
            var insideArray = 0;
            var insideNodeObject = 0;
            var lineNum = 0;
            while ((readLine = file.ReadLine()) != null)
            {
                lineNum++;
                if (readLine.Contains("nodes") && readLine.Contains("["))
                {
                    insideNodes = true;
                    insideArray++;
                }

                if (insideNodes)
                {
                    if (readLine.Contains("[") && !readLine.Contains("]")) insideArray++;
                    if (readLine.Contains("]") && !readLine.Contains("[")) insideArray--;
                    if (insideArray == 1 && readLine.Contains("],"))
                    {
                        insideNodes = false;
                    }
                    if (readLine.Contains("{") && !readLine.Contains("}"))
                    {
                        insideNodeObject++;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{"))
                    {
                        insideNodeObject--;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{") &&
                        insideNodeObject == 0)
                    {
                        nodeNumber++;
                    }
                }
                
                if (readLine.Contains("text") && readLine.Contains("{") && insideNodeObject == 0)
                {
                    continue;
                }
                
                if (readLine.Contains("text") && !readLine.Contains("FR") && !readLine.Contains("{"))
                {
                    var trimmed = readLine.Trim(' ', '"');
                    var text = trimmed.Substring(
                        trimmed.IndexOf(":", StringComparison.Ordinal),
                        trimmed.LastIndexOf("\"", StringComparison.Ordinal)
                        - trimmed.IndexOf("\"", StringComparison.Ordinal) 
                    );
                    Dialogue.nodes[nodeNumber].text.ENG = text.Trim('\"');
                }
            }
            file.Close();
        }

        private void GETVariablesAsync(string filePath)
        {
            if (Dialogue == null) return;
            string readLine;
            var insideVariables = false;
            Dialogue.Variables.variables = new Dictionary<string, Variable>();
            var file = new StreamReader(filePath);
            var insideCount = 0;
            var numberOfObjects = 0;
            var keys = new List<string>();
            var types = new List<int>();
            while ((readLine = file.ReadLine()) != null)
            {
                if (readLine.Contains("variables")) insideVariables = true;

                if (insideVariables)
                {
                    if (readLine.Contains("{") && readLine.Contains("}"))
                    {
                        var variableKey = readLine.Substring(0, readLine.IndexOf(
                            ":", StringComparison.Ordinal)).Trim('\"', ' ');
                        var insideObject = readLine.Substring(
                            readLine.IndexOf("{", StringComparison.Ordinal),
                            readLine.IndexOf("}", StringComparison.Ordinal) + 1
                            - readLine.IndexOf("{", StringComparison.Ordinal));
                        var indexFrom = new List<int>();
                        var indexTo = new List<int>();
                        var index = 0;
                        while ((index = insideObject.IndexOf(":", index, StringComparison.Ordinal)) != -1)
                        {
                            indexFrom.Add(index);
                            index++;
                        }

                        index = 0;
                        while ((index = insideObject.IndexOf(",", index, StringComparison.Ordinal)) != -1)
                        {
                            indexTo.Add(index);
                            index++;
                        }

                        if (indexFrom.Count > indexTo.Count)
                        {
                            var indexToAdd = insideObject.IndexOf("}", StringComparison.Ordinal);
                            indexTo.Add(indexToAdd);
                        }

                        if (indexFrom.Count == indexTo.Count)
                        {
                            int.TryParse(insideObject
                                .Substring(indexFrom[0], indexTo[0] - indexFrom[0]).Trim(' ', ':'), out var type);
                            switch (type)
                            {
                                case 0:
                                    Dialogue.Variables.variables
                                        .Add(variableKey, new Variable(type,
                                            insideObject
                                                .Substring(indexFrom[1], indexTo[1] - indexFrom[1])
                                                .Trim(' ', '\"', ',', '}')));
                                    break;
                                case 1:
                                    int.TryParse(insideObject
                                        .Substring(indexFrom[1], indexTo[1] - indexFrom[1])
                                        .Trim(' ', '\"', ',', '}'), out var valueInt);
                                    Dialogue.Variables.variables
                                        .Add(variableKey, new Variable(type, valueInt));
                                    break;
                                case 2:
                                    bool.TryParse(insideObject
                                        .Substring(indexFrom[1], indexTo[1] - indexFrom[1])
                                        .Trim(' ', '\"', ',', '}'), out var valueBool);
                                    Dialogue.Variables.variables
                                        .Add(variableKey, new Variable(type, valueBool));
                                    break;
                            }
                        }

                        continue;
                    }

                    if (readLine.Contains("{") && !readLine.Contains("}")) insideCount++;
                    if (readLine.Contains("}") && !readLine.Contains("{"))
                    {
                        insideCount--;
                        numberOfObjects++;
                    }

                    if (insideCount == 0)
                    {
                        insideVariables = false;
                    }


                    if (insideVariables && readLine.Contains(":") && insideCount > 0 && !readLine.Contains("variables"))
                    {
                        if (readLine.Contains("{"))
                        {
                            keys.Add(readLine.Substring(0, readLine.IndexOf(
                                ":", StringComparison.Ordinal)).Trim('\"', ' '));
                        }

                        if (readLine.Contains("type") && readLine.Contains(","))
                        {
                            var stringToInt = readLine
                                .Substring(readLine.IndexOf(":", StringComparison.Ordinal),
                                    readLine.IndexOf(",", StringComparison.Ordinal)
                                    - readLine.IndexOf(":", StringComparison.Ordinal) + 1)
                                .Trim(' ', ',', ':');
                            int.TryParse(stringToInt, out int type);
                            types.Add(type);
                        }

                        if (readLine.Contains("value"))
                        {
                            switch (types[numberOfObjects])
                            {
                                case 0:
                                    Dialogue.Variables.variables
                                        .Add(keys[numberOfObjects], new Variable(types[numberOfObjects],
                                            readLine
                                                .Substring(readLine.IndexOf(":", StringComparison.Ordinal),
                                                    readLine.Length - readLine
                                                        .IndexOf(":", StringComparison.Ordinal))
                                                .Trim(' ', '\"', ',', '}')));
                                    break;
                                case 1:
                                    int.TryParse(readLine
                                        .Substring(readLine.IndexOf(":", StringComparison.Ordinal),
                                            readLine.Length - readLine
                                                .IndexOf(":", StringComparison.Ordinal))
                                        .Trim(' ', '\"', ',', '}'), out var valueInt);
                                    Dialogue.Variables.variables
                                        .Add(keys[numberOfObjects], new Variable(types[numberOfObjects], valueInt));
                                    break;
                                case 2:
                                    var stringToBool = readLine
                                        .Substring(readLine.IndexOf(":", StringComparison.Ordinal),
                                            readLine.Length - readLine
                                                .IndexOf(":", StringComparison.Ordinal))
                                        .Trim(' ', '\"', ',', '}', ':');
                                    bool.TryParse(stringToBool, out var valueBool);
                                    Dialogue.Variables.variables
                                        .Add(keys[numberOfObjects], new Variable(types[numberOfObjects], valueBool));
                                    break;
                            }
                        }
                    }
                }
            }

            file.Close();
        }

        private void GETBranchesAsync(string filePath)
        {
            #region SeachFileForBranches

            if (Dialogue == null)
            {
                return;
            }

            var file = new StreamReader(filePath);
            string readLine;
            var insideNodes = false;
            var insideBranches = false;
            var nodeNumber = 0;
            var insideArray = 0;
            var insideNodeObject = 0;
            var lineNum = 0;
            var dictionary = new Dictionary<string, string>();
            while ((readLine = file.ReadLine()) != null)
            {
                lineNum++;
                if (readLine.Contains("nodes") && readLine.Contains("["))
                {
                    insideNodes = true;
                    insideArray++;
                }

                if (insideNodes)
                {
                    if (readLine.Contains("[") && !readLine.Contains("]")) insideArray++;
                    if (readLine.Contains("]") && !readLine.Contains("[")) insideArray--;
                    if (insideArray == 1 && readLine.Contains("],"))
                    {
                        insideNodes = false;
                    }
                    if (readLine.Contains("{") && !readLine.Contains("}"))
                    {
                        insideNodeObject++;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{"))
                    {
                        insideNodeObject--;
                    }

                    if (readLine.Contains("}") && !readLine.Contains("{") &&
                        insideNodeObject == 0)
                    {
                        nodeNumber++;
                    }

                    if (insideNodeObject == 1 && insideBranches && readLine.Contains("},"))
                    {
                        insideBranches = false;
                    }
                    
                    if (insideBranches)
                    {
                        var key = readLine
                            .Substring(0, readLine.IndexOf(":", StringComparison.Ordinal)).Trim(' ', '\"');
                        if(readLine.Contains("null"))
                        {
                            continue;
                        }
                        var value = readLine
                            .Substring(
                                readLine.IndexOf(":", StringComparison.Ordinal), 
                                readLine.LastIndexOf("\"", StringComparison.Ordinal)
                                - readLine.IndexOf(":", StringComparison.Ordinal)).Trim(' ', '\"', ':');
                        dictionary.Add(key, value);
                        Dialogue.nodes[nodeNumber].branches = dictionary;
                    }
                    
                    if (readLine.Contains("\"branches\":"))
                    {
                        insideBranches = true;
                    }
                }
            }

            file.Close();

            #endregion
        }

        private void GETCharacterNameAsync(string filePath)
        {
            #region SeachFileForCharacters

            if (Dialogue == null)
            {
                return;
            }

            string readLine;
            var insideArray = false;
            var insideNode = false;
            var file = new StreamReader(filePath);
            var characterArrayList = new ArrayList();
            while ((readLine = file.ReadLine()) != null)
            {
                if (readLine.Contains("nodes")) insideNode = true;
                if (insideNode)
                {
                    if (insideArray)
                    {
                        if (readLine.Contains("]"))
                        {
                            insideArray = false;
                            continue;
                        }

                        characterArrayList.Add(readLine.Trim(' ', '\"', ','));
                    }

                    if (readLine.Contains("character") && readLine.Contains("[") &&
                        !readLine.Contains("]"))
                    {
                        insideArray = true;
                    }

                    if (readLine.Contains("[") && readLine.Contains("]") &&
                        readLine.Contains("character"))
                    {
                        var substringList = readLine.Substring(
                                readLine.IndexOf("[", StringComparison.Ordinal),
                                ((readLine.IndexOf("]", StringComparison.Ordinal) + 1)
                                 - readLine.IndexOf("[", StringComparison.Ordinal)))
                            .Trim(' ', '\"', '[', ']').Split(',').ToList();
                        characterArrayList.AddRange(substringList);
                    }
                }
            }

            file.Close();

            #endregion

            #region InsertCharactersToNodes

            var charterName = new Queue<string>();
            var charterIndex = new Queue<int>();
            for (int i = 0; i < characterArrayList.Count; ++i)
            {
                if (i % 2 == 0)
                {
                    try
                    {
                        charterName.Enqueue(characterArrayList[i].ToString().Trim('\"'));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        int.TryParse(characterArrayList[i].ToString(), out var index);
                        charterIndex.Enqueue(index);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            foreach (var node in Dialogue.nodes.Where(node => node.text.ENG != null && node.NodeType == NodeTypes.ShowMessage))
            {
                node.character = charterName.Dequeue();
                node.characterIndex = charterIndex.Dequeue();
            }

            #endregion
        }

        
    }
}