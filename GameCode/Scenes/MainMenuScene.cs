﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using OpenGL_Game.Managers;
using OpenGL_Game.GameCode.Managers;

namespace OpenGL_Game.Scenes
{
    class MainMenuScene : Scene
    {
        DroneTKInputManager inputManager;

        public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
        {
            inputManager = new DroneTKInputManager(sceneManager);
            // Set the title of the window
            sceneManager.Title = "Main Menu";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            sceneManager.CursorVisible = true;
            sceneManager.CursorGrabbed = false;
        }

        public override void Update(FrameEventArgs e)
        {
            inputManager.Update(e);
            if (inputManager.LeftClicked) 
            {
                sceneManager.ChangeScene(SceneType.GAME_SCENE);
            }
            else if (inputManager.controlFlags["Continue"])
            {
                sceneManager.ChangeScene(SceneType.OPTIONS_SCENE);
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
            
        }
    }
}