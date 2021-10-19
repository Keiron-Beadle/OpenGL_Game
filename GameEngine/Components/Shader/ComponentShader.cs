using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    abstract class ComponentShader : IComponent
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_SHADER; } }

        public int ShaderID { get; private set; }

        public ComponentShader(string vertexSource, string fragmentSource)
        {
            ShaderID = GL.CreateProgram();
            GL.AttachShader(ShaderID, ResourceManager.LoadOpenGLShader(vertexSource, ShaderType.VertexShader));
            GL.AttachShader(ShaderID, ResourceManager.LoadOpenGLShader(fragmentSource, ShaderType.FragmentShader));
            GL.LinkProgram(ShaderID);
            Console.WriteLine(GL.GetProgramInfoLog(ShaderID));
        }

        public void BindShader() { GL.UseProgram(ShaderID); }
        public void UnbindShader() { GL.UseProgram(0); }
        public abstract void ApplyShader(Matrix4 model, IGeometry geometry);
    }
}
