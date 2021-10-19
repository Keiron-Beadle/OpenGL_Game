﻿using OpenGL_Game.Components;
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
        public bool HasCollisions = false;

        public List<Tuple<Entity,Entity,Vector3, Vector3>> Collisions { get; private set; }

        public SystemCollision()
        {
            Name = "System Collision";
            actors = new List<Entity>();
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
            }
        }
    }
}
