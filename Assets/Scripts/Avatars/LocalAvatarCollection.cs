using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Avatars
{
    public class LocalAvatarCollection
    {
        const string AvatarsFolder = "Graphics/Avatars";
        AvatarsAtlas m_avatarsAtlas;
        readonly List<Texture2D> m_textures = new();
        int m_textureCounter;

        public Dictionary<string, Dictionary<AvatarState, int[]>> GenerateAvatars()
        {
            m_textureCounter = 0;
            var path = $"{Application.streamingAssetsPath}/{AvatarsFolder}";

            //  dict<avatarName, dict<state, indices>>; state = left, right, idle, attack...;  indices = uv region indices
            Dictionary<string, Dictionary<AvatarState, int[]>> avatars = new Dictionary<string, Dictionary<AvatarState, int[]>>();
            //  имена папок аватаров
            var avatarFolderPaths = Directory.GetDirectories(path);
            foreach (var avatarFolder in avatarFolderPaths)
            {
                var avtar = AvatarVariants(Directory.GetDirectories(avatarFolder));
                if (avtar != null)
                {
                    var avatarName = avatarFolder.Split("\\").Last().ToLower();
                    avatars.Add(avatarName, avtar);
                }
            }
            AddMissingTextures(m_textures, avatars);

            m_avatarsAtlas = new AvatarsAtlas();
            m_avatarsAtlas.GenerateAtlas(m_textures);
            return avatars;
        }


        void AddMissingTextures(List<Texture2D> textures, Dictionary<string, Dictionary<AvatarState, int[]>> avatars)
        {
            var counter = textures.Count;
            foreach (var avatar in avatars.Values)
            {
                if (avatar.ContainsKey(AvatarState.Right) && !avatar.ContainsKey(AvatarState.Left))
                {
                    //  create left flipX 
                    var indices = new List<int>();
                    foreach (var id in avatar[AvatarState.Right])
                    {
                        var flipTex = FlipTextureHorizontally(textures[id]);
                        m_textures.Add(flipTex);
                        indices.Add(counter);
                        counter++;
                    }
                    avatar.Add(AvatarState.Left, indices.ToArray());

                    //  add attack if exist
                    if (avatar.ContainsKey(AvatarState.Attack))
                    {
                        //  copy right attack
                        if (avatar.Remove(AvatarState.Attack, out var rightAttackIndices))
                        {
                            avatar.Add(AvatarState.AttackRight, rightAttackIndices);
                        }
                        //  create left attack
                        var leftAttackIndices = new List<int>();
                        foreach (var id in avatar[AvatarState.AttackRight])
                        {
                            var flipTex = FlipTextureHorizontally(textures[id]);
                            m_textures.Add(flipTex);
                            leftAttackIndices.Add(counter);
                            counter++;
                        }
                        avatar.Add(AvatarState.AttackLeft, leftAttackIndices.ToArray());
                    }
                }
                else if (avatar.ContainsKey(AvatarState.Left) && !avatar.ContainsKey(AvatarState.Right))
                {
                    //  create right flipX 
                    var indices = new List<int>();
                    foreach (var id in avatar[AvatarState.Left])
                    {
                        var flipTex = FlipTextureHorizontally(textures[id]);
                        m_textures.Add(flipTex);
                        indices.Add(counter);
                        counter++;
                    }

                    avatar.Add(AvatarState.Right, indices.ToArray());
                    
                    //  add attack if exist
                    if (avatar.ContainsKey(AvatarState.Attack))
                    {
                        //  copy left attack
                        if (avatar.Remove(AvatarState.Attack, out var leftAttackIndices))
                        {
                            avatar.Add(AvatarState.AttackLeft, leftAttackIndices);
                        }
                        //  create right attack
                        var rightAttackIndices = new List<int>();
                        foreach (var id in avatar[AvatarState.AttackLeft])
                        {
                            var flipTex = FlipTextureHorizontally(textures[id]);
                            m_textures.Add(flipTex);
                            rightAttackIndices.Add(counter);
                            counter++;
                        }
                        avatar.Add(AvatarState.AttackRight, rightAttackIndices.ToArray());
                    }
                    
                }

                if (avatar.ContainsKey(AvatarState.Idle) && !avatar.ContainsKey(AvatarState.Left))
                {
                    var left = new List<int>();
                    foreach (var id in avatar[AvatarState.Idle])
                    {
                        m_textures.Add(m_textures[id]);
                        left.Add(counter);
                        counter++;
                    }

                    avatar.Add(AvatarState.Left, left.ToArray());

                    var right = new List<int>();
                    foreach (var id in avatar[AvatarState.Idle])
                    {
                        m_textures.Add(m_textures[id]);
                        right.Add(counter);
                        counter++;
                    }

                    avatar.Add(AvatarState.Right, right.ToArray());
                }
            }
        }

        public Texture2D GetAtlas() => m_avatarsAtlas.GetAtlasTexture();
        public Sprite[] GetSprites() => m_avatarsAtlas.GetSprites();

        [CanBeNull]
        Dictionary<AvatarState, int[]> AvatarVariants(string[] folders)
        {
            var dict = new Dictionary<AvatarState, int[]>();

            //  имена папок вариантов одного спрайта (idle, left, right...)
            foreach (var folder in folders)
            {
                var fileNames = Directory.GetFiles(folder, "*.png");

                var texIndices = new List<int>();
                foreach (var filePath in fileNames)
                {
                    var imageData = File.ReadAllBytes(filePath);
                    var texture = new Texture2D(1, 1);

                    texture.LoadImage(imageData);
                    texture.minimumMipmapLevel = 0;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Point;
                    m_textures.Add(texture);
                    texIndices.Add(m_textureCounter);
                    m_textureCounter++;
                }

                if (texIndices.Count > 0)
                {
                    var avatarState = GetAvatarState(folder.Split("\\").Last());
                    dict[avatarState] = texIndices.ToArray();
                }
            }

            if (dict.Count == 0) return null;
            return dict;
        }

        Texture2D FlipTextureHorizontally(Texture2D original)
        {
            var flipped = new Texture2D(original.width, original.height);

            for (var x = 0; x < original.width; x++)
            {
                for (var y = 0; y < original.height; y++)
                {
                    flipped.SetPixel(original.width - 1 - x, y, original.GetPixel(x, y));
                }
            }

            flipped.Apply();
            return flipped;
        }

        AvatarState GetAvatarState(string avatarName)
        {
            switch (avatarName.ToLower())
            {
                case "idle":
                    return AvatarState.Idle;
                case "left":
                    return AvatarState.Left;
                case "right":
                    return AvatarState.Right;
                case "attack":
                    return AvatarState.Attack;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}