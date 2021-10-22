using OpenGL_Game.Components;
using OpenGL_Game.GameCode.Components;
using OpenGL_Game.GameCode.Components.Controllers;
using OpenGL_Game.GameEngine.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
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
        private EntityManager entityManager;
        private SystemManager systemManager;

        public CollisionManager(SystemCollision pCollisionSystem, EntityManager pEntityManager, SystemManager pSystemManager)
        {
            entityManager = pEntityManager;
            collisionSystem = pCollisionSystem;
            systemManager = pSystemManager;
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
                if (tag1 == TAGS.PLAYER && tag2 == TAGS.WORLD)
                {
                    if (collision.Item2.Name == "Portal" && GameScene.gameInstance.KeysCollected >= 3)
                    {
                        GameScene.gameInstance.GameOver = true;
                    }
                    Entity p1 = collision.Item1;
                    ComponentTransform transform = p1.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                    Vector3 pushbackVector = collision.Item4;
                    pushbackVector.Y = 0.0f;
                    transform.Position += pushbackVector;
                    //ComponentVelocity velocity = p1.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
                    //velocity.Velocity -= collision.Item3 * Vector3.Dot(velocity.Velocity, collision.Item3);
                    //Console.WriteLine($"Player collided with walls: {counter}");
                }
                else if (tag1 == TAGS.PLAYER && tag2 == TAGS.ENEMY)
                {
                    GameScene.gameInstance.PlayerLives--;
                    ComponentAIController aiController = collision.Item2.FindComponentByType(ComponentTypes.COMPONENT_CONTROLLER) as ComponentAIController;
                    aiController.ResetPosition();
                    ComponentPlayerController playerController = collision.Item1.FindComponentByType(ComponentTypes.COMPONENT_CONTROLLER) as ComponentPlayerController;
                    playerController.ResetPosition();
                }
                else if (tag1 == TAGS.ENEMY && tag2 == TAGS.WORLD || tag1 == TAGS.WORLD && tag2 == TAGS.ENEMY)
                {
                    Entity enemy = collision.Item1;
                    if (enemy.Name == "Drone")
                    {
                        ComponentDroneController enemyController = enemy.FindComponentByType(ComponentTypes.COMPONENT_CONTROLLER) as ComponentDroneController;
                        enemyController.ObstructedVision = true;
                    }
                }
                else if (tag1 == TAGS.PLAYER && tag2 == TAGS.PICKUP)
                {
                    Entity pickup = collision.Item2;
                    systemManager.RemoveEntityFromSystems(pickup);
                    entityManager.RemoveEntity(ref pickup);
                    GameScene.gameInstance.KeysCollected++;
                }
            }
        }
    }
}
