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

        public enum CONTROLS
        {
            Forward,
            Backward,
            Left,
            Right,
            Escape,
            GameOver,
        }

        public InputManager(SceneManager pSceneManager)
        {
            sceneManager = pSceneManager;
        }

        public abstract void Update(FrameEventArgs e);

        protected abstract void SaveControls();

        protected abstract void LoadControls();

        public abstract void UpdateFPSCamera(ref Camera camera, float dt);
    }
}
