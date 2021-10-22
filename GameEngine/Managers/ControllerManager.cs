using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Managers
{
    class ControllerManager
    {
        private List<ComponentController> controllers;
        private SystemAudio audioSystem;

        public ControllerManager(SystemAudio pAudioSystem, List<ComponentController> pControllers = null)
        {
            audioSystem = pAudioSystem;
            controllers = new List<ComponentController>();
            if (pControllers != null)
            {
                controllers.AddRange(pControllers);
            }
        }

        public void AddController(ComponentController pComponentController) { controllers.Add(pComponentController); }

        public void Update(float dt)
        {
            foreach (var c in controllers)
                c.Update(audioSystem, dt);
        }
    }
}
