using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Managers
{
    class OpenTKInputManager : InputManager
    {
        //Keyboard variables
        protected KeyboardState prevKeyState;
        protected Dictionary<Key, string> controlBindings;
        public delegate void KeyboardPoll();
        protected KeyboardPoll KeyboardPoller;

        //Mouse variables
        public delegate void MousePoll();
        protected MousePoll MousePoller;
        protected Vector2 centerOfWindow;
        protected MouseState prevMouseState;
        protected bool mouseLeftClick = false; //Bool for if left mouse button clicked right now
        protected bool firstUpdate = true;

        //Properties
        public bool LeftClicked { get { return mouseLeftClick; } }
        public bool AnyKeyPressed { get; protected set; }
        public bool StopCollision { get; protected set; }
        public static bool StopDrone { get; protected set; }

        public OpenTKInputManager(SceneManager sceneManger) : base(sceneManger)
        {
            AddDefaultControlStrings();
            controlBindings = new Dictionary<Key, string>();
            DeltaAxis = new Vector2(0, 0);
            centerOfWindow = new Vector2((sceneManager.Bounds.Left + sceneManager.Bounds.Right) / 2,
                    (sceneManager.Bounds.Top + sceneManager.Bounds.Bottom) / 2);
            LoadXMLControls();
        }

        private void AddDefaultControlStrings()
        {
            controls.Add("Forward");
            controls.Add("Backward");
            controls.Add("Left");
            controls.Add("Right");
            controls.Add("Escape");
            controls.Add("Continue");
            foreach (string s in controls)
                controlFlags.Add(s, false);
        }

        ~OpenTKInputManager()
        {
            SaveXMLControls();
        }

        public override void Update(FrameEventArgs e)
        {
            //Poll the keyboard/mouse depending on game's implementation 
            KeyboardPoller?.Invoke();
            MousePoller?.Invoke();
        }
        
        protected override void SaveXMLControls()
        {
            ScriptManager.SaveTKControls(ref controlBindings);
        }

        protected override void LoadXMLControls()
        {
            ScriptManager.LoadTKControls(ref controlBindings);
        }
    }
}
