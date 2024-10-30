using UnityEngine;

namespace Data
{
    public class ChatUserData
    {

        public string UserName;
        public Color Color;
        public bool IsFirstMessage;
        public bool IsModerator;
        public bool IsSubscriber;
        public bool IsReturningChatter;

        public bool DidSayHello = false;
    }
}