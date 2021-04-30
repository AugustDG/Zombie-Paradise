using System;
using Utility.Events;

namespace Utility
{
    public static class MapEvents
    {
        public static EventHandler<CurrencyType> CurrencyChangedEvent;
        public static EventHandler HumanKilledEvent;
        public static EventHandler TreeLifeLostEvent;
        
        public static EventHandler<AudioEventArgs> StepTakenEvent;

        public static EventHandler<bool> ChangedToGraveyardEvent;
        public static EventHandler<bool> ChangedToTopViewEvent;
    }
}