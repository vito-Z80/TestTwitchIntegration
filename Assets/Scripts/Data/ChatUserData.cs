using UnityEngine;

namespace Data
{
    public class ChatUserData
    {
        public string UserName;
        public string CustomRewardId;
        public Color Color;
        public bool IsReturningChatter;
        public bool IsFirstMessage;

        //  badges
        public bool IsModerator;
        public bool IsPartner;
        public bool IsPremium;
        public bool IsAdmin;
        public bool IsStuff;
        public bool IsVip;
        public int SubscriberLevel;
        public int Bits;
        public int FounderNumber;
        public int HypeTrainLevel;

        //  other...
        public bool DidSayHello = false;
    }
}