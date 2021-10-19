using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.GameEngine.Components.Render;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Systems;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components
{
    class ComponentPlayerController : ComponentController
    {
        private SceneManager sceneManager;
        private InputManager inputManager;
        private ComponentCamera camera;
        private ComponentAudio footstepAudio;
        private float mouseHAngle = 0.0f;
        private float mouseVAngle = 0.0f;
        static float MOUSE_SENSITIVITY = 0.14f;

        const float cameraVelocity = 2.0f; //2.0f
        private bool walking = false, walkingUp = false, walkingDown = true;
        private float walkingVelocity = 0.18f; // 0.18f

        public ComponentPlayerController(SceneManager pSceneManager, InputManager pInputManager, Entity player)
        {
            sceneManager = pSceneManager;
            inputManager = pInputManager;
            camera = player.FindComponentByType(ComponentTypes.COMPONENT_CAMERA) as ComponentCamera;
            transform = player.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            footstepAudio = player.FindComponentByType(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
        }

        public void Update(SystemAudio audioSystem, float dt)
        {
            if (walking)
            {
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
                        audioSystem.PlaySound(footstepAudio);
                }
            }
            CheckForInputFlags(dt);
        }

        private void CheckForInputFlags(float dt)
        {
            Vector2 movementVec = new Vector2(0, 0);
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
            UpdateView(dt);
            Mouse.SetPosition((sceneManager.Bounds.Left + sceneManager.Bounds.Right) / 2,
                            (sceneManager.Bounds.Top + sceneManager.Bounds.Bottom) / 2);
            camera.UpdateView();
        }

        protected override void UpdateView(float dt)
        {
            mouseHAngle += (-MOUSE_SENSITIVITY * inputManager.DeltaAxis.X) * dt; //Append a new delta mouse change
            mouseVAngle += (-MOUSE_SENSITIVITY * inputManager.DeltaAxis.Y) * dt;
            mouseVAngle = MathHelper.Clamp(mouseVAngle, -1.4f, 1.4f); //Clamp vertical so we can't look upside down
            Vector3 dir = new Vector3((float)Math.Cos(mouseVAngle) * (float)Math.Sin(mouseHAngle),
                                       (float)Math.Sin(mouseVAngle),
                                       (float)Math.Cos(mouseVAngle) * (float)Math.Cos(mouseHAngle));
            Vector3 right = new Vector3(
                                (float)Math.Sin(mouseHAngle - MathHelper.PiOver2),
                                 0.0f,
                                 (float)Math.Cos(mouseHAngle - MathHelper.PiOver2));
            Vector3 up = Vector3.Cross(right, dir);
            camera.cameraDirection = dir; //Update camera dir & up vectors with our new calculated ones
            camera.cameraUp = up;
        }
    }
}
