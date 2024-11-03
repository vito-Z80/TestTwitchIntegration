using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Avatars
{
    public class LocalAvatarCollection
    {
        AvatarsAtlas m_avatarsAtlas;
        readonly List<Texture2D> m_textures = new();
        int m_textureCounter;

        public Dictionary<string, AvatarData> GenerateAvatars()
        {
            m_textureCounter = 0;
            var path = Utils.NormalizePath($"{Application.streamingAssetsPath}/{LocalStorage.AvatarsFolder}");


            //  TODO атлас не обновляется при удалении папки в аватарах. 03.11.2024 3:53:54: The number of uv textures does not match the number of pixels of the atlas. Too many images did not fit into the 4096x4096 atlas. The number of uv textures: 73
            //  атлас всегда загружается и видимо юзаются его данные. 03.11.2024 3:53:54: Loaded 85 UV Rects for atlas: C:\projects\unity\Twitch\Test\TestTwitchIntegration\Assets\StreamingAssets\Graphics\Avatars\atlasRects.json
            //  нужно подменять данные аватаров при пересоздании аталаса.


            m_avatarsAtlas = new AvatarsAtlas();
            var avatarsData = LocalStorage.LoadAvatarsData() ?? new Dictionary<string, AvatarData>();

            if (LocalStorage.IsAvatarFolderNotChanged(path))
            {
                if (m_avatarsAtlas.UseLocalAtlas())
                {
                    if (avatarsData.Count > 0)
                    {
                        Log.LogMessage($"Loading local avatars atlas data.");
                        return avatarsData;
                    }
                }
            }
            
            //  имена папок аватаров
            var avatarFolderPaths = Utils.GetDirectory(path);
            foreach (var avatarFolder in avatarFolderPaths)
            {
                
                var avtarStates = AvatarVariants(Utils.GetDirectory(avatarFolder));
                var avatarName = Path.GetFileName(avatarFolder).ToLower();
                var avatarData = GetLocalJson(avatarFolder);
                if (avatarData == null)
                {
                    if (avtarStates != null)
                    {
                        avatarData = new AvatarData
                        {
                            AvatarName = avatarName,
                            Animations = avtarStates,
                            Path = avatarFolder,
                        };
                        avatarsData.Add(avatarName, avatarData);
                    }
                }
                else
                {
                    avatarsData.Add(avatarName, avatarData);
                }
            }

            //  add missing data
            foreach (var avatar in avatarsData.Values)
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

            m_avatarsAtlas.GenerateAtlas(m_textures);
            return avatarsData;
        }

        [CanBeNull]
        AvatarData GetLocalJson(string avatarFolder)
        {
            var filePath = Utils.NormalizePath($"{avatarFolder}/data.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<AvatarData>(json);
            }
        
            return null;
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

        void ProcessDirectory(string path, List<string> filesByDirectory)
        {
            filesByDirectory.Add(Path.GetFullPath(path));
            foreach (var directory in Utils.GetDirectory(path))
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
                nestedFolders.Clear();
                ProcessDirectory(folder, nestedFolders);
                var subAnimations = new List<AvatarAnimationData>();

                foreach (var folderPath in nestedFolders)
                {
                    var texIndices = new List<int>();
                    CollectTexturesByPath(folderPath, texIndices);

                    var defaultFolderName = Path.GetFileName(folder);
                    var nestedFolderName = Path.GetFileName(folderPath);


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