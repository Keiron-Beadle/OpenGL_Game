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
    class ComponentRollingController : ComponentAIController
    {
        private Random rnd = new Random();
        private float speedFactor = 1.0f;
        private float minSpeed = 0.4f;
        private Vector2 Max; //This makes the entity controlled 
        private Vector2 Min; //bounded to an area in space, i.e. it cannot leave the room
        private ComponentVelocity velocity;

        public ComponentRollingController(Entity entity, Vector2 pMaxPoint, Vector2 pMinPoint) : base(entity)
        {
            transform = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            velocity = entity.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
            Max = pMaxPoint;
            Min = pMinPoint;
            velocity.Velocity.X = (float)rnd.NextDouble() * speedFactor;
            velocity.Velocity.Z = (float)rnd.NextDouble() * speedFactor;
        }

        public override void Update(SystemAudio audioSystem, float dt)
        {
            if (transform.Position.X < Min.X)
            {
                transform.Position = new Vector3(Min.X,transform.Position.Y, transform.Position.Z); //Uncollide ball from wall
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.X = velocity.Velocity.X < 0 ? speed : -speed;
            }
            else if (transform.Position.X > Max.X)
            {
                transform.Position = new Vector3(Max.X, transform.Position.Y, transform.Position.Z); //Uncollide ball from wall
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.X = velocity.Velocity.X < 0 ? speed : -speed;
            }
            if (transform.Position.Z < Min.Y)
            {
                transform.Position = new Vector3(transform.Position.X,transform.Position.Y, Min.Y); //Uncollide ball from wall
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Z = velocity.Velocity.Z < 0 ? speed : -speed;
            }
            else if (transform.Position.Z > Max.Y)
            {
                transform.Position = new Vector3(transform.Position.X, transform.Position.Y, Max.Y); //Uncollide ball from wall
                float speed = (float)rnd.NextDouble() * speedFactor + minSpeed;
                velocity.Velocity.Z = velocity.Velocity.Z < 0 ? speed : -speed;
            }

        }
    }
}
