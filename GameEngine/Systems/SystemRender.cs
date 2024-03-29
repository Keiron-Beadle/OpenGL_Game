﻿using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Systems
{
    abstract class SystemRender : ASystem
    { 

        public SystemRender()
        {
            MASK = ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TRANSFORM;
            Name = "System Render";
        }

        public abstract void Draw(Matrix4 mat4, OpenGLGeometry geom, ComponentShader shader);
  
    }
}
