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
        public Vector2 DeltaAxis { get; protected set; } //Used for rotation of camera
        public Dictionary<string, bool> controlFlags;
        protected List<string> controls;

        public bool IsActive(string command) => controlFlags[command];

        public InputManager(SceneManager pSceneManager)
        {
            sceneManager = pSceneManager;
            controls = new List<string>();
            controlFlags = new Dictionary<string, bool>();
        }

        public abstract void Update(FrameEventArgs e);

        protected abstract void SaveXMLControls();

        protected abstract void LoadXMLControls();
    }
}
