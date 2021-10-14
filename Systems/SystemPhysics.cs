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
        protected const ComponentTypes MotionMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);
        protected const ComponentTypes RotationMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_ROTATION);

        public string Name { get; protected set; }

        public SystemPhysics()
        {
            Name = "System Physics";
        }

        public void OnAction(Entity entity)
        {
            DoMotion(ref entity);
            DoRotation(ref entity);
        }

        private void DoRotation(ref Entity entity)
        {
            if ((entity.Mask & RotationMASK) != RotationMASK) return;

            List<IComponent> components = entity.Components;

            IComponent transformComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
            });

            IComponent rotationComponent = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_ROTATION;
            });

            Vector3 rotationRate = (rotationComponent as ComponentRotation).Rotation;

            ((ComponentTransform)transformComponent).Rotation += rotationRate * GameScene.dt;
        }

        private void DoMotion(ref Entity entity)
        {
            if ((entity.Mask & MotionMASK) != MotionMASK) return;

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
