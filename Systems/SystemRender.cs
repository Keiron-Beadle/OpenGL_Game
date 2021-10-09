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

        protected int pgmID;
        protected int vsID;
        protected int fsID;
        protected int uniform_stex;
        protected int uniform_mmodelviewproj;
        protected int uniform_mmodel;
        protected int uniform_diffuse;  // OBJ NEW
        public string Name { get; protected set; }

        public SystemRender()
        {
            Name = "System Render";
        }

        public abstract void OnAction(Entity entity);
        public abstract void Draw(Matrix4 mat4, OpenGLGeometry geom);
    }
}
