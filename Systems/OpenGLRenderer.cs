using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenGL_Game.Systems
{
    class OpenGLRenderer : SystemRender
    {
        private int pgmID;
        private int vsID;
        private int fsID;
        private int uniform_stex;
        private int uniform_mmodelviewproj;
        private int uniform_mmodelview;
        private int uniform_mview;
        private int uniform_diffuse;  // OBJ NEW
        private int uniform_EyePosition;

        public OpenGLRenderer()
        {
            Name = "OpenGL Renderer";
            pgmID = GL.CreateProgram(); //Generic shader for walls / objects
            LoadShader("Shaders/vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("Shaders/fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            uniform_stex = GL.GetUniformLocation(pgmID, "s_texture");
            uniform_mmodelviewproj = GL.GetUniformLocation(pgmID, "ModelViewProjMat");
            uniform_mmodelview = GL.GetUniformLocation(pgmID, "ModelViewMat");
            uniform_diffuse = GL.GetUniformLocation(pgmID, "v_diffuse");     // OBJ NEW
            uniform_EyePosition = GL.GetUniformLocation(pgmID, "EyePosition");
            uniform_mview = GL.GetUniformLocation(pgmID, "ViewMat");
        }

        void LoadShader(string filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public override void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                OpenGLGeometry geometry = (OpenGLGeometry)((ComponentGeometry)geometryComponent).Geometry();

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                Vector3 position = ((ComponentTransform)transformComponent).Position;
                Vector3 scale = ((ComponentTransform)transformComponent).Scale;
                Vector3 rotation = ((ComponentTransform)transformComponent).Rotation;
                Matrix4 xRot = Matrix4.CreateRotationX(rotation.X);
                Matrix4 yRot = Matrix4.CreateRotationY(rotation.Y);
                Matrix4 zRot = Matrix4.CreateRotationZ(rotation.Z);
                Matrix4 overallRot = xRot * yRot * zRot;
                Matrix4 model = Matrix4.CreateScale(scale) * overallRot * Matrix4.CreateTranslation(position);

                GL.Uniform3(uniform_EyePosition, GameScene.gameInstance.camera.cameraPosition);

                if (entity.Name == "Skybox")
                {
                    DrawSkybox(geometry);
                }
                else
                    Draw(model, geometry);
            }
        }

        private void DrawSkybox(OpenGLGeometry geometry)
        {
            GL.DepthMask(false);
            Matrix4 model = Matrix4.CreateTranslation(GameScene.gameInstance.camera.cameraPosition);
            //Matrix4 model = Matrix4.CreateTranslation(Vector3.Zero);
            Draw(model, geometry);
            GL.DepthMask(true);
        }

        public override void Draw(Matrix4 model, OpenGLGeometry geometry)
        {
            GL.UseProgram(pgmID);

            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            Matrix4 modelView = model * GameScene.gameInstance.camera.view;
            GL.UniformMatrix4(uniform_mmodelview, false, ref modelView);
            Matrix4 modelViewProjection = modelView * GameScene.gameInstance.camera.projection;
            GL.UniformMatrix4(uniform_mmodelviewproj, false, ref modelViewProjection);
            GL.UniformMatrix4(uniform_mview, false, ref GameScene.gameInstance.camera.view);

            geometry.Render(uniform_diffuse);   // OBJ CHANGED

            GL.UseProgram(0);
        }

        public override ITexture LoadTexture(string filepath, ref Dictionary<string,ITexture> textureDictionary)
        {
            ITexture texture;
            textureDictionary.TryGetValue(filepath, out texture);
            if (texture == null)
            {
                texture = new OpenGLTexture();
                texture.ID = GL.GenTexture();
                textureDictionary.Add(filepath, texture);
                GL.BindTexture(TextureTarget.Texture2D, texture.ID);

                // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                Bitmap bmp = new Bitmap(filepath);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);
            }

            return texture;
        }
    }
}
