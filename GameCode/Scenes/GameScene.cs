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
using OpenGL_Game.GameEngine.Components.Physics;
using OpenTK.Audio.OpenAL;

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
        SystemAudio audioSystem;
        SystemPhysics physicsSystem; //System to apply motion & do collision detection & response

        public Camera camera;
        Entity footstepSource;
        ComponentTransform footstepTransform;
        const float cameraVelocity = 2.0f; //2.0f
        private bool walking = false, walkingUp = false, walkingDown = true;
        private float walkingVelocity = 0.18f; // 0.18f

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            scriptManager = new MazeScriptManager(); 
            inputManager = new OpenTKInputManager(sceneManager);
            systemManager = new SystemManager();
            physicsSystem = new SystemPhysics();
            audioSystem = new SystemAudio();
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

            footstepSource = new Entity("FootstepsSfx");
            footstepTransform = new ComponentTransform(camera.cameraPosition - new Vector3(0.0f, -0.9f, 0.0f));
            footstepSource.AddComponent(footstepTransform);
            footstepSource.AddComponent(new ComponentVelocity(new Vector3(0.0f, 0.3f, 0.0f)));
            footstepSource.AddComponent(new ComponentAudio("GameCode\\Audio\\footsteps.wav"));
            entityManager.AddEntity(footstepSource);

            scriptManager.LoadMaze("GameCode/map.xml", entityManager, renderSystem);
        }

        private void CreateSystems()
        {
            systemManager.AddRenderSystem(renderSystem, entityManager);
            systemManager.AddNonRenderSystem(physicsSystem, entityManager);
            systemManager.AddNonRenderSystem(audioSystem, entityManager);
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

            if (walking)
            {
                footstepTransform.Position = camera.cameraPosition;
                if (walkingUp)
                {
                    camera.cameraPosition.Y += walkingVelocity * dt;
                    walkingUp = camera.cameraPosition.Y < 1.06f;
                    walkingDown = camera.cameraPosition.Y > 1.06f;
                }
                else if (walkingDown)
                {
                    camera.cameraPosition.Y -= walkingVelocity * dt;
                    walkingUp = camera.cameraPosition.Y < 1.0f;
                    walkingDown = camera.cameraPosition.Y > 1.0f;
                    if (walkingUp)
                        audioSystem.PlaySound(footstepSource);
                }
            }

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
            Vector2 movementVec = new Vector2(0,0);
            //Process any movement commands
            if (inputManager.IsActive("Forward"))
                movementVec.X = cameraVelocity * dt;
            if (inputManager.IsActive("Backward"))
                movementVec.X = -cameraVelocity * dt;
            if (inputManager.IsActive("Left"))
                movementVec.Y = -cameraVelocity * dt;
            if (inputManager.IsActive("Right"))
                movementVec.Y = cameraVelocity * dt;

            if (movementVec.Length == 0 && walking)
            {
                camera.cameraPosition.Y = 1.06f;
                walking = false; //Walking is used for Visual effect of 'head-bob'
            }
            else if (movementVec.Length > 0 && !walking)
                walking = true;
            camera.MoveForward(movementVec.X);
            camera.MoveRight(movementVec.Y);

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
