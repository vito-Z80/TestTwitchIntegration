﻿using System;

namespace Data
{
    [Serializable]
    public class TokenData
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
        public string token_type;
    }
}