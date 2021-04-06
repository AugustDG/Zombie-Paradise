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

        public static bool CanSpawnZombies = true;

        public static Node[,] Map;
        public static Vector2Int MapSize = new Vector2Int(250, 250);

        public static List<CharacterBehaviour> PathfindingList;
        public static List<HumanBehaviour> HumanList;
        public static List<ZombieBehaviour> ZombieList;

        public static ZombieData ZombieToSpawn;
        public static CreationManager CreationManagerRef;

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
            PathfindingList = new List<CharacterBehaviour>();
            HumanList = new List<HumanBehaviour>();
            ZombieList = new List<ZombieBehaviour>();
        }

        public static void ClearLists()
        {
            PathfindingList.Clear();
            HumanList.Clear();
            ZombieList.Clear();
        }
    }
}