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
        private float radius;
        public float RadiusSquared;


        public SphereCollider(Vector3 pCenter, float pRadius)
        {
            Center = pCenter;
            radius = pRadius;
            RadiusSquared = radius * radius;
        }

        public bool Intersect(ComponentSphereCollider spherecomp)
        {
            float dist = Vector3.Distance(Center, spherecomp.Collider.Center);
            return dist < (radius + spherecomp.Collider.radius);
        }

        public bool Intersect(ComponentBoxCollider box) 
        {
            foreach (var b in box.Colliders)
            {
                //if (b.Intersect(this))
                //    return true;
            }
            return false;
        }

        public void Update(ComponentTransform transform)
        {
            Center = transform.Position;
        }
    }
}
