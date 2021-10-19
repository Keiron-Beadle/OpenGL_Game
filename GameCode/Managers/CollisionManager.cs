using OpenGL_Game.GameEngine.Systems;
using OpenGL_Game.Objects;
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
                    Console.WriteLine($"Player collided with walls: {counter}");
                }
                else if (tag1 == TAGS.PLAYER && tag2 == TAGS.ENEMY || tag1 == TAGS.ENEMY && tag2 == TAGS.PLAYER)
                {
                    Console.WriteLine("Player collided with enemy.");
                }
            }
        }
    }
}
