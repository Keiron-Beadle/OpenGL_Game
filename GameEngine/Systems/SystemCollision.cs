using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Systems
{
    class SystemCollision : ASystem
    {
        private List<Entity> actors;
        private List<Entity> enemies;
        private List<Entity> pickups;
        public bool HasCollisions = false;
        private bool playerAlreadyAttacked = false;

        public List<Tuple<Entity,Entity,Vector3, Vector3>> Collisions { get; private set; }

        public SystemCollision()
        {
            Name = "System Collision";
            actors = new List<Entity>();
            enemies = new List<Entity>();
            pickups = new List<Entity>();
            Collisions = new List<Tuple<Entity, Entity, Vector3, Vector3>>();
            MASK = ComponentTypes.COMPONENT_COLLIDER | ComponentTypes.COMPONENT_TRANSFORM;
        }

        /// <summary>
        /// Must override for collision system as I have static objects and 
        /// moving entities I want to check for collision with these world objects
        /// </summary>
        /// <param name="em"></param>
        public override void InitialiseEntities(EntityManager em)
        {
            foreach (Entity entity in em.Entities())
            {
                if ((entity.Mask & MASK) == MASK)
                {
                    if (entity.Tag == TAGS.WORLD)
                    {
                        entities.Add(entity);
                    }
                    else if (entity.Tag == TAGS.PLAYER)
                    {
                        actors.Add(entity);
                    }
                    else if (entity.Tag == TAGS.ENEMY)
                    {
                        enemies.Add(entity);
                    }
                    else if (entity.Tag == TAGS.PICKUP)
                    {
                        pickups.Add(entity);
                    }
                }
            }
        }

        public override void OnAction()
        {
            playerAlreadyAttacked = false;
            Collisions.Clear();
            UpdateColliders();
            DoCollisionDetection();
            HasCollisions = Collisions.Count > 0;
        }

        private void UpdateColliders()
        {
            foreach (var entity in actors)
            {
                IComponent collider = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                ((ComponentCollider)collider).Update();
            }
            foreach (var entity in entities)
            {
                IComponent collider = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                ((ComponentCollider)collider).Update();
            }
            foreach (var entity in enemies)
            {
                IComponent collider = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                ((ComponentCollider)collider).Update();
            }
            foreach (var entity in pickups)
            {
                IComponent collider = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                ((ComponentCollider)collider).Update();
            }
        }

        private void DoCollisionDetection()
        {
            bool lineOfSight = true;

            for (int j = 0; j < actors.Count; j++)
            {
                for (int k = 0; k < pickups.Count; k++)
                {
                    ComponentSphereCollider sc1 = actors[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentSphereCollider;
                    var sc2 = pickups[k].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentSphereCollider;
                    var result = sc1.Colliders[1].Intersect(sc2.Colliders[0]);
                    if (!result.Item1) continue;
                    var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[j], pickups[k], result.Item2, result.Item3);
                    Collisions.Add(collision);
                }
            }

            for (int i = 0; i < entities.Count; i++) //List of world objects
            {
                //Collide terrain with actors/player (Precedes collision resolution)
                for (int j = 0; j < actors.Count; j++) //List of moving actors
                {
                    IComponent collider1 = actors[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                    IComponent collider2 = entities[i].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);

                    if (collider1 is ComponentSphereCollider s1 && collider2 is ComponentBoxCollider b3)
                    {
                        foreach (var box in b3.Colliders)
                        {
                            foreach (var sphere in s1.Colliders)
                            {
                                var result = sphere.Intersect(b3);
                                if (!result.Item1) continue;
                                var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[j], entities[i], result.Item2, result.Item3);
                                Collisions.Add(collision);
                            }
                        }
                    }

                    if (!lineOfSight) continue;
                    for (int k = 0; k < enemies.Count; k++)
                    {
                        if (playerAlreadyAttacked) break;
                        //Do player collisions with enemies 
                        IComponent actorCollider = actors[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                        IComponent enemyCollider = enemies[k].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                        if (actorCollider is ComponentSphereCollider s2 && enemyCollider is ComponentBoxCollider b4)
                        {
                            foreach (var sphere in s2.Colliders)
                            {
                                var result = sphere.Intersect(b4);
                                if (!result.Item1) continue;
                                var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[j], enemies[k], result.Item2, result.Item3);
                                Collisions.Add(collision);
                                playerAlreadyAttacked = true;
                            }
                        }
                        else if (actorCollider is ComponentSphereCollider s3 && enemyCollider is ComponentSphereCollider s4)
                        {
                            foreach (var sphere in s3.Colliders)
                            {
                                var result = sphere.Intersect(s4.Colliders[0]);
                                if (!result.Item1) continue;
                                var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[j], enemies[k], result.Item2, result.Item3);
                                Collisions.Add(collision);
                                playerAlreadyAttacked = true;
                            }
                        }
                    }
                    for (int k = 0; k < enemies.Count; k++) 
                    {
                        //Collisions for enemy to world -> player line of sight
                        if (enemies[k].Name != "Drone") continue;
                        ComponentTransform actorTransform = actors[j].FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                        ComponentTransform enemyTransform = enemies[k].FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                        Vector2 actorPos = actorTransform.Position.Xz;
                        Vector2 enemyPos = enemyTransform.Position.Xz;
                        if (CheckForWallObstruction(enemyPos, actorPos, enemies[k], entities[i]))
                        {
                            lineOfSight = false;
                        }
                    }
                }
            }
        }

        private bool CheckForWallObstruction(Vector2 enemyPos, Vector2 actorPos, Entity enemy, Entity wall)
        {
            ComponentBoxCollider wallCollider = wall.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentBoxCollider;
            foreach (var c in wallCollider.Colliders)
            {
                Vector2 wallMin = new Vector2(c.WorldMin.X, c.WorldMin.Z);
                Vector2 wallMax = new Vector2(c.WorldMax.X, c.WorldMax.Z);
                if (LineCollisionCheck(enemyPos, actorPos, wallMin, wallMax))
                {
                    var collision = new Tuple<Entity, Entity, Vector3, Vector3>(enemy, wall, Vector3.Zero, Vector3.Zero);
                    Collisions.Add(collision);
                    return true; //There is a wall obstruction
                }
            }   
            return false; //No wall obstruction
        }

        private bool LineCollisionCheck(Vector2 enemyPos, Vector2 targetPos, Vector2 wallStart, Vector2 wallEnd)
        {
            Vector2 wallVec = wallEnd - wallStart;
            Vector2 wallNormal = new Vector2(-wallVec.Y, wallVec.X);
            Vector2 wallToPlayer = targetPos - wallStart;
            Vector2 wallToEnemy = enemyPos - wallStart;
            float pDotN = Vector2.Dot(wallToPlayer, wallNormal);
            float eDotN = Vector2.Dot(wallToEnemy, wallNormal);

            Vector2 playerToEnemy = targetPos - enemyPos;
            Vector2 entityNormal = new Vector2(-playerToEnemy.Y, playerToEnemy.X);
            Vector2 entityToWallStart = wallStart - enemyPos;
            Vector2 entityToWallEnd = wallEnd - enemyPos;
            float eDotS = Vector2.Dot(entityToWallStart, entityNormal);
            float eDotE = Vector2.Dot(entityToWallEnd, entityNormal);

            return (pDotN * eDotN < 0) && (eDotS * eDotE < 0);
        }

        public override void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            actors.Remove(entity);
            enemies.Remove(entity);
            pickups.Remove(entity);
        }
    }
}
