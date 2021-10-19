using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
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
        private List<Tuple<Entity, Entity>> collisionList;

        public SystemCollision()
        {
            Name = "System Collision";
            collisionList = new List<Tuple<Entity, Entity>>();
            MASK = ComponentTypes.COMPONENT_COLLIDER | ComponentTypes.COMPONENT_TRANSFORM;
        }

        public override void OnAction()
        {
            DoCollisionDetection();
            if (collisionList.Count > 0)
            {
                DoCollisionResponse();
            }
            collisionList.Clear();
        }

        private void DoCollisionResponse()
        {
            foreach (var collision in collisionList)
            {
                Entity item1 = collision.Item1;
                Entity item2 = collision.Item2;
                Console.WriteLine(item1.Name + " collided with " + item2.Name);
            }
        }

        private void DoCollisionDetection()
        {
            //will consider spatial segmentation if this is too intensive.
            for (int i = 0; i < entities.Count; i++)
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    if (entities[i] == entities[j]) continue;

                    IComponent collider1 = entities[i].Components.Find(delegate (IComponent component)
                    { return component.ComponentType == ComponentTypes.COMPONENT_COLLIDER; });
                    IComponent collider2 = entities[j].Components.Find(delegate (IComponent component)
                    { return component.ComponentType == ComponentTypes.COMPONENT_COLLIDER; });

                    if (collider1 is ComponentBoxCollider b1 && collider2 is ComponentBoxCollider b2)
                    {
                        if (!b1.Intersect(ref b2)) continue;
                        var collision = new Tuple<Entity, Entity>(entities[i], entities[j]);
                        if (!collisionList.Contains(collision) && !collisionList.Contains(new Tuple<Entity, Entity>(collision.Item2, collision.Item1)))
                        {
                            collisionList.Add(collision);
                        }

                    }
                }
            }
        }
    }
}
