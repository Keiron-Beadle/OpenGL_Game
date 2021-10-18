using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.OBJLoader
{
    class OpenGLTexture : ITexture
    {
        public int ID { get; set; }

        public void DeleteTexture()
        {
            GL.DeleteTexture(ID);
        }
    }
}
