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
        InputManager inputManager;
        ISystem renderSystem;

        //temp input control
        float mouseHAngle = 0.0f;
        float mouseVAngle = 0.0f;
        Vector2 prevMouse;
        const float mouseSpd = 0.5f;
        bool firstRun = true;

        //Temp variables
        Entity skyBox;
        const float cameraVelocity = 0.6f;

        public Camera camera;

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            inputManager = new InputManager();
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
            camera = new Camera(new Vector3(0, 4, 7), new Vector3(0, 0, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);
            prevMouse = new Vector2(0, 0);
            //Mouse.SetPosition(sceneManager.Width / 2, sceneManager.Height / 2);
            CreateSystems();
            CreateEntities();
        }

        private void CreateEntities()
        {
            Entity newEntity, starshipEntity, intergalacticShip, sussybaka;
            const string STARSHIP_OBJ_RELPATH = "Geometry/Wraith_Raider_Starship/Wraith_Raider_Starship.obj";
            const string INTERGALACTIC_SHIP_OBJ_RELPATH = "Geometry/Intergalactic_Ship/Intergalactic_Spaceship.obj";
            const string SKYBOX_TEX_RELPATH = "Geometry/Skybox/skybox.obj";
            const string SUSSY_OBJ_RELPATH = "Geometry/Amogus/amogus.obj";

            skyBox = new Entity("Skybox"); //Skybox needs to be rendered first, as Depth first is disabled for the draw
            skyBox.AddComponent(new ComponentTransform(0.0f, 0.0f, 0.0f));
            skyBox.AddComponent(new ComponentGeometry(SKYBOX_TEX_RELPATH, renderSystem));
            entityManager.AddEntity(skyBox);

            newEntity = new Entity("Moon");
            newEntity.AddComponent(new ComponentTransform(-2.5f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj", renderSystem));
            entityManager.AddEntity(newEntity);

            //Excercise 1 - Add raider starship
            starshipEntity = new Entity("Wraith_Raider_Starship");
            starshipEntity.AddComponent(new ComponentTransform(new Vector3(2.5f,0.0f,0.0f), Vector3.One, new Vector3(0f, 0.0f, 0.0f)));
            starshipEntity.AddComponent(new ComponentGeometry(STARSHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(starshipEntity);

            //Excercise 2 - Add intergalactic Starship
            intergalacticShip = new Entity("Intergalactic Ship");
            intergalacticShip.AddComponent(new ComponentTransform(new Vector3(0.4f, 0.0f, 0.0f), new Vector3(0.2f,0.2f,0.2f), Vector3.Zero));
            intergalacticShip.AddComponent(new ComponentGeometry(INTERGALACTIC_SHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(intergalacticShip);

            //Exercise 3 - Add custom model, it's kinda sus
            sussybaka = new Entity("Sus man");
            sussybaka.AddComponent(new ComponentTransform(new Vector3(0.0f, 0.6f, -2.0f), new Vector3(0.5f,0.5f,0.5f), Vector3.Zero));
            sussybaka.AddComponent(new ComponentGeometry(SUSSY_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(sussybaka);
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
            Console.WriteLine(camera.cameraPosition);
            inputManager.Update(e);
            ProcessInput();

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
        }

        private void ProcessInput()
        {
            if (inputManager.ControlFlags[(int)CONTROLS.Forward])
                camera.MoveForward(cameraVelocity * dt);
            if (inputManager.ControlFlags[(int)CONTROLS.Backward])
                camera.MoveForward(-cameraVelocity * dt);
            if (inputManager.ControlFlags[(int)CONTROLS.Left])
                camera.MoveRight(-cameraVelocity * dt);
            if (inputManager.ControlFlags[(int)CONTROLS.Right])
                camera.MoveRight(cameraVelocity * dt);

            if (inputManager.ControlFlags[(int)CONTROLS.Escape])
                sceneManager.Exit();
        
        }

        /// <summary>
        /// Method is called in the OnMouseMove callback
        /// </summary>
        private void UpdateCameraLookAt()
        {
            float xPos = Mouse.GetState().X; //Get current mouse data
            float yPos = Mouse.GetState().Y;
            if (firstRun) { prevMouse = new Vector2(xPos, yPos); firstRun = false; return; }
            float deltaX = xPos - prevMouse.X;
            float deltaY = yPos - prevMouse.Y;
            mouseHAngle += -mouseSpd * dt * deltaX; //Append a new delta mouse change
            mouseVAngle += -mouseSpd * dt * deltaY;
            mouseVAngle = MathHelper.Clamp(mouseVAngle, -1.4f, 1.4f); //Clamp vertical so we can't look upside down
            Vector3 dir = new Vector3((float)Math.Cos(mouseVAngle) * (float)Math.Sin(mouseHAngle),
                                       (float)Math.Sin(mouseVAngle),
                                       (float)Math.Cos(mouseVAngle) * (float)Math.Cos(mouseHAngle));
            Vector3 right = new Vector3(
                                (float)Math.Sin(mouseHAngle - MathHelper.PiOver2),
                                 0.0f,
                                 (float)Math.Cos(mouseHAngle - MathHelper.PiOver2));
            Vector3 up = Vector3.Cross(right, dir);
            camera.view = Matrix4.LookAt(camera.cameraPosition, camera.cameraPosition + dir, up);
            camera.cameraDirection = dir; //Update camera dir & up vectors with our new calculated ones
            camera.cameraUp = up;
            camera.UpdateView(); //Update the view

            prevMouse = new Vector2(xPos, yPos); //Set the prevmouse vector to be the current mouse vector at the end
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            UpdateCameraLookAt(); //Update player camera movement
            Mouse.SetPosition(sceneManager.Width / 2, sceneManager.Height / 2);
        }
        
    }
}
