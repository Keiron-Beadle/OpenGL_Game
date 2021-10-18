using OpenGL_Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Systems
{
    class SystemAudio : ASystem
    {
        public SystemAudio()
        {
            Name = "System Audio";
            masks.Add(ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY);
        }

        public override void OnAction(ComponentTypes currentMask)
        {
            throw new NotImplementedException();
        }
    }
}
