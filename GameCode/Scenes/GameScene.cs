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
using OpenGL_Game.GameEngine.Systems;
using OpenGL_Game.GameEngine.Components.Render;
using OpenGL_Game.GameCode.Components;

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
        SystemPhysics physicsSystem; //System to apply motion & rotation
        SystemCollision collisionSystem;
        CollisionManager collisionManager;

        ComponentPlayerController playerController;
        public ComponentCamera playerCamera; //Static camera manager in future to access this

        ComponentAIController droneController;

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
            collisionSystem = new SystemCollision();
            collisionManager = new CollisionManager(collisionSystem);
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

            Entity player = new Entity("Player", TAGS.PLAYER);
            ComponentTransform playerTransform = new ComponentTransform(new Vector3(0, 1.06f, 0));
            player.AddComponent(playerTransform);
            playerCamera = new ComponentCamera(player, new Vector3(0, 2.23f, 5), 
                (float)sceneManager.Width / (float)sceneManager.Height, 0.1f, 100f);
            player.AddComponent(playerCamera);
            player.AddComponent(new ComponentVelocity(Vector3.Zero));
            player.AddComponent(new ComponentAudio("GameCode\\Audio\\footsteps.wav", playerCamera, playerTransform));
            playerController = new ComponentPlayerController(sceneManager, inputManager, player);
            player.AddComponent(playerController);
            player.AddComponent(new ComponentSphereCollider(player, Vector3.Zero, 0.14f));
            entityManager.AddEntity(player);

            Entity drone = new Entity("Drone", TAGS.ENEMY);
            drone.AddComponent(new ComponentTransform(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.1f), Vector3.Zero));
            drone.AddComponent(new ComponentGeometry("GameCode\\Geometry\\Drone\\Drone.obj", renderSystem));
            drone.AddComponent(new ComponentShaderPointLight("GameCode\\Shaders\\vsPointLight.glsl", "GameCode\\Shaders\\fsPointLight.glsl"));
            droneController = new ComponentAIController(drone, player);
            drone.AddComponent(droneController);
            drone.AddComponent(new ComponentBoxCollider(drone));
            entityManager.AddEntity(drone);

            scriptManager.LoadMaze("GameCode/map.xml", entityManager, renderSystem);
        }

        private void CreateSystems()
        {
            systemManager.AddRenderSystem(renderSystem, entityManager);
            systemManager.AddNonRenderSystem(physicsSystem, entityManager);
            systemManager.AddNonRenderSystem(audioSystem, entityManager);
            systemManager.AddNonRenderSystem(collisionSystem, entityManager);
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

            //Console.WriteLine(playerCamera.cameraPosition);
            inputManager.Update(e);
            ProcessInput();

            //Action NON-RENDER systems
            systemManager.ActionNonRenderSystems();
            collisionManager.Update();
        }

        private void ProcessInput()
        {
            //Check if we need to change scene
            if (inputManager.IsActive("Continue"))
                sceneManager.ChangeScene(SceneType.GAME_OVER_SCENE);
            //Check if we need to exit
            if (inputManager.IsActive("Escape"))
                sceneManager.Exit();

            playerController.Update(audioSystem, dt); //Update the player with the newly updating input
            droneController.Update(audioSystem, dt);
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
    }
}
