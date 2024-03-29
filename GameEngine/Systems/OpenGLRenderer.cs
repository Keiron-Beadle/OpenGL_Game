﻿using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenGL_Game.Systems
{
    class OpenGLRenderer : SystemRender
    {
        
        public OpenGLRenderer()
        {
            Name = "OpenGL Renderer";
        }

        public override void OnAction()
        {
            foreach (Entity entity in entities)
            {
                IComponent geometryComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_GEOMETRY);
                IComponent transformComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);
                IComponent shaderComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_SHADER);

                OpenGLGeometry geometry = (OpenGLGeometry)((ComponentGeometry)geometryComponent).Geometry();

                Vector3 position = ((ComponentTransform)transformComponent).Position;
                Vector3 scale = ((ComponentTransform)transformComponent).Scale;
                Vector3 rotation = ((ComponentTransform)transformComponent).Rotation;
                Matrix4 xRot = Matrix4.CreateRotationX(rotation.X);
                Matrix4 yRot = Matrix4.CreateRotationY(rotation.Y);
                Matrix4 zRot = Matrix4.CreateRotationZ(rotation.Z);
                Matrix4 overallRot = xRot * yRot * zRot;
                Matrix4 model = Matrix4.CreateScale(scale) * overallRot * Matrix4.CreateTranslation(position);

                if (entity.Name == "Skybox")
                {
                    DrawSkybox(geometry, (ComponentShader)shaderComponent);
                }
                else
                    Draw(model, geometry, (ComponentShader)shaderComponent);

            }
        }

        private void DrawSkybox(OpenGLGeometry geometry, ComponentShader shader)
        {
            GL.DepthMask(false);
            Matrix4 model = Matrix4.CreateTranslation(GameScene.gameInstance.playerCamera.cameraPosition);
            Draw(model, geometry, shader);
            GL.DepthMask(true);
        }

        public override void Draw(Matrix4 model, OpenGLGeometry geometry, ComponentShader shader)
        {
            Debug.Assert(shader != null, "No shader attached to object.");
            shader.BindShader();
            shader.ApplyShader(model, geometry);
            shader.UnbindShader();
        }

        public override void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
