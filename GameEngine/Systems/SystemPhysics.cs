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
            MASK = MotionMASK ;
        }

        public override void OnAction()
        {
            foreach (Entity entity in entities)
            {
                bool motion = (entity.Mask & MotionMASK) == MotionMASK;
                bool rotation = (entity.Mask & MotionMASK) == RotationMASK;

                if (rotation)
                    DoRotation(entity);
                if (motion)
                    DoMotion(entity);
            }
        }

        private void DoRotation(Entity entity)
        {
            IComponent transformComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);

            IComponent rotationComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_ROTATION);

            Vector3 rotationRate = (rotationComponent as ComponentRotation).Rotation;

            ((ComponentTransform)transformComponent).Rotation += rotationRate * GameScene.dt;
        }

        private void DoMotion(Entity entity)
        {
            IComponent transformComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);

            IComponent velocityComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY);

            Vector3 velocity = (velocityComponent as ComponentVelocity).Velocity;

            ((ComponentTransform)transformComponent).Position += velocity * GameScene.dt;
        }

        public override void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
