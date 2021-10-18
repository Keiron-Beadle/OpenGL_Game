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
        private bool[] controlFlags; //Array to hold which keys are pressed currently
        private Dictionary<Key, CONTROLS> controlBindings;

        //Mouse variables
        private Vector2 centerOfWindow;
        private MouseState prevMouseState;
        private bool mouseLeftClick = false; //Bool for if left mouse button clicked right now
        private bool firstRun = true;
        float mouseHAngle = 0.0f;
        float mouseVAngle = 0.0f;
        const float SENSITIVITY = 0.14f;

        //Properties
        public Vector2 DeltaMouse { get; private set; }
        public bool[] ControlFlags { get { return controlFlags; } }
        public bool LeftClicked { get { return mouseLeftClick; } }
        public bool AnyKeyPressed { get; private set; }


        public OpenTKInputManager(SceneManager sceneManger) : base(sceneManger)
        {
            centerOfWindow = new Vector2((sceneManager.Bounds.Left + sceneManager.Bounds.Right) / 2,
                (sceneManager.Bounds.Top + sceneManager.Bounds.Bottom) / 2);
            controlFlags = new bool[Enum.GetNames(typeof(CONTROLS)).Length];
            controlBindings = new Dictionary<Key, CONTROLS>();
            DeltaMouse = new Vector2(0, 0);
            //GenerateControls();
            LoadControls();
        }

        ~OpenTKInputManager()
        {
            SaveControls();
        }

        public override void Update(FrameEventArgs e)
        {
            UpdateKeyboard();
            UpdateMouse();
        }

        protected override void SaveControls()
        {
            ScriptManager.SaveTKControls(ref controlBindings);
        }

        protected override void LoadControls()
        {
            ScriptManager.LoadTKControls(ref controlBindings);
        }

        //private void GenerateControls()
        //{
        //    Key w = Key.W;
        //    Key a = Key.A;
        //    Key s = Key.S;
        //    Key d = Key.D;
        //    Key up = Key.Up;
        //    Key down = Key.Down;
        //    Key right = Key.Right;
        //    Key left = Key.Left;

        //    CONTROLS forward = CONTROLS.Forward;
        //    CONTROLS backward = CONTROLS.Backward;
        //    CONTROLS leftC = CONTROLS.Left;
        //    CONTROLS rightC = CONTROLS.Right;

        //    controlBindings.Add(w, forward);
        //    controlBindings.Add(s, backward);
        //    controlBindings.Add(a, leftC);
        //    controlBindings.Add(d, rightC);
        //    controlBindings.Add(up, forward);
        //    controlBindings.Add(down, backward);
        //    controlBindings.Add(left, leftC);
        //    controlBindings.Add(right, rightC);

        //    SaveControls();
        //    Environment.Exit(0);
        //}

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

            if (firstRun)
            {
                DeltaMouse = Vector2.Zero;
                firstRun = false;
                return;
            }

            prevMouseState = currentMouseState;
        }

        private void UpdateKeyboard()
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            for (int i = 0; i < controlFlags.Length; i++) { controlFlags[i] = false; }
            foreach (var pair in controlBindings)
            {
                if (currentKeyState.IsKeyDown(pair.Key))
                    controlFlags[(int)pair.Value] = true;
            }

            AnyKeyPressed = currentKeyState.IsAnyKeyDown && !prevKeyState.IsAnyKeyDown ? true : false;

            prevKeyState = currentKeyState;
        }

        public override void UpdateFPSCamera(ref Camera camera, float dt)
        {
            mouseHAngle += (-SENSITIVITY * DeltaMouse.X) * dt; //Append a new delta mouse change
            mouseVAngle += (-SENSITIVITY * DeltaMouse.Y) * dt;
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
