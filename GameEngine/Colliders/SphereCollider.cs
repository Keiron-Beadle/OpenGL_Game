using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Colliders
{
    class SphereCollider
    {
        public Vector3 Center;
        private Vector3 OffSet = Vector3.Zero;
        public float Radius;
        public float RadiusSquared;


        public SphereCollider(Vector3 pCenter, float pRadius, Vector3 pOffSet) : this(pCenter, pRadius)
        {
            OffSet = pOffSet;
        }

        public SphereCollider(Vector3 pCenter, float pRadius)
        {
            Center = pCenter;
            Radius = pRadius;
            RadiusSquared = Radius * Radius;
        }

        public Tuple<bool, Vector3, Vector3> Intersect(SphereCollider collider)
        {
            Vector3 direction = Vector3.UnitY;
            Vector3 difference = Vector3.Zero;
            bool intersecting = false;
            float dist = Vector3.Distance(Center, collider.Center);
            intersecting = dist < (Radius + collider.Radius);

            return new Tuple<bool, Vector3, Vector3>(intersecting, direction, difference);
        }

        public Tuple<bool, Vector3, Vector3> Intersect(ComponentBoxCollider box) 
        {
            foreach (var b in box.Colliders)
            {
                var result = b.Intersect(this);
                if (result.Item1)
                    return result;
            }
            return new Tuple<bool, Vector3, Vector3>(false, Vector3.Zero, Vector3.Zero);
        }

        public void Update(Vector3 transform)
        {
            Center = transform + OffSet;
        }
    }
}
