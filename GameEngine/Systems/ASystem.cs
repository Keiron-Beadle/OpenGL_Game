using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Systems
{
    abstract class ASystem
    {
        protected List<int> nullIndices;
        protected List<Entity> entities;
        protected ComponentTypes MASK;

        public string Name
        {
            get;
            protected set;
        }

        public ASystem()
        {
            nullIndices = new List<int>();
            entities = new List<Entity>();
        }

        public abstract void OnAction();

        public abstract void RemoveEntity(Entity entity);

        public virtual void InitialiseEntities(EntityManager em)
        {
            foreach (Entity e in em.Entities())
            {
                if ((e.Mask & MASK) == MASK)
                {
                    entities.Add(e); //If mask is the required for the current system, add to the system's cache
                }
            }
        }

    }
}
