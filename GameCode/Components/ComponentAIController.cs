using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components
{
    enum AI_STATE
    {
        RANDOM_PATH,
        CHASE
    }

    class ComponentAIController : ComponentController
    {
        ComponentTransform target;
        float viewDist = 4.0f;
        Vector3 viewDir = new Vector3(0.0f, 0.0f, 3.0f);

        public bool ObstructedVision;

        public ComponentAIController(Entity ai, Entity pTarget)
        {
            target = pTarget.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            transform = ai.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
        }

        public void Update(SystemAudio audioSystem, float dt)
        {
            if (ObstructedVision)
            {
                Console.WriteLine("Vision obstructed");
            }
            else
            {
                Console.WriteLine("Clear vision to player");
            }

            //if (!ObstructedVision && PlayerVisible())
            //{
            //    Console.WriteLine("Player visible");
            //}
            //else
            //{
            //    Console.WriteLine("Player not visible");
            //}
            ObstructedVision = false; //Reset value for next frame.
        }

        private bool PlayerVisible()
        {
            Vector3 toTarget = (target.Position - transform.Position);
            if (toTarget.Length > viewDist) return false;
            toTarget.Normalize();
            float vDotT = Vector3.Dot(toTarget, viewDir.Normalized());
            return vDotT >= 0.707;
        }

        protected override void UpdateView(float dt)
        {
            
        }
    }
}
