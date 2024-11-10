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
        AvatarsAtlas m_avatarsAtlas;
        readonly List<Texture2D> m_textures = new();
        int m_textureCounter;

        public Dictionary<string, AvatarData> GenerateAvatars()
        {
            m_textureCounter = 0;
            var path = LocalStorage.NormalizePath($"{Application.streamingAssetsPath}/{LocalStorage.AvatarsFolder}");


            //  TODO атлас не обновляется при удалении папки в аватарах. 03.11.2024 3:53:54: The number of uv textures does not match the number of pixels of the atlas. Too many images did not fit into the 4096x4096 atlas. The number of uv textures: 73
            //  атлас всегда загружается и видимо юзаются его данные. 03.11.2024 3:53:54: Loaded 85 UV Rects for atlas: C:\projects\unity\Twitch\Test\TestTwitchIntegration\Assets\StreamingAssets\Graphics\Avatars\atlasRects.json
            //  нужно подменять данные аватаров при пересоздании аталаса.


            m_avatarsAtlas = new AvatarsAtlas();
            var existingData = LocalStorage.LoadAvatarsData();

            if (LocalStorage.IsAvatarFolderNotChanged())
            {
                if (m_avatarsAtlas.UseLocalAtlas())
                {
                    if (existingData != null)
                    {
                        Log.LogMessage($"Loading local avatars atlas data.");
                        return existingData;
                    }
                }
            }

            var avatarsData = new Dictionary<string, AvatarData>();
            //  имена папок аватаров
            var avatarFolderPaths = LocalStorage.GetDirectories(path);
            foreach (var avatarFolder in avatarFolderPaths)
            {
                var avatarName = Path.GetFileName(avatarFolder).ToLower();

                var avatarVariants = AvatarVariants(LocalStorage.GetDirectories(avatarFolder));
                if (avatarVariants != null)
                {
                    //  create new if states exists
                    var avatarData = new AvatarData
                    {
                        AvatarName = avatarName,
                        Animations = avatarVariants,
                        Path = avatarFolder,
                    };
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
            if (existingData != null)
            {
                UpgradeAvatarsData(avatarsData, existingData);
            }

            return avatarsData;
        }


        void UpgradeAvatarsData(Dictionary<string, AvatarData> newData, Dictionary<string, AvatarData> existingData)
        {
            foreach (var (newAvatarName, newAvatarData) in newData)
            {
                if (existingData.TryGetValue(newAvatarName, out var existingAvatarData))
                {
                    //  Обновляем поля AvatarData
                    newAvatarData.Access = existingAvatarData.Access;
                    
                    var newAvatarsAnimationStates = newAvatarData.Animations.Keys.ToArray();
                    foreach (var newAvatarsAnimationState in newAvatarsAnimationStates)
                    {
                        if (existingAvatarData.Animations.TryGetValue(newAvatarsAnimationState, out var existingAnimationData))
                        {
                            Log.LogMessage($"Upgrading avatar {newAvatarName} to animation: {newAvatarsAnimationState}");
                            var newAnimationData = newData[newAvatarName].Animations[newAvatarsAnimationState];
                            UpgradeAnimationsData(newAnimationData, existingAnimationData);
                        }
                    }
                }
                else
                {
                    Log.LogMessage($"Adding avatar {newAvatarName} with animations: {string.Join(", ", newAvatarData.Animations.Keys)}");
                }
            }
        }


        void UpgradeAnimationsData(AvatarAnimationData[] newAnimationsData, AvatarAnimationData[] existingAnimationsData)
        {
            foreach (var nData in newAnimationsData)
            {
                var existingData = existingAnimationsData.FirstOrDefault(ead => ead.SubName == nData.SubName);
                if (existingData != null)
                {
                    //  Обновляем поля AvatarAnimationData
                    nData.AnimationSpeed = existingData.AnimationSpeed;
                    nData.AvatarSpeed = existingData.AvatarSpeed;
                    Log.LogMessage($"Upgrading subAnimation {nData.SubName}");
                }
            }
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
            foreach (var directory in LocalStorage.GetDirectories(path))
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
            //  имена папок вариантов одной из анимаций (idle, left, right...)
            foreach (var folder in folders)
            {
                nestedFolders.Clear();
                ProcessDirectory(folder, nestedFolders);
                var subAnimations = new List<AvatarAnimationData>();
                var defaultFolderName = Path.GetFileName(folder);
                foreach (var folderPath in nestedFolders)
                {
                    var texIndices = new List<int>();
                    CollectTexturesByPath(folderPath, texIndices);


                    var nestedFolderName = Path.GetFileName(folderPath);


                    if (texIndices.Count > 0)
                    {
                        var newAnimationData = new AvatarAnimationData
                        {
                            AnimationIndices = texIndices.ToArray(),
                            SubName = defaultFolderName == nestedFolderName ? "Default variant" : nestedFolderName,
                            // SubName =  nestedFolderName,
                        };

                        // var existingAnimationData = existingAnimationStates?
                        //     .FirstOrDefault(state => state.Key == GetAvatarState(defaultFolderName)).Value?
                        //     .FirstOrDefault(data => data.SubName == nestedFolderName);
                        // //  upgrade animation variant data.
                        // newAnimationData.AnimationSpeed = existingAnimationData?.AnimationSpeed ?? 12.0f;
                        // newAnimationData.AvatarSpeed = existingAnimationData?.AvatarSpeed ?? 60.0f;

                        subAnimations.Add(newAnimationData);
                    }
                }

                if (subAnimations.Count > 0)
                {
                    dict.Add(GetAvatarState(defaultFolderName), subAnimations.ToArray());
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