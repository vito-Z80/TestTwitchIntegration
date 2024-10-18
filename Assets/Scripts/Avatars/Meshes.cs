using UnityEngine;

namespace Avatars
{
    public static class Meshes
    {
        public static Mesh Mesh;

        // (-2.00, -2.00, 0.00),(2.00, -2.00, 0.00),(2.00, 2.00, 0.00),(-2.00, 2.00, 0.00)
        public static void Init()
        {
            Mesh = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-2f, -2f, 0),
                    new Vector3(2f, -2f, 0),
                    new Vector3(2f, 2f, 0),
                    new Vector3(-2f, 2f, 0)
                },
                triangles = new[] { 0, 1, 2, 2, 3, 0 }
            };
        }
    }
}