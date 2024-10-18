using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Avatars
{
    public class AvatarsAtlas
    {
        const int AtlasWidth = 4096;
        const int AtlasHeight = 4096;

        Rect[] m_uvRects;
        Texture2D m_atlasTexture;
        Sprite[] m_sprites;

        public void GenerateAtlas(List<Texture2D> textures, Dictionary<string, Dictionary<AvatarState, int[]>> avatars)
        {
            m_atlasTexture = new Texture2D(AtlasWidth, AtlasHeight, TextureFormat.RGBA32, false, false)
            {
                minimumMipmapLevel = 0,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };

            m_uvRects = m_atlasTexture.PackTextures(textures.ToArray(), 2, AtlasWidth);

            if (m_uvRects.Length != textures.Count)
            {
                Debug.LogError("The number of uv textures does not match the number of pixels of the atlas");
            }

            m_sprites = new Sprite[textures.Count];
            for (var id = 0; id < m_uvRects.Length; id++)
            {
                var pixelRect = new Rect(
                    m_uvRects[id].x * m_atlasTexture.width,
                    m_uvRects[id].y * m_atlasTexture.height,
                    m_uvRects[id].width * m_atlasTexture.width,
                    m_uvRects[id].height * m_atlasTexture.height
                );
                var sprite = Sprite.Create(m_atlasTexture, pixelRect, new Vector2(0.5f, 0.0f), 16.0f, 0, SpriteMeshType.FullRect);

                m_sprites[id] = sprite;
            }
        }

        Vector2[] GetUV(Rect uv)
        {
            var uvs = new List<Vector2>();

            var leftBottom = new Vector2(uv.xMin, uv.yMin); // UV для вершины 0 (левый нижний угол текстуры)
            var rightBottom = new Vector2(uv.xMax, uv.yMin); // UV для вершины 1 (правый нижний угол текстуры)
            var rightTop = new Vector2(uv.xMax, uv.yMax); // UV для вершины 2 (правый верхний угол текстуры)
            var leftTop = new Vector2(uv.xMin, uv.yMax); // UV для вершины 3 (левый верхний угол текстуры)

            uvs.Add(leftBottom);
            uvs.Add(rightBottom);
            uvs.Add(rightTop);
            uvs.Add(leftTop);
            return uvs.ToArray();
        }

        Vector3[] GetVertices(Texture2D texture)
        {
            var vertices = new List<Vector3>();
            var w = texture.width / 16.0f / 2.0f; //  16.0f < ppu
            var h = texture.height / 16.0f / 2.0f;
            var leftBottom = new Vector3(-w, -h, 0); // Вершина 0: Левый нижний угол
            var rightBottom = new Vector3(w, -h, 0); // Вершина 1: Правый нижний угол
            var rightTop = new Vector3(w, h, 0); // Вершина 2: Правый верхний угол
            var leftTop = new Vector3(-w, h, 0); // Вершина 3: Левый верхний угол
            vertices.Add(leftBottom);
            vertices.Add(rightBottom);
            vertices.Add(rightTop);
            vertices.Add(leftTop);
            return vertices.ToArray();
        }

        Vector3[] GetVerticesFlipX(Texture2D texture)
        {
            var vertices = new List<Vector3>();
            var w = texture.width / 16.0f / 2.0f; //  16.0f < ppu
            var h = texture.height / 16.0f / 2.0f;
            var leftBottom = new Vector3(-w, -h, 0); // Вершина 0: Левый нижний угол
            var rightBottom = new Vector3(w, -h, 0); // Вершина 1: Правый нижний угол
            var rightTop = new Vector3(w, h, 0); // Вершина 2: Правый верхний угол
            var leftTop = new Vector3(-w, h, 0); // Вершина 3: Левый верхний угол
            vertices.Add(rightBottom);
            vertices.Add(leftBottom);
            vertices.Add(leftTop);
            vertices.Add(rightTop);
            return vertices.ToArray();
        }

        public Texture2D GetAtlasTexture() => m_atlasTexture;
        public Sprite[] GetSprites() => m_sprites;
    }
}