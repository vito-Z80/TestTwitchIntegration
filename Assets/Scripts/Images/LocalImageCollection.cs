using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Data;
using JetBrains.Annotations;
using Newtonsoft.Json.Converters;
using Unity.VisualScripting;
using UnityEngine;

namespace Images
{
    public class LocalImageCollection
    {
        const string AvatarsFolder = "Graphics/Images";
        public const string UserStartPattern = "_";

        public ImageData[] GetImages()
        {
            var path = $"{Application.streamingAssetsPath}/{AvatarsFolder}";


            List<ImageData> images = new List<ImageData>();
            //  имена папок изображений
            var imageFolderPaths = Directory.GetDirectories(path);

            foreach (var imageFolder in imageFolderPaths)
            {
                var data = GetImage(imageFolder);
                if (data == null) continue;
                images.Add(data);
            }

            return images.ToArray();
        }

        [CanBeNull]
        ImageData GetImage(string imageFolder)
        {
            var imageFileNames = Directory.GetFiles(imageFolder, "*.png");
            var textFileNames = Directory.GetFiles(imageFolder, "*.txt");

            if (imageFileNames.Length == 0) return null;

            var texture = GetTexture(imageFileNames[0]);
            var sprite = GetSprite(texture);

            var patterns = GetPatterns(textFileNames[0]);
            var name = imageFolder.Split("\\").Last().ToLower();
            patterns.Add($"{UserStartPattern}{name}");

            var data = new ImageData
            {
                Pattern = @"\b(" + string.Join("|", patterns) + @")\b[\s.,!?]*",
                Sprite = sprite
            };
            return data;
        }

        HashSet<string> GetPatterns(string textPath)
        {
            var textData = File.ReadAllText(textPath);
            var patterns = new HashSet<string>(textData.Split(",").Select(s => $"{UserStartPattern}{s.Trim().ToLower()}"));
            return patterns;
        }

        Texture2D GetTexture(string imagePath)
        {
            var imageData = File.ReadAllBytes(imagePath);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(imageData);
            texture.minimumMipmapLevel = 0;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        Sprite GetSprite(Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f),
                16.0f, 0, SpriteMeshType.FullRect);
            return sprite;
        }
    }
}