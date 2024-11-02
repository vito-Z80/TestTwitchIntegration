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

        public Dictionary<string, Sprite> GetImages()
        {
            var path = $"{Application.streamingAssetsPath}/{LocalStorage.ImagesFolder}";


            Dictionary<string, Sprite> images = new();
            //  имена папок изображений
            var imageFolderPaths = Directory.GetDirectories(path);

            foreach (var imageFolder in imageFolderPaths)
            {
                var fileNames = Directory.GetFiles(imageFolder, "*.png");
             
                if (fileNames.Length == 0) continue;
                
                var texture = GetTexture(fileNames[0]);
                var sprite = GetSprite(texture);
                var name = imageFolder.Split("\\").Last().ToLower();
                images.Add(name, sprite);
            }

            return images;
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
                100.0f, 0, SpriteMeshType.FullRect);
            return sprite;
        }
    }
}