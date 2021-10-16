using OpenGL_Game.Objects;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Components
{
    class ComponentShaderPointLight : ComponentShader
    {
        static int lightIndex = 0;
        static int NUMBEROFLIGHTS = 1;
        static Vector3[] pointLights = new Vector3[NUMBEROFLIGHTS];

        private int uniform_stex;
        private int uniform_mproj;
        private int uniform_mmodel;
        private int uniform_mview;

        public ComponentShaderPointLight(string vs, string fs) : base(vs, fs)
        {
            uniform_stex = GL.GetUniformLocation(ShaderID, "s_texture");
            uniform_mproj = GL.GetUniformLocation(ShaderID, "projection");
            uniform_mmodel = GL.GetUniformLocation(ShaderID, "model");
            uniform_mview = GL.GetUniformLocation(ShaderID, "view");
        }

        public static void AddLight(Vector3 position)
        {
            pointLights[lightIndex] = position;
            lightIndex++;       
        }

        public override void ApplyShader(Matrix4 model, IGeometry geometry)
        {
            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            for (int i = 0; i < pointLights.Length; i++)
            {
                GL.Uniform3(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].position"), pointLights[i]);
                GL.Uniform1(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].constant"), 1.0f);
                GL.Uniform1(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].linear"), 0.09f);
                GL.Uniform1(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].quadratic"), 0.032f);
                GL.Uniform3(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].ambient"), new Vector3(0.01f, 0.01f, 0.01f));
                GL.Uniform3(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].diffuse"), new Vector3(0.06666f, 0.4705f, 0.23529f));
                GL.Uniform3(GL.GetUniformLocation(ShaderID, "pointLights["+i+"].specular"), new Vector3(1.0f, 1.0f, 1.0f));
            }
            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            GL.UniformMatrix4(uniform_mview, false, ref GameScene.gameInstance.camera.view);
            GL.UniformMatrix4(uniform_mproj, false, ref GameScene.gameInstance.camera.projection);

            geometry.Render();   // OBJ CHANGED
        }
    }
}
