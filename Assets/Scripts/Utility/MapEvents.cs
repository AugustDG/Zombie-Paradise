using System;

namespace Utility
{
    public static class MapEvents
    {
        public static EventHandler CurrencyChangedEvent;
        
        public static EventHandler FingerAddedEvent;
        public static EventHandler BrainAddedEvent;
        
        public static EventHandler<bool> ChangedToGraveyardEvent;
        public static EventHandler<bool> ChangedToTopViewEvent;
    }
}