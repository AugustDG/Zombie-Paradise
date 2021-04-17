using System.Collections.Generic;
using Characters;
using ScriptableObjects;
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
        public static ResearchManager ResearchManagerRef;

        public static float CurrentMaxAttack = 5f;
        public static float CurrentMaxHealth = 30f;
        public static float CurrentMaxSpeed = 5f;
        public static float CurrentCostMult = 1f; //cost is dimished by this multiplier
        public static float CurrentTreeHealth = 500f;
        public static float CurrentTreeAttack = 10f;

        public static int FingerAmount
        {
            get => _fingerAmount;
            set
            {
                var type = CurrencyType.Fingers;

                if (value > _fingerAmount)
                    type = CurrencyType.FingersAdded;
                else if (value < _fingerAmount) 
                    type = CurrencyType.FingersAdded;
                
                _fingerAmount = value;
                MapEvents.CurrencyChangedEvent.Invoke(null, type);
            }
        }

        private static int _fingerAmount = 0;

        public static int BrainAmount
        {
            get => _brainAmount;
            set
            {
                var type = CurrencyType.Brains;

                if (value > _brainAmount)
                    type = CurrencyType.BrainsAdded;
                else if (value < _brainAmount) 
                    type = CurrencyType.BrainsAdded;

                _brainAmount = value;
                MapEvents.CurrencyChangedEvent.Invoke(null, type);
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