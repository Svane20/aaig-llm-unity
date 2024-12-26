using Dialogue.Objects;
using GameSystems.Timeline;
using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(CutsceneController))]
    public class CutsceneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (CutsceneController) target;
        
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space((Screen.width / 100) * 20 );
            if (GUILayout.Button(
                "Update Json",
                GUILayout.Width((Screen.width / 100) * 30)))
            {
                script.UpdateJson();
            }
        
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        

        }
    
    }

