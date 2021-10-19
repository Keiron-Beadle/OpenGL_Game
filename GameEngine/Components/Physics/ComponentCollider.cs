using OpenGL_Game.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    abstract class ComponentCollider : IComponent
    {
        protected ComponentTransform transform;
        public override abstract ComponentTypes ComponentType { get; }


        public abstract void Update();

        public bool Intersect(ComponentSphereCollider sphere1, ComponentSphereCollider sphere2)
        {

            return false;
        }
    }
}
