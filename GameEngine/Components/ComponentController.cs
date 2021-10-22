using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components
{
    abstract class ComponentController : IComponent
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_CONTROLLER; } }
        protected ComponentTransform transform;
        public abstract void Update(SystemAudio audioSystem, float dt);
    }
}
