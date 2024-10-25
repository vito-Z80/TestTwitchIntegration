using Newtonsoft.Json;

namespace Data
{
    public class AppSettingsData
    {

        [JsonProperty("as")] public float avatarsSpeed = 0.3f;
        [JsonProperty("aw")] public float avatarAreaWidth = 1.0f;
        [JsonProperty("ah")] public float avatarAreaHeight = 0.5f;
        [JsonProperty("iw")] public float imageScale = 1.0f;
        [JsonProperty("ws")] public int cameraPpu = 13; 
        
        [JsonProperty("px")] public float areaPosX;
        [JsonProperty("py")] public float areaPosY;
        
        
        [JsonProperty("ww")] public int windowWidth = 640;
        [JsonProperty("wh")] public int windowHeight = 360;
        
        [JsonProperty("ps")] public bool pixelSnapping;
        [JsonProperty("r")] public bool randomSpeedEnabled;
    }
}