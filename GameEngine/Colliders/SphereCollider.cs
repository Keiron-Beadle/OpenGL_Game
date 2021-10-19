using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
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
        public float Radius;
        public float RadiusSquared;


        public SphereCollider(Vector3 pCenter, float pRadius)
        {
            Center = pCenter;
            Radius = pRadius;
            RadiusSquared = Radius * Radius;
        }

        public bool Intersect(ComponentSphereCollider spherecomp)
        {
            float dist = Vector3.Distance(Center, spherecomp.Collider.Center);
            return dist < (Radius + spherecomp.Collider.Radius);
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

        public void Update(ComponentTransform transform)
        {
            Center = transform.Position;
        }
    }
}
