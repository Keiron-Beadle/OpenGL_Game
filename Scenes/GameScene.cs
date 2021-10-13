﻿using OpenTK;
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

        //temp input control
        float mouseHAngle = 0.0f;
        float mouseVAngle = 0.0f;
        Vector2 prevMouse;
        const float mouseSpd = 0.5f;
        bool firstRun = true;

        //Temp variables
        Entity skyBox;

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
            sceneManager.mouseMoveDelegate += OnMouseMove;
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
            Entity newEntity, starshipEntity, intergalacticShip;
            const string STARSHIP_OBJ_RELPATH = "Geometry/Wraith_Raider_Starship/Wraith_Raider_Starship.obj";
            const string SKYBOX_TEX_RELPATH = "Geometry/Skybox/skybox.obj";

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
            starshipEntity.AddComponent(new ComponentTransform(new Vector3(2.5f,0.0f,0.0f), Vector3.One, new Vector3(3.14f, 0.0f, 0.0f)));
            starshipEntity.AddComponent(new ComponentGeometry(STARSHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(starshipEntity);

            //Excercise 2 - Add intergalactic Starship
            intergalacticShip = new Entity("Intergalactic Ship");
            intergalacticShip.AddComponent(new ComponentTransform(0.4f, 0.0f, 0.0f));
            intergalacticShip.AddComponent(new ComponentGeometry(STARSHIP_OBJ_RELPATH, renderSystem));
            entityManager.AddEntity(intergalacticShip);
        }

        private void CreateSystems()
        {
            //ISystem newSystem; //For future systems

            systemManager.AddSystem(renderSystem);
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
                case Key.W:
                    camera.MoveForward(0.1f);
                    break;
                case Key.S:
                case Key.Down:
                    camera.MoveForward(-0.1f);
                    break;
                //case Key.Left:
                //    camera.RotateY(-0.01f);
                //    break;
                //case Key.Right:
                //    camera.RotateY(0.01f);
                //    break;
                case Key.M:
                    sceneManager.StartMenu();
                    break;
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            UpdateCameraLookAt(); //Update player camera movement
            Mouse.SetPosition(sceneManager.Width / 2, sceneManager.Height / 2);
        }
        
    }
}