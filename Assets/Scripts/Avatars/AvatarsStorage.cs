using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Avatars
{
    public static class AvatarsStorage
    {
        static readonly LocalAvatarCollection LocalAvatarCollection= new();
        static readonly Dictionary<string,  AvatarData> Avatars = LocalAvatarCollection.GenerateAvatars();

       
        public static Dictionary<string,  AvatarData> GetAvatars() => Avatars;

        public static AvatarData GetAvatarData(string avatarName)
        {
            return Avatars.GetValueOrDefault(avatarName, null);
        }

        public static string[] GetAvatarNames() => Avatars.Keys.ToArray();
        public static Texture2D GetAvatarsAtlas() => LocalAvatarCollection.GetAtlas();
        public static Sprite[] GetSprites() => LocalAvatarCollection.GetSprites();
    }
}