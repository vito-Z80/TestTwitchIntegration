using System;

namespace Data
{
    [Flags]
    public enum AvatarState
    {
        Idle = 1,
        Left = 2,
        Right = 4,
        Attack = 8,
        AttackLeft = 16,
        AttackRight = 32
    }
}