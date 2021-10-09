using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.OBJLoader
{
    interface IGeometry
    {
        void LoadObject(string filename);
        void RemoveGeometry();
    }
}
