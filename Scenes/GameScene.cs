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

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        public static float dt = 0;
        EntityManager entityManager;
        SystemManager systemManager;
        ISystem renderSystem;

        public Camera camera;

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            renderSystem = new OpenGLRenderer();
            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;

            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(0, 4, 7), new Vector3(0, 0, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);

            CreateSystems();
            CreateEntities();
        }

        private void CreateEntities()
        {
            Entity newEntity, starshipEntity, intergalacticShip;
            const string STARSHIP_OBJ_RELPATH = "Geometry/Wraith_Raider_Starship/Wraith_Raider_Starship.obj";
            newEntity = new Entity("Moon");
            newEntity.AddComponent(new ComponentPosition(-2.5f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj", renderSystem));
            entityManager.AddEntity(newEntity);

            //Excercise 1 - Add raider starship
            starshipEntity = new Entity("Wraith_Raider_Starship");
            starshipEntity.AddComponent(new ComponentPosition(2.5f, 0.0f, 0.0f));
            starshipEntity.AddComponent(new ComponentGeometry(STARSHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(starshipEntity);

            //Excercise 2 - Add intergalactic Starship
            intergalacticShip = new Entity("Intergalactic Ship");
            intergalacticShip.AddComponent(new ComponentPosition(0.4f, 0.0f, 0.0f));
            intergalacticShip.AddComponent(new ComponentGeometry(STARSHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(intergalacticShip);
        }

        private void CreateSystems()
        {
            //ISystem newSystem; //For future systems

            systemManager.AddSystem(renderSystem);
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

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Action ALL systems
            systemManager.ActionSystems(entityManager);

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
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
        }

        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    camera.MoveForward(0.1f);
                    break;
                case Key.Down:
                    camera.MoveForward(-0.1f);
                    break;
                case Key.Left:
                    camera.RotateY(-0.01f);
                    break;
                case Key.Right:
                    camera.RotateY(0.01f);
                    break;
                case Key.M:
                    sceneManager.StartMenu();
                    break;
            }
        }
    }
}
