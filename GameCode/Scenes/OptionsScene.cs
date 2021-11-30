using OpenGL_Game.GameCode.Managers;
using OpenGL_Game.Managers;
using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Scenes
{
    class OptionsScene : Scene
    {
        DroneTKInputManager inputManager;

        public OptionsScene(SceneManager pSceneManager): base(pSceneManager)
        {
            inputManager = new DroneTKInputManager(sceneManager);
            // Set the title of the window
            sceneManager.Title = "Options";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer += Render;
            sceneManager.updater += Update;
            sceneManager.CursorVisible = true;
            sceneManager.CursorGrabbed = false;
        }

        public override void Update(FrameEventArgs e)
        {
            inputManager.Update(e);
            if (inputManager.controlFlags["Escape"])
            {
                Close();
            }
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.CornflowerBlue;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Background();
            //GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "Main Menu", (int)fontSize, StringAlignment.Center);
            GUI.Render();
        }

        public override void Close()
        {
            sceneManager.updater -= Update;
            sceneManager.renderer -= Render;
            sceneManager.ChangeScene(SceneType.MAIN_MENU_SCENE);
        }
    }
}
