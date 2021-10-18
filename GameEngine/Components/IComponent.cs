﻿using System;

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
        COMPONENT_COLLIDER = 1 << 7
    }

    interface IComponent
    {
        ComponentTypes ComponentType
        {
            get;
        }
    }
}
