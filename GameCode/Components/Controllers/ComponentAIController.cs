using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.GameEngine.Pathing;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components.Controllers
{
    abstract class ComponentAIController : ComponentController
    {
        protected Random randomFunc;
        protected Vector3 nextLocation = Vector3.Zero;
        protected float speed;
        Vector3 originalStartPosition;

        public ComponentAIController(Entity ai)
        {
            randomFunc = new Random(System.DateTime.Now.Millisecond);
            transform = ai.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            originalStartPosition = transform.Position;
        }

        public void ResetPosition()
        {
            transform.Position = originalStartPosition;
        }

        protected void MoveToNextLocation(float dt)
        {
            if (nextLocation != Vector3.Zero) //we have next destination, move to it
            {
                Vector3 vec = (nextLocation - transform.Position);
                if (vec.Length > -0.01f && vec.Length < 0.0f) nextLocation = Vector3.Zero;
                if (float.IsNaN(vec.Length)) return;
                transform.Position += (vec.Normalized() * speed) * dt;
            }
        }
    }
}
