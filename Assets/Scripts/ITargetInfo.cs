using UnityEngine;

public interface ITargetInfo
{
    Transform Transform { get; }
    EntityType EntityType { get; }
}
