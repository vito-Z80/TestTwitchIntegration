using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Avatars
{
    public class AvatarsStorage
    {
        Dictionary<string, Dictionary<AvatarState, int[]>> m_avatars = new();
        LocalAvatarCollection m_localAvatarCollection;

        internal void Init()
        {
            m_localAvatarCollection = new LocalAvatarCollection();
            m_avatars = m_localAvatarCollection.GenerateAvatars();
        }

        public Dictionary<AvatarState, int[]> GetAvatar(string avatarName)
        {
            return m_avatars.GetValueOrDefault(avatarName, null);
        }

        public string[] GetAvatarNames() => m_avatars.Keys.ToArray();
        public Texture2D GetAvatarTexture() => m_localAvatarCollection.GetAtlas();
        public Sprite[] GetSprites() => m_localAvatarCollection.GetSprites();
    }
}