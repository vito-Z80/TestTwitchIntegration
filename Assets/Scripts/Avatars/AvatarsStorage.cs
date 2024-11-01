using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Avatars
{
    public static class AvatarsStorage
    {
        static readonly LocalAvatarCollection LocalAvatarCollection= new LocalAvatarCollection();
        static readonly Dictionary<string,  AvatarData> Avatars = LocalAvatarCollection.GenerateAvatars();

       

        public static AvatarData GetAvatarData(string avatarName)
        {
            return Avatars.GetValueOrDefault(avatarName, null);
        }

        public static string[] GetAvatarNames() => Avatars.Keys.ToArray();
        // public Texture2D GetAvatarTexture() => m_localAvatarCollection.GetAtlas();
        public static Sprite[] GetSprites() => LocalAvatarCollection.GetSprites();
    }
}