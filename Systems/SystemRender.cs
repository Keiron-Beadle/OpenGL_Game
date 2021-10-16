using System;
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
            Name = "System Render";
            masks.Add(ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_SHADER);
        }

        public abstract ITexture LoadTexture(string filepath, ref Dictionary<string, ITexture> textureDictionary);
        public abstract void Draw(Matrix4 mat4, OpenGLGeometry geom, ComponentShader shader);
  
    }
}
