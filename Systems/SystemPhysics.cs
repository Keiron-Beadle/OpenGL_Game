using OpenGL_Game.Components;
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
            masks.Add(ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);
            masks.Add(ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_ROTATION);
        }

        public override void OnAction(ComponentTypes currentMask)
        {
            switch (currentMask)
            {
                case MotionMASK:
                    DoMotion();
                    break;
                case RotationMASK:
                    DoRotation();
                    break;
            }
        }

        private void DoRotation()
        {
            foreach (Entity entity in entities)
            {
                if ((entity.Mask & RotationMASK) != RotationMASK) continue;
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
        }

        private void DoMotion()
        {
            foreach (Entity entity in entities)
            {
                //Not a perfect solution, still need to run through entity list when dealing with
                //multiple mask systems, although it's a much SMALLER list. 
                if ((entity.Mask & MotionMASK) != MotionMASK) continue; 
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
}
