using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentTransform : IComponent
    {
        Vector3 position;
        Vector3 scale;
        Vector3 rotation;

        public ComponentTransform() : this(Vector3.Zero, Vector3.One, Vector3.Zero) { }
        public ComponentTransform(float x, float y, float z) : this(new Vector3(x,y,z), Vector3.One, Vector3.Zero) { }
        public ComponentTransform(Vector3 pos) : this(pos, Vector3.One, Vector3.Zero) { }
        public ComponentTransform(Vector3 pos, Vector3 scale, Vector3 rotation)
        {
            position = pos;
            this.scale = scale;
            this.rotation = rotation;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; Notify(); }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; Notify(); }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; Notify(); }
        }

        public override ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TRANSFORM; }
        }
    }
}
