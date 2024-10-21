using System;
using Newtonsoft.Json;

namespace Data
{
    [Serializable]
    public class AppSettings
    {

        [JsonProperty("as")] public float avatarsSpeed;
        [JsonProperty("aw")] public float avatarAreaWidth;
        [JsonProperty("ah")] public float avatarAreaHeight;
        [JsonProperty("iw")] public float imageScale;
        [JsonProperty("ws")] public float worldSize;
        
        [JsonProperty("px")] public float areaPosX;
        [JsonProperty("py")] public float areaPosY;
        
        [JsonProperty("ps")] public bool pixelSnapping;
        [JsonProperty("r")] public bool randomSpeedEnabled;
        
        [JsonProperty("c")] public int chromakeyId;
        
    }
}