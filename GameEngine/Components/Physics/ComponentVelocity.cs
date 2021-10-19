using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentVelocity : IComponent
    {
        public Vector3 Velocity;

        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_VELOCITY; } }

        public ComponentVelocity(float x,float y, float z) : this(new Vector3(x,y,z)) { }
        public ComponentVelocity(Vector3 pVelocity)
        {
            Velocity = pVelocity;
        }
    }
}
