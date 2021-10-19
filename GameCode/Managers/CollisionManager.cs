using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Systems;
using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Managers
{
    class CollisionManager
    {
        private int counter = 0;
        private SystemCollision collisionSystem;

        public CollisionManager(SystemCollision pCollisionSystem)
        {
            collisionSystem = pCollisionSystem;
        }

        public void Update()
        {
            if (collisionSystem.HasCollisions)
            {
                ProcessCollisions();
            }
        }

        private void ProcessCollisions()
        {
            var collisions = collisionSystem.Collisions;
            foreach (var collision in collisions)
            {
                TAGS tag1 = collision.Item1.Tag;
                TAGS tag2 = collision.Item2.Tag;
                counter++;
                if (tag1 == TAGS.PLAYER && tag2 == TAGS.WORLD || tag1 == TAGS.WORLD && tag2 == TAGS.PLAYER)
                {
                    Entity p1 = collision.Item1;
                    ComponentTransform transform = p1.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                    Vector3 pushbackVector = collision.Item4;
                    pushbackVector.Y = 0.0f;
                    transform.Position += pushbackVector;
                    //ComponentVelocity velocity = p1.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
                    //velocity.Velocity -= collision.Item3 * Vector3.Dot(velocity.Velocity, collision.Item3);
                    //Console.WriteLine($"Player collided with walls: {counter}");
                }
                else if (tag1 == TAGS.PLAYER && tag2 == TAGS.ENEMY || tag1 == TAGS.ENEMY && tag2 == TAGS.PLAYER)
                {
                    Console.WriteLine("Player collided with enemy.");
                }
            }
        }
    }
}
