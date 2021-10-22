using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components
{
    abstract class ComponentController : IComponent
    {
        protected Vector3 originalPosition;
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_CONTROLLER; } }
        protected ComponentTransform transform;
        public virtual void ResetPosition()
        {
            transform.Position = originalPosition;
            if (float.IsNaN(transform.Position.X) || float.IsNaN(transform.Position.Z))
            {
                int test = 0;
            }
        }
        public abstract void Update(SystemAudio audioSystem, float dt);
    }
}
