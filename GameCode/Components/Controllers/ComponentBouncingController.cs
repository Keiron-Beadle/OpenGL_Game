using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components.Controllers
{
    class ComponentBouncingController : ComponentAIController
    {
        private Random rnd = new Random();
        private float speedFactor = 3.0f;
        private float minSpeed = 2.0f;
        private Vector2 Max;
        private Vector2 Min;
        private ComponentVelocity velocity;

        public ComponentBouncingController(Entity entity, Vector2 pMaxPoint, Vector2 pMinPoint) : base(entity)
        {
            transform = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            velocity = entity.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
            Max = pMaxPoint;
            Min = pMinPoint;
            velocity.Velocity.Y = (float)rnd.NextDouble() * speedFactor;
            velocity.Velocity.Z = (float)rnd.NextDouble() * speedFactor;
        }

        public override void Update(SystemAudio audioSystem, float dt)
        {
            if (transform.Position.Y < Min.X)
            {
                transform.Position = new Vector3(transform.Position.X, Min.X, transform.Position.Z);
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Y = velocity.Velocity.Y < 0 ? speed : -speed;
            }
            else if (transform.Position.Y > Max.X)
            {
                transform.Position = new Vector3(transform.Position.X, Max.X, transform.Position.Z);
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Y = velocity.Velocity.Y < 0 ? speed : -speed;
            }

            if (transform.Position.Z < Min.Y)
            {
                transform.Position = new Vector3(transform.Position.X, transform.Position.Y, Min.Y);
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Z = velocity.Velocity.Z < 0 ? speed : -speed;

            }
            else if (transform.Position.Z > Max.Y)
            {
                transform.Position = new Vector3(transform.Position.X, transform.Position.Y, Max.Y);
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Z = velocity.Velocity.Z < 0 ? speed : -speed;

            }
        }
    }
}
