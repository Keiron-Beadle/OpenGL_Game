using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentVelocity : IComponent
    {
        private Vector3 velocity;

        public ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_VELOCITY; } }

        public ComponentVelocity(float x,float y, float z) : this(new Vector3(x,y,z)) { }
        public ComponentVelocity(Vector3 pVelocity)
        {
            velocity = pVelocity;
        }

        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    }
}
