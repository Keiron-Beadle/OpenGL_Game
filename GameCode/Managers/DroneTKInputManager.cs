using OpenGL_Game.Managers;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Managers
{
    class DroneTKInputManager : OpenTKInputManager
    {
        public DroneTKInputManager(SceneManager pSceneManager) : base(pSceneManager)
        {
            KeyboardPoller = UpdateKeyboard;
            MousePoller = UpdateMouse;
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

                if (pair.Value == "StopCollision")
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
    }
}
