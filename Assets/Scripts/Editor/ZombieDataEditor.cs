using ScriptableObjects;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ZombieData))]
    public class ZombieDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var baseScript = (ZombieData)target;
            
            GUILayout.Space(10f);
            
            if (GUILayout.Button("Update Modifier Values"))
            {
                baseScript.CalculateTotalModifiers();
            }
        }
    }