using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResearchManager))]
public class ResearchManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var baseScript = (ResearchManager)target;
        
        GUILayout.Space(5f);
        if (GUILayout.Button("Fill Dictionary")) baseScript.FillDictionary();
        GUILayout.Space(8f);
        
        DrawDefaultInspector();
    }
}
