using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.Components;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Systems
{
    abstract class SystemRender : ISystem
    {
        protected const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY);

        public string Name { get; protected set; }

        public SystemRender()
        {
            Name = "System Render";
        }

        public abstract ITexture LoadTexture(string filepath, ref Dictionary<string, ITexture> textureDictionary);
        public abstract void OnAction(Entity entity);
        public abstract void Draw(Matrix4 mat4, OpenGLGeometry geom);
    }
}
