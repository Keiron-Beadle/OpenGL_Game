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
        public Dictionary<string, bool> controlFlags;
        private List<string> controls;

        //Mouse variables
        private Vector2 centerOfWindow;
        private MouseState prevMouseState;
        private bool mouseLeftClick = false; //Bool for if left mouse button clicked right now
        private bool firstUpdate = true;
        private float mouseHAngle = 0.0f;
        private float mouseVAngle = 0.0f;
        static float MOUSE_SENSITIVITY = 0.14f;

        //Properties
        public Vector2 DeltaMouse { get; private set; }
        public bool LeftClicked { get { return mouseLeftClick; } }
        public bool AnyKeyPressed { get; private set; }

        public bool IsActive(string command) => controlFlags[command];


        public OpenTKInputManager(SceneManager sceneManger) : base(sceneManger)
        {
            controls = new List<string>();
            controlFlags = new Dictionary<string, bool>();
            AddDefaultControlStrings();
            controlBindings = new Dictionary<Key, string>();
            DeltaMouse = new Vector2(0, 0);
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
        
        protected override void SaveXMLControls()
        {
            ScriptManager.SaveTKControls(ref controlBindings);
        }

        protected override void LoadXMLControls()
        {
            ScriptManager.LoadTKControls(ref controlBindings);
        }

        private void UpdateMouse()
        {
            float xT = Mouse.GetCursorState().X;
            float xY = Mouse.GetCursorState().Y;
            MouseState currentMouseState = Mouse.GetState();
            DeltaMouse = new Vector2(xT - centerOfWindow.X,
                                     xY - centerOfWindow.Y);
            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                mouseLeftClick = true;
            else
                mouseLeftClick = false;

            if (firstUpdate)
            {
                DeltaMouse = Vector2.Zero;
                firstUpdate = false;
                return;
            }

            prevMouseState = currentMouseState;
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
            }

            AnyKeyPressed = currentKeyState.IsAnyKeyDown && !prevKeyState.IsAnyKeyDown ? true : false;

            prevKeyState = currentKeyState;
        }

        public override void UpdateFPSCamera(ref Camera camera, float dt)
        {
            mouseHAngle += (-MOUSE_SENSITIVITY * DeltaMouse.X) * dt; //Append a new delta mouse change
            mouseVAngle += (-MOUSE_SENSITIVITY * DeltaMouse.Y) * dt;
            mouseVAngle = MathHelper.Clamp(mouseVAngle, -1.4f, 1.4f); //Clamp vertical so we can't look upside down
            Vector3 dir = new Vector3((float)Math.Cos(mouseVAngle) * (float)Math.Sin(mouseHAngle),
                                       (float)Math.Sin(mouseVAngle),
                                       (float)Math.Cos(mouseVAngle) * (float)Math.Cos(mouseHAngle));
            Vector3 right = new Vector3(
                                (float)Math.Sin(mouseHAngle - MathHelper.PiOver2),
                                 0.0f,
                                 (float)Math.Cos(mouseHAngle - MathHelper.PiOver2));
            Vector3 up = Vector3.Cross(right, dir);
            camera.cameraDirection = dir; //Update camera dir & up vectors with our new calculated ones
            camera.cameraUp = up;
        }
    }
}
