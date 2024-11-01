using System;

namespace Data
{
    [Flags]
    public enum ChatterStatus
    {
        //  0 - for all chatters
        Subscriber = 1,
        Vip = 2,
        Moderator = 4
    }
}