using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.OBJLoader
{
    public interface ITexture
    {
        int ID { get; set; }

        void DeleteTexture();
    }
}
