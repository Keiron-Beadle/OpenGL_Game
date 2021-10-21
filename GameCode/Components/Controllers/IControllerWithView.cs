using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components.Controllers
{
    interface IControllerWithView
    {
        void UpdateView(float dt);
    }
}
