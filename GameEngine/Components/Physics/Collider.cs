using OpenGL_Game.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    abstract class Collider : IComponent
    {
        public ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_COLLIDER; } }
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 WorldMin;
        public Vector3 WorldMax;

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

        public bool Intersect(ComponentBoxCollider box, ComponentSphereCollider sphere)
        {

            return false;
        }
        public bool Intersect(ref ComponentBoxCollider box)
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
        public bool Intersect(ComponentSphereCollider sphere1, ComponentSphereCollider sphere2)
        {

            return false;
        }
    }
}
