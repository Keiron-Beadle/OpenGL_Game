using OpenGL_Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    abstract class Collider : IComponent
    {
        public ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_COLLIDER; } }

        public bool Intersect(ComponentBoxCollider box, ComponentSphereCollider sphere)
        {

            return false;
        }
        public bool Intersect(ComponentBoxCollider box1, ComponentBoxCollider box2)
        {
            return (box1.minX <= box2.maxX && box1.maxX >= box2.maxX) &&
                (box1.minY <= box2.maxY && box1.maxY >= box2.minY) &&
                (box1.minZ <= box2.maxZ && box1.maxZ >= box2.minZ);
        }
        public bool Intersect(ComponentSphereCollider sphere1, ComponentSphereCollider sphere2)
        {

            return false;
        }
    }
}
