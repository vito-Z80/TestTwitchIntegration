using Newtonsoft.Json;
using TMPro;

namespace Data
{
    public class AppSettingsData
    {
        
        [JsonProperty("px")] public float areaPosX; //  0-1
        [JsonProperty("py")] public float areaPosY; //  0-1
        [JsonProperty("aw")] public float avatarAreaWidth = 1.0f; //  0-1
        [JsonProperty("ah")] public float avatarAreaHeight = 0.5f; //  0-1
        
        [JsonProperty("as")] public float avatarsSpeed = 0.3f; //  0-1
        
        
        [JsonProperty("ix")] public float imageX = 0.0f;    //  0-1
        [JsonProperty("iy")] public float imageY = 0.0f;    //  0-1
        [JsonProperty("iw")] public float imageScale = 1.0f; //  0-1
        
        [JsonProperty("mv")] public float masterVolume = 0.5f;
        
        [JsonProperty("ws")] public int cameraPpu = 13; //  1-24
        [JsonProperty("ww")] public int windowWidth = 640; //  pixel
        [JsonProperty("wh")] public int windowHeight = 360; //  pixel

        [JsonProperty("ps")] public bool pixelSnapping;
        [JsonProperty("r")] public bool randomSpeedEnabled;

        // [JsonProperty("so")] public bool avatarSubscribersOnly;
        [JsonProperty("nc")] public bool displayNicknameColor = true;
        
        [JsonProperty("ua")] public bool useAvatars = true;
        [JsonProperty("ui")] public bool useImages = true;
        [JsonProperty("ug")] public bool useGreetings = true;
        
        [JsonProperty("at")] public string avatarNameTag = "!";
        [JsonProperty("it")] public string imageNameTag = "!";
        
        
        [JsonProperty("gia")] public TMP_Compatibility.AnchorPositions? greetingsImageAnchor;
        [JsonProperty("gis")] public float greetingsImageSize = 0.1f;
        [JsonProperty("gip")] public string greetingsImagePath = "";
        [JsonProperty("gap")] public string greetingsAudioPath = "";
    }
}