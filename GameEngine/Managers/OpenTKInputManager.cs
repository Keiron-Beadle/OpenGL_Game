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
        private KeyboardState prevKeyState;
        private Dictionary<Key, string> controlBindings;

        //Mouse variables
        private Vector2 centerOfWindow;
        private MouseState prevMouseState;
        private bool mouseLeftClick = false; //Bool for if left mouse button clicked right now
        private bool firstUpdate = true;

        //Properties
        public bool LeftClicked { get { return mouseLeftClick; } }
        public bool AnyKeyPressed { get; private set; }

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
            UpdateKeyboard();
            UpdateMouse();
        }

        private void UpdateKeyboard()
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            for (int i = 0; i < controls.Count; i++)
            { controlFlags[controls[i]] = false; } //Reset to false
            foreach (var pair in controlBindings)
            {
                if (currentKeyState.IsKeyDown(pair.Key))
                    controlFlags[pair.Value] = true;

                if(pair.Value == "StopCollision")
                {
                    if (currentKeyState.IsKeyUp(pair.Key) && prevKeyState.IsKeyDown(pair.Key))
                    {
                        StopCollision = !StopCollision;
                    }
                }

                if (pair.Value == "StopDrone")
                {
                    if (currentKeyState.IsKeyUp(pair.Key) && prevKeyState.IsKeyDown(pair.Key))
                    {
                        StopDrone = !StopDrone;
                    }
                }
            }


            AnyKeyPressed = currentKeyState.IsAnyKeyDown && !prevKeyState.IsAnyKeyDown ? true : false;

            prevKeyState = currentKeyState;
        }


        private void UpdateMouse()
        {
            float xT = Mouse.GetCursorState().X;
            float xY = Mouse.GetCursorState().Y;
            MouseState currentMouseState = Mouse.GetState();
            DeltaAxis = new Vector2(xT - centerOfWindow.X,
                                     xY - centerOfWindow.Y);
            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                mouseLeftClick = true;
            else
                mouseLeftClick = false;

            if (firstUpdate)
            {
                DeltaAxis = Vector2.Zero;
                firstUpdate = false;
                return;
            }

            prevMouseState = currentMouseState;

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
