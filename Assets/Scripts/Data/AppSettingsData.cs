﻿using Newtonsoft.Json;

namespace Data
{
    public class AppSettingsData
    {
        [JsonProperty("as")] public float avatarsSpeed = 0.3f; //  0-1
        [JsonProperty("aw")] public float avatarAreaWidth = 1.0f; //  0-1
        [JsonProperty("ah")] public float avatarAreaHeight = 0.5f; //  0-1
        [JsonProperty("iw")] public float imageScale = 1.0f; //  0-1
        
        [JsonProperty("px")] public float areaPosX; //  considering ppu 
        [JsonProperty("py")] public float areaPosY; //  considering ppu
        
        [JsonProperty("ws")] public int cameraPpu = 13; //  1-24
        [JsonProperty("ww")] public int windowWidth = 640; //  pixel
        [JsonProperty("wh")] public int windowHeight = 360; //  pixel

        [JsonProperty("ps")] public bool pixelSnapping;
        [JsonProperty("r")] public bool randomSpeedEnabled;
    }
}