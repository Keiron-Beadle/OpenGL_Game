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
using OpenGL_Game.GameCode.Components.Controllers;
using OpenGL_Game.GameEngine.Colliders;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        public static float dt = 0;
        public static Vector3 WorldTranslate = Vector3.Zero;
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

        ComponentAIController ballController;
        ComponentAIController droneController;
        ComponentAIController bouncingController;

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
            //player.AddComponent(new ComponentTransform(new Vector3(7f, 1.06f, 2f)));

            Vector3 playerPos = new Vector3(-8.0f, 1.06f, 7.92f);
            player.AddComponent(new ComponentTransform(playerPos));
            playerCamera = new ComponentCamera(player, new Vector3(0, 2.23f, 5), 
                (float)sceneManager.Width / (float)sceneManager.Height, 0.1f, 100f);
            player.AddComponent(playerCamera);
            player.AddComponent(new ComponentVelocity(Vector3.Zero));
            player.AddComponent(new ComponentAudio("GameCode\\Audio\\footsteps.wav", playerCamera, player));
            playerController = new ComponentPlayerController(sceneManager, inputManager, player);
            player.AddComponent(playerController);
            ComponentSphereCollider playerCollider = new ComponentSphereCollider(player, Vector3.Zero, 0.14f);
            playerCollider.AddCollider(new SphereCollider(Vector3.Zero, 0.14f, new Vector3(0,-1.06f,0)));
            player.AddComponent(playerCollider); //Head collider for drone
            //player.AddComponent(new ComponentSphereCollider(player, Vector3.Zero, 0.14f, new Vector3(0.0f,-1.06f,0.0f))); //Foot collider for rolling ball
            entityManager.AddEntity(player);

            Entity drone = new Entity("Drone", TAGS.ENEMY);
            drone.AddComponent(new ComponentTransform(new Vector3(-2.5f, 1.0f, 1.5f), new Vector3(0.1f), Vector3.Zero));
            drone.AddComponent(new ComponentGeometry("GameCode\\Geometry\\Drone\\Drone.obj", renderSystem));
            drone.AddComponent(new ComponentVelocity(Vector3.Zero));
            drone.AddComponent(new ComponentAudio("GameCode\\Audio\\buzz.wav", playerCamera, drone));
            drone.AddComponent(new ComponentShaderPointLight("GameCode\\Shaders\\vsPointLight.glsl", "GameCode\\Shaders\\fsPointLight.glsl"));
            droneController = new ComponentDroneController(drone, player, "GameCode\\graphMap.txt");
            drone.AddComponent(droneController);
            drone.AddComponent(new ComponentBoxCollider(drone));
            entityManager.AddEntity(drone);

            Entity rollingBall = new Entity("RollingBall", TAGS.ENEMY);
            rollingBall.AddComponent(new ComponentTransform(new Vector3(-8.21f, 0.14f, -6.38f)));
            rollingBall.AddComponent(new ComponentGeometry("GameCode\\Geometry\\Ball\\Ball.obj", renderSystem));
            rollingBall.AddComponent(new ComponentVelocity(Vector3.Zero));
            rollingBall.AddComponent(new ComponentShaderPointLight("GameCode\\Shaders\\vsPointLight.glsl", "GameCode\\Shaders\\fsPointLight.glsl"));
            ballController = new ComponentRollingController(rollingBall, new Vector2(-4.412f, -4.819f), new Vector2(-11.69f, -8.71f));
            rollingBall.AddComponent(ballController);
            rollingBall.AddComponent(new ComponentSphereCollider(rollingBall));
            entityManager.AddEntity(rollingBall);

            Entity bouncingBall = new Entity("BouncingBall", TAGS.ENEMY);
            bouncingBall.AddComponent(new ComponentTransform(new Vector3(-10.15f, 1.6f, 8.23f)));
            bouncingBall.AddComponent(new ComponentGeometry("GameCode\\Geometry\\Ball\\Ball.obj", renderSystem));
            bouncingBall.AddComponent(new ComponentVelocity(Vector3.Zero));
            bouncingBall.AddComponent(new ComponentShaderPointLight("GameCode\\Shaders\\vsPointLight.glsl", "GameCode\\Shaders\\fsPointLight.glsl"));
            bouncingController = new ComponentBouncingController(bouncingBall, new Vector2(1.6f, 8.2583f), new Vector2(0.1f, 3.73f));
            bouncingBall.AddComponent(bouncingController);
            bouncingBall.AddComponent(new ComponentSphereCollider(bouncingBall));
            entityManager.AddEntity(bouncingBall);

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
            ballController.Update(audioSystem, dt);
            bouncingController.Update(audioSystem, dt);
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
