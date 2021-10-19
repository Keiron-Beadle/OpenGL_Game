using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Colliders
{
    class BoxCollider : ICollider
    {
        public Vector3 Min;
        public Vector3 Max;

        public Vector3 WorldMin;
        public Vector3 WorldMax;

        public BoxCollider(Vector3 pMin, Vector3 pMax, ComponentTransform transform)
        {
            Min = pMin;
            Max = pMax;
            Matrix4 model = Matrix4.CreateScale(transform.Scale) * Matrix4.CreateTranslation(transform.Position);
            WorldMax = (new Vector4(Max, 1.0f) * model).Xyz;
            WorldMin = (new Vector4(Min, 1.0f) * model).Xyz;
        }

        public BoxCollider(Vector3[] vertices, ComponentTransform transform)
        {
            Matrix4 rot = Matrix4.CreateRotationX(transform.Rotation.X) *
              Matrix4.CreateRotationY(transform.Rotation.Y) * Matrix4.CreateRotationZ(transform.Rotation.Z);
            Vector3[] rotatedVerts = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                rotatedVerts[i] = (new Vector4(vertices[i], 1.0f) * rot).Xyz;
            }
            FindMinMax(rotatedVerts);
            Matrix4 model = Matrix4.CreateScale(transform.Scale) * Matrix4.CreateTranslation(transform.Position);
            WorldMax = (new Vector4(Max, 1.0f) * model).Xyz;
            WorldMin = (new Vector4(Min, 1.0f) * model).Xyz;
        }

        private void FindMinMax(Vector3[] vertices)
        {
            Vector3 min = vertices[0];
            Vector3 max = vertices[0];

            foreach (Vector3 v in vertices)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    min = MinVec(min, vertices[i]);
                    max = MaxVec(max, vertices[i]);
                }

                Min = min;
                Max = max;
            }
        }

        protected Vector3 MinVec(Vector3 u, Vector3 v)
        {
            Vector3 minVec = v;

            if (u.X < v.X)
                minVec.X = u.X;

            if (u.Y < v.Y)
                minVec.Y = u.Y;

            if (u.Z < v.Z)
                minVec.Z = u.Z;

            return minVec;
        }

        protected Vector3 MaxVec(Vector3 u, Vector3 v)
        {
            Vector3 maxVec = v;

            if (u.X > v.X)
                maxVec.X = u.X;

            if (u.Y > v.Y)
                maxVec.Y = u.Y;

            if (u.Z > v.Z)
                maxVec.Z = u.Z;

            return maxVec;
        }

        public bool Intersect(SphereCollider sphere)
        {
            float squareDist = 0.0f;
            var center = sphere.Center;
            if (center.X < WorldMin.X) squareDist += (WorldMin.X - center.X) * (WorldMin.X - center.X);
            if (center.X > WorldMax.X) squareDist += (center.X - WorldMax.X) * (center.X - WorldMax.X);

            if (center.Y < WorldMin.Y) squareDist += (WorldMin.Y - center.Y) * (WorldMin.Y - center.Y);
            if (center.Y > WorldMax.Y) squareDist += (center.Y - WorldMax.Y) * (center.Y - WorldMax.Y);

            if (center.Z < WorldMin.Z) squareDist += (WorldMin.Z - center.Z) * (WorldMin.Z - center.Z);
            if (center.Z > WorldMax.Z) squareDist += (center.Z - WorldMax.Z) * (center.Z - WorldMax.Z);

            return squareDist <= sphere.RadiusSquared;
        }

        public bool Intersect(BoxCollider box)
        {
            if ((WorldMax.X >= box.WorldMin.X) && WorldMin.X <= box.WorldMax.X)
            {
                if ((WorldMax.Y < box.WorldMin.Y) || WorldMin.Y > box.WorldMax.Y)
                {
                    return false;
                }

                return (WorldMax.Z >= box.WorldMin.Z) && (WorldMin.Z <= box.WorldMax.Z);
            }
            return false;
        }

        public void Update(ComponentTransform transform)
        {
            Matrix4 model = Matrix4.CreateScale(transform.Scale) * Matrix4.CreateTranslation(transform.Position);
            WorldMax = (new Vector4(Max, 1.0f) * model).Xyz;
            WorldMin = (new Vector4(Min, 1.0f) * model).Xyz;
        }
    }
}
