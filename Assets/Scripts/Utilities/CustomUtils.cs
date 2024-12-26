using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Formatting = System.Xml.Formatting;

namespace Utilities
{
    public static class CustomUtils
    {
        private static bool windows;
        public static T InstanceOfType<T>(List<Component> list)
        {
            var result =  list.Find(component => 
                component.GetType() == typeof(T));
            var resultOfType = (T) Convert.ChangeType(result, typeof(T));
            return resultOfType;
        } 
        
        public static T InstanceOfType<T>(List<Component> list, string search)
        {
            var result = list.Find(component => 
                component.GetType() == typeof(T) && component.name == search);
            var resultOfType = (T) Convert.ChangeType(result, typeof(T));
            return resultOfType;
        }


        public static void PrettifyJson(TextAsset json)
        {
            var filePath = GETDialogueFilePathFromAsset(json);
            var obj = JsonConvert.DeserializeObject(json.ToString());
            var content = JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting) Formatting.Indented);

            File.WriteAllText(filePath, content);

        }

        public static string GETDialogueFilePathFromAsset(TextAsset asset)
        {
            var sb = new StringBuilder();
            
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        windows = false;    
#else
            windows = true;
#endif
            var root = String.Empty;
            if (windows)
            {
               root = Application.streamingAssetsPath + "\\DialogueDesigner\\OutPut\\";
            }
            else
            {
                root = Application.streamingAssetsPath + "/DialogueDesigner/OutPut/";
            }
            var files = Directory.GetFiles(root, "*.json*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Contains("meta")) continue;
                var fileName = string.Empty;
                if (windows)
                {
                    fileName = file.
                        Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1,
                            file.IndexOf(".json", StringComparison.Ordinal) 
                            - file.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                }
                else
                {
                    fileName = file.
                        Substring(file.LastIndexOf("/", StringComparison.Ordinal) + 1,
                            file.IndexOf(".json", StringComparison.Ordinal) 
                            - file.LastIndexOf("/", StringComparison.Ordinal) - 1);
                }
                if (fileName.Equals(asset.name)) sb.Append(file);
            }
            return sb.ToString();
        }
    }
}