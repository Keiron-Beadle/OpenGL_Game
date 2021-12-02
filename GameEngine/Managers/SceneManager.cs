using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Scenes;
using System.Collections.Generic;
using OpenGL_Game.GameCode.Scenes;

namespace OpenGL_Game.Managers
{
    class SceneManager : GameWindow
    {
        public static int width = 1600, height = 1200;
        public static int windowXPos = 200, windowYPos = 80;
        Stack<Scene> scenes;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            scenes = new Stack<Scene>();
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
            //GUI.SetUpGUI(width, height);

            ChangeScene(SceneType.MAIN_MENU_SCENE);
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
                    GUI.UnloadImage();
                    StartNewGame();
                    break;
                case SceneType.MAIN_MENU_SCENE:
                    GUI.LoadImage("GameCode\\Resources\\MainMenuImage.jpg");
                    StartMenu();
                    break;
                case SceneType.GAME_OVER_SCENE:
                    GUI.LoadImage("GameCode\\Resources\\GameOverImage.jpg");
                    StartGameOver();
                    break;
                case SceneType.OPTIONS_SCENE:
                    GUI.LoadImage("GameCode\\Resources\\OptionsImage.jpg");
                    StartOptions();
                    break;
                case SceneType.NULL_SCENE:
                    break;
            }
        }

        public void PopScene()
        {
            scenes.Pop();
        }

        private void StartOptions()
        {
            scenes.Push(new OptionsScene(this));
        }

        private void StartGameOver()
        {
            scenes.Pop().Close();
            scenes.Push(new GameOverScene(this));
        }

        private void StartNewGame()
        {
            scenes.Push(new GameScene(this));
        }

        private void StartMenu()
        {
            if (scenes.Count == 0) //Only want 1 Main menu scene at the bottom of stack
            {
                scenes.Push(new MainMenuScene(this));
                return;
            }
            scenes.Pop().Close();
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

