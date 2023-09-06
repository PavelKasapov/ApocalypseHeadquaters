using UnityEngine;

public interface IClickable
{
    EntityType EntityType { get; }
}

public enum EntityType
{
    SquadMember,
    Enemy,
    Ground,
}