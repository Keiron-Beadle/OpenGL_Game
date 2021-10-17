using OpenGL_Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.OBJLoader
{
    interface IGeometry
    {
        void Render(int diffuseLocation);
        void LoadObject(string filename, SystemRender renderSystem);
        void RemoveGeometry();
    }
}
