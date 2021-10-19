using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Systems
{
    class SystemPhysics : ASystem
    {
        protected const ComponentTypes MotionMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);
        protected const ComponentTypes RotationMASK = (ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_ROTATION);

        public SystemPhysics()
        {
            Name = "System Physics";
            MASK = MotionMASK | RotationMASK;
        }

        public override void OnAction()
        {
            foreach (Entity entity in entities)
            {
                bool motion = (entity.Mask & MotionMASK) == MotionMASK;
                bool rotation = (entity.Mask & MotionMASK) == RotationMASK;

                if (rotation)
                    DoRotation(entity.Components);
                if (motion)
                    DoMotion(entity.Components);
            }
        }

        private void DoRotation(List<IComponent> components)
        {
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

        private void DoMotion(List<IComponent> components)
        {
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
