using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Drawing;
using System;
using System.Diagnostics;
using static OpenGL_Game.Managers.InputManager;
using OpenGL_Game.GameCode.Managers;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        public static float dt = 0;
        EntityManager entityManager; //Used to hold entities and manage them
        MazeScriptManager scriptManager; //Used to hot-load data
        OpenTKInputManager inputManager; //Used as a means of getting universal control responses from
                                   //a varied number of controllers

        SystemManager systemManager; //Manages and actions other systems
        SystemRender renderSystem; //Abstract system to render entities
        SystemPhysics physicsSystem; //System to apply motion & do collision detection & response

        public Camera camera;
        const float cameraVelocity = 2.7f;

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            scriptManager = new MazeScriptManager(); 
            inputManager = new OpenTKInputManager(sceneManager);
            systemManager = new SystemManager();
            physicsSystem = new SystemPhysics();
            renderSystem = new OpenGLRenderer();
            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.CursorVisible = false;
            sceneManager.CursorGrabbed = true;
            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(0, 1.06f, 0), new Vector3(0, 2.23f, 5), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);
            CreateEntities();
            CreateSystems();
        }

        private void CreateEntities()
        {
            const string SKYBOX_TEX_RELPATH = "GameCode/Geometry/Skybox/skybox.obj";

            Entity skyBox = new Entity("Skybox"); //Skybox needs to be rendered first, as Depth first is disabled for the draw
            skyBox.AddComponent(new ComponentTransform(0.0f, 0.0f, 0.0f));
            skyBox.AddComponent(new ComponentGeometry(SKYBOX_TEX_RELPATH, renderSystem));
            skyBox.AddComponent(new ComponentShaderBasic("GameCode/Shaders/vs.glsl", "GameCode/Shaders/fs.glsl"));
            entityManager.AddEntity(skyBox);

            scriptManager.LoadMaze("GameCode/map.xml", entityManager, renderSystem);
        }

        private void CreateSystems()
        {
            systemManager.AddRenderSystem(renderSystem, entityManager);
           //systemManager.AddNonRenderSystem(physicsSystem, entityManager);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            //System.Console.WriteLine("fps=" + (int)(1.0/dt));
            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();
            //Console.WriteLine(camera.cameraPosition);
            inputManager.Update(e);
            ProcessInput();

            //Action NON-RENDER systems
            systemManager.ActionNonRenderSystems();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Action RENDER systems
            systemManager.ActionRenderSystems();

            // Render score
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Score: 000", 18, StringAlignment.Near, Color.White);
            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            ResourceManager.RemoveAllAssets();
        }

        private void ProcessInput()
        {
            //Process any movement commands
            if (inputManager.IsActive("Forward"))
                camera.MoveForward(cameraVelocity * dt);
            if (inputManager.IsActive("Backward"))
                camera.MoveForward(-cameraVelocity * dt);
            if (inputManager.IsActive("Left"))
                camera.MoveRight(-cameraVelocity * dt);
            if (inputManager.IsActive("Right"))
                camera.MoveRight(cameraVelocity * dt);
            //Check if we need to change scene
            if (inputManager.IsActive("Continue"))
                sceneManager.ChangeScene(SceneType.GAME_OVER_SCENE);
            //Check if we need to exit
            if (inputManager.IsActive("Escape"))
                sceneManager.Exit();

            //Process mouse movement for the current frame
            inputManager.UpdateFPSCamera(ref camera, dt);
            Mouse.SetPosition((sceneManager.Bounds.Left + sceneManager.Bounds.Right) / 2, 
                            (sceneManager.Bounds.Top + sceneManager.Bounds.Bottom) / 2);
            camera.UpdateView();
        }
    }
}
