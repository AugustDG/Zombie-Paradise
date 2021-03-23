using System;
using System.Collections.Generic;
using Characters;
using ScriptableObjects;
using Unity.Collections;
using UnityEngine;

namespace Utility
{
    public static class MapData
    {
        public static float FogHiDensity = 0.1f;
        public static float FogLoDensity = 0.01f;
        
        public static Node[,] Map;
        public static NativeArray<JobNode> NativeMap;
        public static Vector2Int MapSize = new Vector2Int(250, 250);

        public static List<CharacterBehaviour> CharacterList;
        public static SyncList<CharacterBehaviour, HumanBehaviour> HumanList;
        public static SyncList<CharacterBehaviour, ZombieBehaviour> ZombieList;

        public static ZombieData ZombieToSpawn;

        public static int FingerAmount
        {
            get => _fingerAmount;
            set
            {
                _fingerAmount = value;
                MapEvents.CurrencyChangedEvent.Invoke(null, EventArgs.Empty);
            }
        }

        private static int _fingerAmount = 0;
        
        public static int BrainAmount
        {
            get => _brainAmount;
            set
            {
                _brainAmount = value;
                MapEvents.CurrencyChangedEvent.Invoke(null, EventArgs.Empty);
            }
        }

        private static int _brainAmount = 0;

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