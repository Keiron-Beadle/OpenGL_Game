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
        static int NUMBEROFLIGHTS = 8;
        static PointLight[] pointLights = new PointLight[NUMBEROFLIGHTS];

        private int uniform_stex;
        private int uniform_mproj;
        private int uniform_mmodel;
        private int uniform_mview;
        private int uniform_mdiffuse;

        private int uniform_lightPosition;
        private int uniform_lightConstant;
        private int uniform_lightLinear;
        private int uniform_lightQuadratic;
        private int uniform_lightAmbient;
        private int uniform_lightDiffuse;
        private int uniform_lightSpec;


        public ComponentShaderPointLight(string vs, string fs) : base(vs, fs)
        {
            uniform_stex = GL.GetUniformLocation(ShaderID, "s_texture");
            uniform_mproj = GL.GetUniformLocation(ShaderID, "projection");
            uniform_mmodel = GL.GetUniformLocation(ShaderID, "model");
            uniform_mview = GL.GetUniformLocation(ShaderID, "view");
            uniform_mdiffuse = GL.GetUniformLocation(ShaderID, "diffuse");
            uniform_lightPosition = GL.GetUniformLocation(ShaderID, "pointLights[0].position");
            uniform_lightConstant = GL.GetUniformLocation(ShaderID, "pointLights[0].constant");
            uniform_lightLinear = GL.GetUniformLocation(ShaderID, "pointLights[0].linear");
            uniform_lightQuadratic = GL.GetUniformLocation(ShaderID, "pointLights[0].quadratic");
            uniform_lightAmbient = GL.GetUniformLocation(ShaderID, "pointLights[0].ambient");
            uniform_lightDiffuse = GL.GetUniformLocation(ShaderID, "pointLights[0].diffuse");
            uniform_lightSpec = GL.GetUniformLocation(ShaderID, "pointLights[0].specular");
        }

        public static void AddLight(PointLight pointLight)
        {
            pointLights[lightIndex] = pointLight;

            lightIndex++;
            if (lightIndex >= pointLights.Length)
                lightIndex = 0;
        }

        public override void ApplyShader(Matrix4 model, IGeometry geometry)
        {
            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            for (int i = 0; i < pointLights.Length; i++)
            {
                GL.Uniform3(uniform_lightPosition + (i*7) , pointLights[i].position); //Position
                GL.Uniform1(uniform_lightConstant + (i * 7), pointLights[i].constant); //Constant
                GL.Uniform1(uniform_lightLinear + (i * 7), pointLights[i].linear); //Linear
                GL.Uniform1(uniform_lightQuadratic + (i * 7), pointLights[i].quadratic); //Quadratic 
                GL.Uniform3(uniform_lightAmbient + (i * 7), pointLights[i].ambient); //Ambient
                GL.Uniform3(uniform_lightDiffuse + (i * 7), pointLights[i].diffuse); //Diffuse
                GL.Uniform3(uniform_lightSpec + (i * 7), pointLights[i].specular); //Specular
            }
            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            GL.UniformMatrix4(uniform_mview, false, ref GameScene.gameInstance.camera.view);
            GL.UniformMatrix4(uniform_mproj, false, ref GameScene.gameInstance.camera.projection);

            geometry.Render(uniform_mdiffuse);   // OBJ CHANGED
        }
    }
}
