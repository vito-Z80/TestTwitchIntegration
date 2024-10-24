using System;

namespace Data
{
    [Serializable]
    public class AuthorizationData
    {
        public string UserName;
        public string ChannelName;
        public string Redirect;
        public string ClientId;
    }
}