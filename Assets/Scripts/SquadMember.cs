using UnityEngine;

public class SquadMember : Character
{
    [SerializeField] private Squad squadUnit;
    public Squad Squad => squadUnit;
    public override EntityType EntityType => EntityType.SquadMember;
}