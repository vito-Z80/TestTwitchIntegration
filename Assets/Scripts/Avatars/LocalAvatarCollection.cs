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

        public Dictionary<string, AvatarData> GenerateAvatars()
        {
            m_textureCounter = 0;
            var path = $"{Application.streamingAssetsPath}/{AvatarsFolder}";

            //  dict<avatarName, dict<state, indices>>; state = left, right, idle, attack...;  indices = uv region indices
            Dictionary<string, AvatarData> avatars = new Dictionary<string, AvatarData>();
            //  имена папок аватаров
            var avatarFolderPaths = Directory.GetDirectories(path);
            foreach (var avatarFolder in avatarFolderPaths)
            {
                var avtar = AvatarVariants(Directory.GetDirectories(avatarFolder));
                if (avtar != null)
                {
                    var avatarName = avatarFolder.Split("\\").Last().ToLower();
                    var avatarData = new AvatarData
                    {
                        AvatarName = avatarName,
                        Animations = avtar
                    };
                    avatars.Add(avatarName, avatarData);
                }
            }

            //  add missing data
            foreach (var avatar in avatars.Values)
            {
                var avatarStateKeys = avatar.Animations.Keys;
                var keysFlag = (int)avatarStateKeys.Aggregate((acc, num) => acc | num);

                switch (keysFlag)
                {
                    case 1: // need: right, left
                    case 9: // need: right, left, атака как в случае 14 не зеркалится.
                        CopyMissingAvatarData(avatar, AvatarState.Idle, AvatarState.Right);
                        CopyMissingAvatarData(avatar, AvatarState.Idle, AvatarState.Left);
                        break;
                    case 2: //  need: right
                    case 3: //  need: right
                        CopyMissingAvatarData(avatar, AvatarState.Left, AvatarState.Right, true);
                        break;
                    case 4: //  need: left
                    case 5: //  need: left
                        CopyMissingAvatarData(avatar, AvatarState.Right, AvatarState.Left, true);
                        break;
                    case 6: //  skip
                    case 7:
                    case 14: // в этом случае атака не зеркалится. Допустим это массовая атака вызовом какой то магии. 
                    case 15: // как и 14    
                        break;
                    case 8: // ignore в случаях когда анимация атаки только одна = уничтожить аватар.
                        avatar.Animations.Remove(AvatarState.Attack);
                        break;
                    case 10: // need: attackLeft, attackRight, right
                    case 11: // need: attackLeft, attackRight, right
                        CopyMissingAvatarData(avatar, AvatarState.Attack, AvatarState.AttackLeft);
                        CopyMissingAvatarData(avatar, AvatarState.Attack, AvatarState.AttackRight, true);
                        CopyMissingAvatarData(avatar, AvatarState.Left, AvatarState.Right, true);
                        avatar.Animations.Remove(AvatarState.Attack);
                        break;
                    case 12: // need: attackLeft, attackRight, left
                    case 13: // need: attackLeft, attackRight, left
                        CopyMissingAvatarData(avatar, AvatarState.Attack, AvatarState.AttackLeft, true);
                        CopyMissingAvatarData(avatar, AvatarState.Attack, AvatarState.AttackRight);
                        CopyMissingAvatarData(avatar, AvatarState.Right, AvatarState.Left, true);
                        avatar.Animations.Remove(AvatarState.Attack);
                        break;
                }
            }


            // AddMissingTextures(m_textures, avatars);

            m_avatarsAtlas = new AvatarsAtlas();
            m_avatarsAtlas.GenerateAtlas(m_textures);
            return avatars;
        }

        void CopyMissingAvatarData(AvatarData avatarData, AvatarState existingState, AvatarState newState, bool flipX = false)
        {
            var animations = new List<AvatarAnimationData>();
            foreach (var existingAnimation in avatarData.Animations[existingState])
            {
                var animation = new AvatarAnimationData
                {
                    AnimationIndices = existingAnimation.AnimationIndices,
                    FlipX = flipX,
                    SubName = existingAnimation.SubName,
                };
                animations.Add(animation);
            }

            avatarData.Animations.Add(newState, animations.ToArray());
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

        void ProcessDirectory(string path, List<string> filesByDirectory)
        {
            filesByDirectory.Add(Path.GetFullPath(path));
            foreach (var directory in Directory.GetDirectories(path))
            {
                ProcessDirectory(directory, filesByDirectory);
            }
        }


        public Texture2D GetAtlas() => m_avatarsAtlas.GetAtlasTexture();
        public Sprite[] GetSprites() => m_avatarsAtlas.GetSprites();

        [CanBeNull]
        Dictionary<AvatarState, AvatarAnimationData[]> AvatarVariants(string[] folders)
        {
            var dict = new Dictionary<AvatarState, AvatarAnimationData[]>();

            var nestedFolders = new List<string>();
            //  имена папок вариантов одной анимации (idle, left, right...)
            foreach (var folder in folders)
            {
                Debug.Log(folder);
                nestedFolders.Clear();
                ProcessDirectory(folder, nestedFolders);

                foreach (var f in nestedFolders)
                {
                    Debug.Log(f);
                }

                var subAnimations = new List<AvatarAnimationData>();

                foreach (var folderPath in nestedFolders)
                {
                    var texIndices = new List<int>();
                    CollectTexturesByPath(folderPath, texIndices);

                    var defaultFolderName = folder.Split("\\").Last();
                    var nestedFolderName = folderPath.Split("\\").Last();


                    if (texIndices.Count > 0)
                    {
                        var aad = new AvatarAnimationData
                        {
                            AnimationIndices = texIndices.ToArray(),
                            SubName = defaultFolderName == nestedFolderName ? "Default variant" : nestedFolderName,
                        };
                        subAnimations.Add(aad);
                    }
                }

                if (subAnimations.Count > 0)
                {
                    dict.Add(GetAvatarState(folder.Split("\\").Last()), subAnimations.ToArray());
                }
            }

            if (dict.Count == 0) return null;
            return dict;
        }

        void CollectTexturesByPath(string folderPath, List<int> texIndices)
        {
            var fileNames = Directory.GetFiles(folderPath, "*.png");

            foreach (var fileName in fileNames)
            {
                var imageData = File.ReadAllBytes(fileName);
                var texture = new Texture2D(1, 1);

                texture.LoadImage(imageData);
                texture.minimumMipmapLevel = 0;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Point;
                m_textures.Add(texture);
                texIndices.Add(m_textureCounter);
                m_textureCounter++;
            }
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