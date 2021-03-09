using System.Collections.Generic;
using Characters;
using ScriptableObjects;
using Unity.Collections;
using UnityEngine;

namespace Utility
{
    public static class MapData
    {
        public static Node[,] Map;
        public static NativeArray<JobNode> NativeMap;
        public static Vector2Int MapSize = new Vector2Int(250, 250);

        public static List<CharacterBehaviour> CharacterList;
        public static SyncList<CharacterBehaviour, HumanBehaviour> HumanList;
        public static SyncList<CharacterBehaviour, ZombieBehaviour> ZombieList;

        public static ZombieData ZombieToSpawn;
        
        public static float FogHiDensity = 0.1f;
        public static float FogLoDensity = 0.01f;

        static MapData()
        {
            CharacterList = new List<CharacterBehaviour>();
            HumanList = new SyncList<CharacterBehaviour, HumanBehaviour>(CharacterList);
            ZombieList = new SyncList<CharacterBehaviour, ZombieBehaviour>(CharacterList);
        }

        public static void ClearLists()
        {
            CharacterList.Clear();
            HumanList.Clear();
            ZombieList.Clear();
        }
    }
}