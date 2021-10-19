using OpenGL_Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Colliders
{
    interface ICollider
    {
        void Update(ComponentTransform transform);
    }
}
