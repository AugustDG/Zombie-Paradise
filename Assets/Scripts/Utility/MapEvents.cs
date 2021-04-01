using System;
using UnityEngine;
using Utility.Events;

namespace Utility
{
    public static class MapEvents
    {
        public static EventHandler CurrencyChangedEvent;
        public static EventHandler HumanKilledEvent;
        
        public static EventHandler<AudioEventArgs> StepTakenEvent;
        public static EventHandler FingerAddedEvent;
        public static EventHandler BrainAddedEvent;
        
        public static EventHandler<bool> ChangedToGraveyardEvent;
        public static EventHandler<bool> ChangedToTopViewEvent;
    }
}