using UnityEditor;
using UnityEngine;
using Utility;
[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var baseScript = (MapManager)target;
        
        GUILayout.Space(10f);
        GUILayout.Label("Generators", EditorStyles.boldLabel);
        GUILayout.Space(10f);

        if (GUILayout.Button("Generate Map"))
        {
            if (MapData.NativeMap.IsCreated) MapData.NativeMap.Dispose();
                baseScript.CreateMap();
        }
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Trees")) baseScript.SpawnTrees();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Trees")) baseScript.DeleteTrees();
    }
}
