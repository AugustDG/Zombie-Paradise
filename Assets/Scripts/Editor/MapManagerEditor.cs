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
        GUILayout.Space(5f);

        if (GUILayout.Button("Generate Map"))
        {
            if (MapData.NativeMap.IsCreated) MapData.NativeMap.Dispose();
                baseScript.CreateMap();
        }
        
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Tiles")) baseScript.SpawnTiles();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Tiles")) baseScript.DeleteTiles();
        
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Border Trees")) baseScript.SpawnBorderTrees();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Border Trees")) baseScript.DeleteBorderTrees();
        
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Map Trees")) baseScript.SpawnMapTrees();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Map Trees")) baseScript.DeleteMapTrees();
        
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Props")) baseScript.SpawnProps();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Props")) baseScript.DeleteProps();
        
        GUILayout.Space(8f);
        if (GUILayout.Button("Generate Humans")) baseScript.SpawnHumans();
        
        GUILayout.Space(3f);
        if (GUILayout.Button("Delete Humans")) baseScript.DeleteHumans();
    }
}
