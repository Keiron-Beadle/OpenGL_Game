﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Managers
{
    public enum SceneType
    {
        NULL_SCENE,
        GAME_SCENE,
        MAIN_MENU_SCENE,
        GAME_OVER_SCENE,
    }

    class SceneManager : GameWindow
    {
        Scene scene;
        public static int width = 1600, height = 1200;
        public static int windowXPos = 200, windowYPos = 80;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            this.X = windowXPos;
            this.Y = windowYPos;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //Load the GUI
            GUI.SetUpGUI(width, height);

            StartMenu();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Focused)
                updater(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            renderer(e);

            GL.Flush();
            SwapBuffers();
        }

        public void ChangeScene(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.GAME_SCENE:
                    StartNewGame();
                    break;
                case SceneType.MAIN_MENU_SCENE:
                    StartMenu();
                    break;
                case SceneType.GAME_OVER_SCENE:
                    StartGameOver();
                    break;
                case SceneType.NULL_SCENE:
                    break;
            }
        }

        private void StartGameOver()
        {
            if (scene != null) scene.Close();
            scene = new GameOverScene(this);
        }

        private void StartNewGame()
        {
            if(scene != null) scene.Close();
            scene = new GameScene(this);
        }

        private void StartMenu()
        {
            if (scene != null) scene.Close();
            scene = new MainMenuScene(this);
        }

        public static int WindowWidth
        {
            get { return width; }
        }

        public static int WindowHeight
        {
            get { return height; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            SceneManager.width = Width;
            SceneManager.height = Height;

            //Load the GUI
            GUI.SetUpGUI(Width, Height);
        }
    }

}

