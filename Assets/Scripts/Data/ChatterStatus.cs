using System;

namespace Data
{
    [Flags]
    public enum ChatterStatus
    {
        //  0 - for all chatters
        Moderator = 1,
        Partner = 2,
        Premium = 4,
        Admin = 8,
        Stuff = 16,
        Vip = 32,
        Subscriber = 64,
    }
}