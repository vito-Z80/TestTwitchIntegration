using Newtonsoft.Json;

namespace Data
{
    public class AppSettingsData
    {

        [JsonProperty("as")] public float avatarsSpeed = 0.3f;
        [JsonProperty("aw")] public float avatarAreaWidth = 1.0f;
        [JsonProperty("ah")] public float avatarAreaHeight = 0.5f;
        [JsonProperty("iw")] public float imageScale = 1.0f;
        [JsonProperty("ws")] public float cameraPpu = 0.5f;     //  12ppu
        
        [JsonProperty("px")] public float areaPosX;
        [JsonProperty("py")] public float areaPosY;
        
        [JsonProperty("ps")] public bool pixelSnapping;
        [JsonProperty("r")] public bool randomSpeedEnabled;
    }
}