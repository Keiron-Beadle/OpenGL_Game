using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Render
{
    class ComponentCamera : IComponent
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_CAMERA; } }

        private ComponentTransform transform;
        public Matrix4 view, projection;
        public Vector3 cameraPosition, cameraDirection, cameraUp;
        private Vector3 targetPosition;

        public ComponentCamera(Entity host) : this(host, new Vector3(0, 0, -1f), 1.0f, 0.1f, 100f)
        { }

        public ComponentCamera(Entity host, Vector3 targetPos, float ratio, float near, float far)
        {
            transform = host.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            transform.AttachObserver(this); //Attach camera component to transform component, this will give me updates when it changes
            cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            cameraPosition = transform.Position;
            cameraDirection = targetPos - transform.Position;
            cameraDirection.Normalize();
            UpdateView();
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), ratio, near, far);
            Notify();
        }

        protected override void OnPropertyChanged(IComponent changedComponent)
        {
            if (changedComponent is ComponentTransform transform)
            {
                cameraPosition = transform.Position;
                cameraDirection = targetPosition - transform.Position;
                cameraDirection.Normalize();
                UpdateView();
                Notify();
            }
        }

        public void MoveForward(float move)
        {
            Vector3 movement = move * cameraDirection;
            movement.Y = 0;
            cameraPosition += movement;
            Notify();
        }

        public void MoveRight(float move)
        {
            Vector3 movement = move * Vector3.Cross(cameraDirection.Normalized(), cameraUp.Normalized());
            movement.Y = 0;
            cameraPosition += movement;
            Notify();
        }

        public void UpdateView()
        {
            targetPosition = cameraPosition + cameraDirection;
            view = Matrix4.LookAt(cameraPosition, targetPosition, cameraUp);
            Notify();
        }
    }
}
