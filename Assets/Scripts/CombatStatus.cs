using System;

[Flags]
public enum CombatStatus
{
    None = 0,
    LookAtTarget = 1 << 0,
    Attacking = 1 << 1,
    GettingCloser = 1 << 2,
    All = ~0
}