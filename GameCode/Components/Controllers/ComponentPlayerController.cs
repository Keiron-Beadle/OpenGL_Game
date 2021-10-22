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

namespace OpenGL_Game.GameCode.Components.Controllers
{
    class ComponentPlayerController : ComponentController, IControllerWithView
    {
        private SceneManager sceneManager;
        private InputManager inputManager;
        private ComponentCamera camera;
        private ComponentAudio footstepAudio;
        private ComponentVelocity velocity;
        private float mouseHAngle = 0.0f;
        private float mouseVAngle = 0.0f;
        static float MOUSE_SENSITIVITY = 0.14f;

        const float cameraVelocity = 2.0f; //2.0f
        private bool walking = false, walkingUp = false, walkingDown = true;
        private float walkingVelocity = 0.13f; // 0.18f

        public ComponentPlayerController(SceneManager pSceneManager, InputManager pInputManager, Entity player)
        {
            sceneManager = pSceneManager;
            inputManager = pInputManager;
            camera = player.FindComponentByType(ComponentTypes.COMPONENT_CAMERA) as ComponentCamera;
            transform = player.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            footstepAudio = player.FindComponentByType(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
            velocity = player.FindComponentByType(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;
            originalPosition = transform.Position;
        }

        public override void ResetPosition()
        {
            base.ResetPosition();
            camera.cameraPosition = originalPosition;
            velocity.Velocity = Vector3.Zero;
            mouseHAngle = 0.0f;
            mouseVAngle = 0.0f;
        }

        public override void Update(SystemAudio audioSystem, float dt)
        {
            CheckForInputFlags(dt);
            if (walking)
            {
                if (walkingUp)
                {
                    velocity.Velocity += new Vector3(0, walkingVelocity, 0); 
                    walkingUp = camera.cameraPosition.Y < 1.06f;
                    walkingDown = camera.cameraPosition.Y > 1.06f;
                }
                else if (walkingDown)
                {
                    velocity.Velocity += new Vector3(0, -walkingVelocity, 0);
                    walkingUp = camera.cameraPosition.Y < 1.0f;
                    walkingDown = camera.cameraPosition.Y > 1.0f;
                    if (walkingUp)
                        audioSystem.PlaySound(footstepAudio);
                }
            }
        }

        private void CheckForInputFlags(float dt)
        {
            Vector2 movementVec = new Vector2(0, 0);
            //Process any movement commands
            if (inputManager.IsActive("Forward"))
                movementVec.X = cameraVelocity;
            if (inputManager.IsActive("Backward"))
                movementVec.X = -cameraVelocity;
            if (inputManager.IsActive("Left"))
                movementVec.Y = -cameraVelocity;
            if (inputManager.IsActive("Right"))
                movementVec.Y = cameraVelocity;

            if (velocity.Velocity.X == 0 && velocity.Velocity.Z == 0 && walking)
            {
                transform.Position = new Vector3(transform.Position.X, 1.06f, transform.Position.Z);
                walking = false; //Walking is used for Visual effect of 'head-bob'
                walkingUp = false;
                walkingDown = true;
            }
            else if (movementVec.Length > 0 && !walking)
                walking = true;

            Vector3 movementX = Vector3.Zero;
            Vector3 movementY = Vector3.Zero;
            if (movementVec.X != 0)
            {
                movementX = movementVec.X * camera.cameraDirection;
                movementX.Y = 0;
            }
            if (movementVec.Y != 0)
            {
                movementY = movementVec.Y * Vector3.Cross(camera.cameraDirection.Normalized(), camera.cameraUp.Normalized());
                movementY.Y = 0;
            }
            if (movementX.Length != 0 || movementY.Length != 0)
            {
                movementX += movementY;
                movementX.Normalize(); //Normalize so diagonal movement isn't > orthogonal
                movementX *= cameraVelocity;
                velocity.Velocity.X = movementX.X;
                velocity.Velocity.Z = movementX.Z;
                velocity.Velocity.Y = 0;
            }
            else
            {
                velocity.Velocity = Vector3.Zero;
            }
            //Process mouse movement for the current frame
            UpdateView(dt);
            Mouse.SetPosition((sceneManager.Bounds.Left + sceneManager.Bounds.Right) / 2,
                            (sceneManager.Bounds.Top + sceneManager.Bounds.Bottom) / 2);
        }

        public void UpdateView(float dt)
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
            camera.UpdateView();
        }
    }
}
