using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenGL_Game.Components;

namespace OpenGL_Game.Objects
{
    public enum TAGS
    {
        NONE,
        WORLD,
        ENEMY,
        PLAYER
    }

    class Entity
    {
        readonly string name;
        readonly TAGS tag;
        List<IComponent> componentList = new List<IComponent>();
        ComponentTypes mask;
 
        public Entity(string name, TAGS optionalTag = TAGS.NONE)
        {
            this.name = name;
            this.tag = optionalTag;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            componentList.Add(component);
            mask |= component.ComponentType;
        }

        public IComponent FindComponentByType(ComponentTypes type)
        {
            return componentList.Find(delegate (IComponent component)
            {
                return component.ComponentType == type;
            });
        }

        public String Name
        {
            get { return name; }
        }

        public TAGS Tag { get { return tag; } }

        public ComponentTypes Mask
        {
            get { return mask; }
        }

        public List<IComponent> Components
        {
            get { return componentList; }
        }
    }
}
