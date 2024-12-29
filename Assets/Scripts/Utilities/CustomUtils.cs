using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Formatting = System.Xml.Formatting;

namespace Utilities
{
    public static class CustomUtils
    {
        public static T InstanceOfType<T>(List<Component> list)
        {
            var result = list.Find(component =>
                component.GetType() == typeof(T));
            var resultOfType = (T)Convert.ChangeType(result, typeof(T));
            return resultOfType;
        }

        public static T InstanceOfType<T>(List<Component> list, string search)
        {
            var result = list.Find(component =>
                component.GetType() == typeof(T) && component.name == search);
            var resultOfType = (T)Convert.ChangeType(result, typeof(T));
            return resultOfType;
        }


        public static void PrettifyJson(TextAsset json)
        {
            var filePath = GetDialogueFilePathFromAsset(json);
            var obj = JsonConvert.DeserializeObject(json.ToString());
            var content = JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting)Formatting.Indented);

            File.WriteAllText(filePath, content);
        }

        public static string GetDialogueFilePathFromAsset(TextAsset asset)
        {
            var root = Path.Combine(Application.dataPath, "Resources", "DialogueDesigner", "Output");
            var files = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith(".meta"));

            return files.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file).Equals(asset.name));
        }
    }
}