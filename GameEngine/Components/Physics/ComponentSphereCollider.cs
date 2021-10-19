using OpenGL_Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentSphereCollider : Collider
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_COLLIDER; } }
    }
}
