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
using OpenGL_Game.GameEngine.Managers;
using OpenGL_Game.OBJLoader;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        public static GameScene gameInstance;

        EntityManager entityManager; //Used to hold entities and manage them
        MazeScriptManager scriptManager; //Used to hot-load data
        OpenTKInputManager inputManager; //Used as a means of getting universal control responses from
                                         //a varied number of controllers
        CollisionManager collisionManager;
        SystemManager systemManager; //Manages and actions other systems
        SystemRender renderSystem; //Abstract system to render entities
        SystemAudio audioSystem;
        SystemPhysics physicsSystem; //System to apply motion & rotation
        SystemCollision collisionSystem;

        public static float dt = 0;
        public static Vector3 WorldTranslate = Vector3.Zero;

        public int PlayerLives = 3;
        public int KeysCollected = 0;
        public int PortalOnlineBuffer;
        private bool swappedPortalAudio = false;
        private bool swappedPortalColour = false;
        public bool GameOver = false;
        private ITexture heartTex, keyTex;

        public ControllerManager controllerManager;
        public ComponentCamera playerCamera; //Static camera manager in future to access this

        private ComponentAudio portalAudio;

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
            collisionManager = new CollisionManager(collisionSystem, entityManager, systemManager, audioSystem);
            controllerManager = new ControllerManager(audioSystem);
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
            const string SKYBOX_TEX_RELPATH = "GameCode\\Geometry\\Skybox\\skybox.obj";

            Entity skyBox = new Entity("Skybox"); //Skybox needs to be rendered first, as Depth first is disabled for the draw
            skyBox.AddComponent(new ComponentTransform(0.0f, 0.0f, 0.0f));
            skyBox.AddComponent(new ComponentGeometry(SKYBOX_TEX_RELPATH, renderSystem));
            skyBox.AddComponent(new ComponentShaderBasic("GameCode\\Shaders\\vs.glsl", "GameCode\\Shaders\\fs.glsl"));
            entityManager.AddEntity(skyBox);

            scriptManager.LoadMaze("GameCode\\map.xml", entityManager, sceneManager, renderSystem, inputManager);

            PortalOnlineBuffer = ResourceManager.LoadAudioBuffer("GameCode\\Audio\\portalonline.wav");
            Entity portal = entityManager.FindEntity("Portal");
            if (portal == null) { return; }
            portalAudio = portal.FindComponentByType(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
            ComponentTransform portalTransform = portal.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            portalTransform.Position = new Vector3(portalTransform.Position.X, 0.0f, portalTransform.Position.Z);
            audioSystem.PlaySound(portalAudio, true);

            heartTex = ResourceManager.LoadTexture("GameCode\\Icons\\heart.png", renderSystem);
            keyTex = ResourceManager.LoadTexture("GameCode\\Icons\\key.png", renderSystem);
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

            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            DoGameLogic();
            inputManager.Update(e);
            ProcessInput();

            //Action NON-RENDER systems
            systemManager.ActionNonRenderSystems();
            LightManager.Update(dt);
            if (!inputManager.StopCollision)
            {
                collisionManager.Update();
            }
        }

        private void DoGameLogic()
        {
            if (KeysCollected == 3 && !swappedPortalAudio)
            {
                audioSystem.ReplaceSound(portalAudio, PortalOnlineBuffer);
                swappedPortalAudio = true;
            }
            if (KeysCollected == 3 && !swappedPortalColour)
            {
                Entity portal = entityManager.FindEntity("Portal");
                ComponentGeometry geometry = portal.FindComponentByType(ComponentTypes.COMPONENT_GEOMETRY) as ComponentGeometry;
                geometry.SetDiffuse(1.0f, new Vector3((20.0f / 255), (219.0f / 255), (83.0f / 255)), renderSystem);
                swappedPortalColour = true;
            }
            if (PlayerLives <= 0)
            {
                sceneManager.ChangeScene(SceneType.GAME_OVER_SCENE);
            }
            if (GameOver)
            {
                sceneManager.ChangeScene(SceneType.GAME_OVER_SCENE);
            }
        }

        private void ProcessInput()
        {
            //Check if we need to exit
            if (inputManager.IsActive("Escape"))
                sceneManager.Exit();

            controllerManager.Update(dt);
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
            GUI.Label(new Rectangle(10, 50, (int)width, (int)(fontSize * 2f)), "Keys: ", 18, StringAlignment.Near, Color.White);
            //GUI.DrawIcon(new Rectangle(70+(50), sceneManager.Height - 100, 50, 50), heartTex);
            for (int i = 0; i < KeysCollected; i++)
            {
                GUI.DrawIcon(new Rectangle(80 + (55 * i), sceneManager.Height - 95, 50, 50), keyTex);
            }
            GUI.Label(new Rectangle(10, 150, (int)width, (int)(fontSize * 2f)), "Lives: ", 18, StringAlignment.Near, Color.White);
            for (int i = 0; i < PlayerLives; i++)
            {
                GUI.DrawIcon(new Rectangle(80 + (55 * i), sceneManager.Height - 200, 50, 50), heartTex);
            }
            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            audioSystem.StopAllSounds();
            ResourceManager.RemoveAllAssets();
        }
    }
}
