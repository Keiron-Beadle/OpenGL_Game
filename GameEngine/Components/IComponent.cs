using System;
using System.Collections.Generic;

namespace OpenGL_Game.Components
{
    [FlagsAttribute]
    enum ComponentTypes {
        COMPONENT_NONE     = 0,
	    COMPONENT_TRANSFORM = 1 << 0,
        COMPONENT_GEOMETRY = 1 << 1,
        COMPONENT_TEXTURE  = 1 << 2,
        COMPONENT_VELOCITY = 1 << 3,
        COMPONENT_ROTATION = 1 << 4,
        COMPONENT_SHADER = 1 << 5,
        COMPONENT_AUDIO = 1 << 6,
        COMPONENT_COLLIDER = 1 << 7,
        COMPONENT_CAMERA = 1 << 8,
        COMPONENT_CONTROLLER = 1 << 9
    }

    abstract class IComponent
    {
        public abstract ComponentTypes ComponentType
        {
            get;
        }

        List<IComponent> observers = new List<IComponent>();

        public void AttachObserver(IComponent comp)
        {
            observers.Add(comp);
        }
        
        public void DetachObserver(IComponent comp)
        {
            observers.Remove(comp);
        }

        public void Notify()
        {
            foreach (var observer in observers)
                observer.OnPropertyChanged(this);
        }

        protected virtual void OnPropertyChanged(IComponent changedComponent) { }
    }
}
