using System;

[Flags]
public enum MovementStatus
{
    None = 0,
    Moving = 1 << 0,
    Rotating = 1 << 1,
    All = ~0
}
