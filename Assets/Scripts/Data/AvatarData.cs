using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data
{
    public class AvatarData
    {
        [JsonProperty("n")] public string AvatarName;
        [JsonProperty("a")] public int Access = 0;
        [JsonProperty("am")] public Dictionary<AvatarState, AvatarAnimationData[]> Animations;
    }

    public class AvatarAnimationData
    {
        [JsonProperty("f")] public bool FlipX;
        [JsonProperty("avs")] public float AvatarSpeed = 60.0f; //  on pixel per seconds.
        [JsonProperty("ans")] public float AnimationSpeed = 12.0f; //  on frame per seconds.
        [JsonProperty("ai")] public int[] AnimationIndices;
        [JsonProperty("sn")] public string SubName;
    }
}