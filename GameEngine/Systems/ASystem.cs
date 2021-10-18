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
        protected List<ComponentTypes> masks; //Some systems have multiple responsibilites -> multiple masks
        protected List<Entity> entities;

        public string Name
        {
            get;
            protected set;
        }

        public ASystem()
        {
            entities = new List<Entity>();
            masks = new List<ComponentTypes>();
        }

        public void Action()
        {
            for (int i = 0; i < masks.Count; i++) 
            {
                OnAction(masks[i]);
            }
        }
        public abstract void OnAction(ComponentTypes currentMask);

        public void InitialiseEntities(EntityManager em)
        {
            foreach (ComponentTypes MASK in masks)
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
}
