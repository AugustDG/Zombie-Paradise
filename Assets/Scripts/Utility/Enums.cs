namespace Utility
{
    public enum Side
    {
        Left,
        Right
    }

    public enum NodeTypes
    {
        Blocked,
        Free
    }

    public enum ResearchState
    {
        Researched,
        TooExpensive,
        Unlocked,
        Locked,
    }

    public enum ResearchType
    {
        ATTACK,
        HEALTH,
        SPEED,
        COST,
        ATTACK2,
        HEALTH2,
        SPEED2,
        COST2,
        ATTACK3,
        HEALTH3,
        SPEED3,
        COST3,
        THEALTH,
        THEALTH2,
        THEALTH3,
        TATTACK,
        TATTACK2,
        TATTACK3,
    }
    
    public enum PartType
    {
        Head,
        Torso,
        Arms,
        Leg
    }

    public enum ModifierType
    {
        Add,
        Mult
    }
    
    public enum Button3DState
    {
        None,
        Hovered,
        Pressed
    }
    
    public enum CurrencyType
    {
        Fingers,
        Brains,
        FingersAdded,
        BrainsAdded,
        FingersRemoved,
        BrainsRemoved
    }

    public enum GameEndType
    {
        Win,
        LossByTime,
        LossByTree
    }
}