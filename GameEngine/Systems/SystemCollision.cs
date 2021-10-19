using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Systems
{
    class SystemCollision : ASystem
    {
        private List<Entity> colliderables;
        public bool HasCollisions = false;

        public List<Tuple<Entity,Entity>> Collisions { get; private set; }

        public SystemCollision()
        {
            Name = "System Collision";
            colliderables = new List<Entity>();
            Collisions = new List<Tuple<Entity, Entity>>();
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
                        colliderables.Add(entity);
                    }

                }
            }
        }

        public override void OnAction()
        {
            HasCollisions = false;
            Collisions.Clear();
            UpdateColliders();
            DoCollisionDetection();
            if (Collisions.Count > 0)
            {
                HasCollisions = true;
            }
        }

        private void UpdateColliders()
        {
            foreach (var entity in colliderables)
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
            for (int i = 0; i < colliderables.Count; i++) //List of moving actors
            {
                for (int j = 0; j < entities.Count; j++) //List of world objects
                {
                    IComponent collider1 = colliderables[i].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);
                    IComponent collider2 = entities[j].FindComponentByType(ComponentTypes.COMPONENT_COLLIDER);

                    if (collider1 is ComponentBoxCollider b1 && collider2 is ComponentBoxCollider b2)
                    {
                        if (!b1.Intersect(ref b2)) continue;
                        var collision = new Tuple<Entity, Entity>(colliderables[i], entities[j]);
                        Collisions.Add(collision);
                    }
                }
            }
        }
    }
}
