using System;

namespace Data
{
    [Serializable]
    public struct AreaOffset
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public AreaOffset(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }
}