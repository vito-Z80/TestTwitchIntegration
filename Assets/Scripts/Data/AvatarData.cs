using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data
{
    public class AvatarData
    {
        [JsonProperty("n")] public string AvatarName;
        [JsonProperty("s")] public float Speed;
        [JsonProperty("a")] public int Access = 0;
        [JsonProperty("am")] public Dictionary<AvatarState, AvatarAnimationData[]> Animations;
    }

    public class AvatarAnimationData
    {
        [JsonProperty("f")] public bool FlipX;
        [JsonProperty("ps")] public float FramePerSecond;
        [JsonProperty("ai")] public int[] AnimationIndices;
        [JsonProperty("sn")] public string SubName;
    }
}