using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OpenGL_Game.Managers
{
    abstract class InputManager
    {
        protected SceneManager sceneManager;

        public InputManager(SceneManager pSceneManager)
        {
            sceneManager = pSceneManager;
        }

        public abstract void Update(FrameEventArgs e);

        protected abstract void SaveXMLControls();

        protected abstract void LoadXMLControls();

        public abstract void UpdateFPSCamera(ref Camera camera, float dt);
    }
}
