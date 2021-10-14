using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Systems
{
    class SystemPhysics : ISystem
    {
        protected const ComponentTypes MASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);

        public string Name { get; protected set; }

        public SystemPhysics()
        {
            Name = "System Physics";
        }

        public void OnAction(Entity entity)
        {
            DoMotion(ref entity);
        }

        private void DoMotion(ref Entity entity)
        {
            if ((entity.Mask & MASK) != MASK) return;

            List<IComponent> components = entity.Components;

            IComponent transformComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
            });

            IComponent velocityComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
            });

            Vector3 velocity = (velocityComponent as ComponentVelocity).Velocity;
            Vector3 position = (transformComponent as ComponentTransform).Position;

            ((ComponentTransform)transformComponent).Position += velocity * GameScene.dt;
        }
    }
}
