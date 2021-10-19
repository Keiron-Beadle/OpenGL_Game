using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentRotation : IComponent
    {
        private Vector3 rotation;

        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_ROTATION; } }

        public ComponentRotation(float x, float y, float z) : this(new Vector3(x, y, z)) { }
        public ComponentRotation(Vector3 pRotation)
        {
            rotation = pRotation;
        }

        public Vector3 Rotation { get { return rotation; } set { rotation = value; } }
    }
}
