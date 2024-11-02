using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class UVRect
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public UVRect(Rect rect)
        {
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
        }

        public Rect ToRect() => new Rect(x, y, width, height);
    }
}