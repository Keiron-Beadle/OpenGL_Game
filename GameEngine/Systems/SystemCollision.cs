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
        public bool HasCollisions = false;

        public List<Tuple<Entity,Entity,Vector3, Vector3>> Collisions { get; private set; }

        public SystemCollision()
        {
            Name = "System Collision";
            actors = new List<Entity>();
            enemies = new List<Entity>();
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
                }
            }
        }

        public override void OnAction()
        {
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
        }

        private void DoCollisionDetection()
        {
            //will consider spatial segmentation if this is too intensive.
            
            //Test for collision between actors + world
            for (int i = 0; i < actors.Count; i++) //List of moving actors
            {
                for (int j = 0; j < entities.Count; j++) //List of world objects
                {
                    IComponent collider1 = actors[i].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                    IComponent collider2 = entities[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);

                    //if (collider1 is ComponentBoxCollider b1 && collider2 is ComponentBoxCollider b2)
                    //{
                    //    foreach (var box in b2.Colliders)
                    //    {
                    //        if (!b1.Colliders[0].Intersect(box)) continue;
                    //        var collision = new Tuple<Entity, Entity>(actors[i], entities[j]);
                    //        Collisions.Add(collision);
                    //    }
                    //}
                     if (collider1 is ComponentSphereCollider s1 && collider2 is ComponentBoxCollider b3)
                     {
                        foreach (var box in b3.Colliders)
                        {
                            var result = s1.Collider.Intersect(b3);
                            if (!result.Item1) continue;
                            var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[i], entities[j], result.Item2, result.Item3);
                            Collisions.Add(collision);
                        }
                     }
                }

                for (int j = 0; j < enemies.Count; j++)
                {
                    IComponent actorCollider = actors[i].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                    IComponent enemyCollider = enemies[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                    if (actorCollider is ComponentSphereCollider s1 && enemyCollider is ComponentBoxCollider b3)
                    {
                        var result = s1.Collider.Intersect(b3);
                        if (!result.Item1) goto End;
                        var collision = new Tuple<Entity, Entity, Vector3, Vector3>(actors[i], enemies[j], result.Item2, result.Item3);
                        Collisions.Add(collision);
                    }
                    End:
                    ComponentTransform actorTransform = actors[i].FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                    ComponentTransform enemyTransform = enemies[j].FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                    Vector2 actorPos = actorTransform.Position.Xz;
                    Vector2 enemyPos = enemyTransform.Position.Xz;
                    if (CheckForWallObstruction(enemyPos, actorPos, enemies[j]))
                    {
                        break;
                    }                                    
                }
            }
        }

        private bool CheckForWallObstruction(Vector2 enemyPos, Vector2 actorPos, Entity enemy)
        {
            for (int k = 0; k < entities.Count; k++)
            {
                ComponentBoxCollider wallCollider = entities[k].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentBoxCollider;
                foreach (var c in wallCollider.Colliders)
                {
                    Vector2 wallMin = new Vector2(c.WorldMin.X, c.WorldMin.Z);
                    Vector2 wallMax = new Vector2(c.WorldMax.X, c.WorldMax.Z);
                    if (LineCollisionCheck(enemyPos, actorPos, wallMin, wallMax))
                    {
                        var collision = new Tuple<Entity, Entity, Vector3, Vector3>(enemy, entities[k], Vector3.Zero, Vector3.Zero);
                        Collisions.Add(collision);
                        return true; //There is a wall obstruction
                    }
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
    }
}
