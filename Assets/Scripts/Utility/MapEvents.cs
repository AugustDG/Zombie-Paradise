using System;

namespace Utility
{
    public static class MapEvents
    {
        public static EventHandler<int> IncreaseBrainEvent = IncreaseBrainHandler;
        public static EventHandler<int> IncreaseFingerEvent = IncreaseFingerHandler;
        public static EventHandler CurrencyChangedEvent;

        private static void IncreaseBrainHandler(object sender, int amount) => MapData.BrainAmount += amount;
        private static void IncreaseFingerHandler(object sender, int amount) => MapData.FingerAmount += amount;
    }
}