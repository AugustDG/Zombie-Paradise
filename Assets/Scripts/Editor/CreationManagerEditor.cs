using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(CreationManager))]
    public class CreationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();

            var baseScript = (CreationManager)target;
            
            GUILayout.Space(10f);
            
            if (GUILayout.Button("Create Zombie"))
            {
                baseScript.DisplayZombie();
            }
            
            if (GUILayout.Button("Delete Zombie"))
            {
                baseScript.DeleteZombie();
            }
        }
    }