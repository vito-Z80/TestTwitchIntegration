using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Data
{
    public class AvatarData
    {
        [JsonProperty("n")] public string AvatarName;
        [JsonProperty("s")] public float Speed;
        
        [JsonProperty("a")] public int Access = 0;

        [JsonProperty("aa")] [CanBeNull] public AvatarAnimationData[] AttackAnimationsIndices;
        [JsonProperty("ia")] [CanBeNull] public AvatarAnimationData[] IdleAnimationsIndices;
        [JsonProperty("la")] [CanBeNull] public AvatarAnimationData[] LeftAnimationsIndices;
        [JsonProperty("ra")] [CanBeNull] public AvatarAnimationData[] RightAnimationsIndices;
    }

    public class AvatarAnimationData
    {
        [JsonProperty("fps")] public float FramePerSecond;
        [JsonProperty("ai")] public int[] AnimationIndices;
    }
}