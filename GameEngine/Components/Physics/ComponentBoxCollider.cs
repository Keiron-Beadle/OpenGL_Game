using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentBoxCollider : Collider
    {
        private Vector3[] vertices;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;

        public ComponentBoxCollider(Vector3 center, float width, float height, float depth)
        {
            vertices = new Vector3[8];
            vertices[0] = new Vector3(center.X - width / 2, center.Y + height / 2, center.Z + depth / 2);
            vertices[1] = new Vector3(center.X + width / 2, center.Y + height / 2, center.Z + depth / 2);
            vertices[2] = new Vector3(center.X + width / 2, center.Y + height / 2, center.Z - depth / 2);
            vertices[3] = new Vector3(center.X - width / 2, center.Y + height / 2, center.Z - depth / 2);

            vertices[4] = new Vector3(center.X - width / 2, center.Y - height / 2, center.Z + depth / 2);
            vertices[5] = new Vector3(center.X + width / 2, center.Y - height / 2, center.Z + depth / 2);
            vertices[6] = new Vector3(center.X + width / 2, center.Y - height / 2, center.Z - depth / 2);
            vertices[7] = new Vector3(center.X - width / 2, center.Y - height / 2, center.Z - depth / 2);

            minX = center.X - width / 2 < center.X + width / 2 ? vertices[0].X : vertices[1].X;
            maxX = center.X - width / 2 > center.X + width / 2 ? vertices[0].X : vertices[1].X;

            minY = center.Y + height / 2 < center.Y - height / 2 ? vertices[4].Y : vertices[0].Y;
            maxY = center.Y + height / 2 > center.Y - height / 2 ? vertices[4].Y : vertices[0].Y;

            minZ = center.Z - depth / 2 < center.Z + depth / 2 ? vertices[2].Z : vertices[0].Z;
            maxZ = center.Z - depth / 2 > center.Z + depth / 2 ? vertices[2].Z : vertices[0].Z;
        }
    }
}
