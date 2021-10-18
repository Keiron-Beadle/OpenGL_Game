﻿using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class Camera
    {
        public Matrix4 view, projection;
        public Vector3 cameraPosition, cameraDirection, cameraUp;
        private Vector3 targetPosition;

        public Camera()
        {
            cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
            cameraDirection = new Vector3(0.0f, 0.0f, -1.0f);
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1.0f, 0.1f, 100f);
        }

        public Camera(Vector3 cameraPos, Vector3 targetPos, float ratio, float near, float far)
        {
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            cameraPosition = cameraPos;
            cameraDirection = targetPos-cameraPos;
            cameraDirection.Normalize();
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), ratio, near, far);
        }

        public void MoveForward(float move)
        {
            Vector3 movement = move * cameraDirection;
            movement.Y = 0;
            cameraPosition += movement;
        }

        public void MoveRight(float move)
        {
            Vector3 movement = move * Vector3.Cross(cameraDirection.Normalized(), cameraUp.Normalized());
            movement.Y = 0;
            cameraPosition += movement;
        }

        public void Translate(Vector3 move)
        {
            cameraPosition += move;
        }

        public void RotateY(float angle)
        {
            cameraDirection = Matrix3.CreateRotationY(angle) * cameraDirection;
        }

        public void UpdateView()
        {
            targetPosition = cameraPosition + cameraDirection;
            view = Matrix4.LookAt(cameraPosition, targetPosition, cameraUp);
        }
    }
}
