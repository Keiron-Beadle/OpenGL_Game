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
    class ComponentShaderLight : ComponentShader
    {
        static int pointLightIndex = 0;
        static int spotLightIndex = 0;
        static int NUMOFPOINTLIGHTS = 8;
        static int NUMOFSPOTLIGHTS = 4;
        static PointLight[] pointLights = new PointLight[NUMOFPOINTLIGHTS];
        static SpotLight[] spotLights = new SpotLight[NUMOFSPOTLIGHTS];

        private int uniform_stex;
        private int uniform_mproj;
        private int uniform_mmodel;
        private int uniform_mview;
        private int uniform_mdiffuse;

        private int uniform_pointLightPosition;
        private int uniform_pointLightConstant;
        private int uniform_pointLightLinear;
        private int uniform_pointLightQuadratic;
        private int uniform_pointLightAmbient;
        private int uniform_pointLightDiffuse;
        private int uniform_pointLightSpec;

        private int uniform_spotLightPosition;
        private int uniform_spotLightConstant;
        private int uniform_spotLightLinear;
        private int uniform_spotLightQuadratic;
        private int uniform_spotLightAmbient;
        private int uniform_spotLightDiffuse;
        private int uniform_spotLightSpec;
        private int uniform_spotLightAngle;
        private int uniform_spotLightDirection;


        public ComponentShaderLight(string vs, string fs) : base(vs, fs)
        {
            uniform_stex = GL.GetUniformLocation(ShaderID, "s_texture");
            uniform_mproj = GL.GetUniformLocation(ShaderID, "projection");
            uniform_mmodel = GL.GetUniformLocation(ShaderID, "model");
            uniform_mview = GL.GetUniformLocation(ShaderID, "view");
            uniform_mdiffuse = GL.GetUniformLocation(ShaderID, "diffuse");
            uniform_pointLightPosition = GL.GetUniformLocation(ShaderID, "pointLights[0].position"); //Arrays in GLSL
            uniform_pointLightConstant = GL.GetUniformLocation(ShaderID, "pointLights[0].constant"); //are contiguous
            uniform_pointLightLinear = GL.GetUniformLocation(ShaderID, "pointLights[0].linear"); //meaning I can get these
            uniform_pointLightQuadratic = GL.GetUniformLocation(ShaderID, "pointLights[0].quadratic"); //initial values of 
            uniform_pointLightAmbient = GL.GetUniformLocation(ShaderID, "pointLights[0].ambient"); //element 0 and then
            uniform_pointLightDiffuse = GL.GetUniformLocation(ShaderID, "pointLights[0].diffuse"); // * index by number of uniforms
            uniform_pointLightSpec = GL.GetUniformLocation(ShaderID, "pointLights[0].specular"); //to get next array uniform locations

            uniform_spotLightPosition = GL.GetUniformLocation(ShaderID, "spotLights[0].position"); //Arrays in GLSL
            uniform_spotLightConstant = GL.GetUniformLocation(ShaderID, "spotLights[0].constant"); //are contiguous
            uniform_spotLightLinear = GL.GetUniformLocation(ShaderID, "spotLights[0].linear"); //meaning I can get these
            uniform_spotLightQuadratic = GL.GetUniformLocation(ShaderID, "spotLights[0].quadratic"); //initial values of 
            uniform_spotLightAmbient = GL.GetUniformLocation(ShaderID, "spotLights[0].ambient"); //element 0 and then
            uniform_spotLightDiffuse = GL.GetUniformLocation(ShaderID, "spotLights[0].diffuse"); // * index by number of uniforms
            uniform_spotLightSpec = GL.GetUniformLocation(ShaderID, "spotLights[0].specular"); //to get next array uniform locations
            uniform_spotLightAngle = GL.GetUniformLocation(ShaderID, "spotLights[0].cutoff");
            uniform_spotLightDirection = GL.GetUniformLocation(ShaderID, "spotLights[0].coneDirection");
        }

        public static void AddLight(PointLight pointLight)
        {
            pointLights[pointLightIndex] = pointLight;

            pointLightIndex++;
            if (pointLightIndex >= pointLights.Length)
                pointLightIndex = 0;
        }

        public static void AddLight(SpotLight spotLight)
        {
            spotLights[spotLightIndex] = spotLight;
            spotLightIndex++;
            if (spotLightIndex >= spotLights.Length)
                spotLightIndex = 0;
        }

        public override void ApplyShader(Matrix4 model, IGeometry geometry)
        {
            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            for (int i = 0; i < pointLights.Length; i++)
            {
                GL.Uniform3(uniform_pointLightPosition + (i*7) , pointLights[i].position); //Position
                GL.Uniform1(uniform_pointLightConstant + (i * 7), pointLights[i].constant); //Constant
                GL.Uniform1(uniform_pointLightLinear + (i * 7), pointLights[i].linear); //Linear
                GL.Uniform1(uniform_pointLightQuadratic + (i * 7), pointLights[i].quadratic); //Quadratic 
                GL.Uniform3(uniform_pointLightAmbient + (i * 7), pointLights[i].ambient); //Ambient
                GL.Uniform3(uniform_pointLightDiffuse + (i * 7), pointLights[i].diffuse); //Diffuse
                GL.Uniform3(uniform_pointLightSpec + (i * 7), pointLights[i].specular); //Specular
            }

            for (int i = 0; i < spotLights.Length; i++)
            {
                GL.Uniform3(uniform_spotLightPosition + (i * 9), spotLights[i].position); //Position
                GL.Uniform1(uniform_spotLightConstant + (i * 9), spotLights[i].constant); //Constant
                GL.Uniform1(uniform_spotLightLinear + (i * 9), spotLights[i].linear); //Linear
                GL.Uniform1(uniform_spotLightQuadratic + (i * 9), spotLights[i].quadratic); //Quadratic 
                GL.Uniform3(uniform_spotLightAmbient + (i * 9), spotLights[i].ambient); //Ambient
                GL.Uniform3(uniform_spotLightDiffuse + (i * 9), spotLights[i].diffuse); //Diffuse
                GL.Uniform3(uniform_spotLightSpec + (i * 9), spotLights[i].specular); //Specular
                GL.Uniform1(uniform_spotLightAngle + (i * 9), (float)Math.Cos(spotLights[i].cutoff)); //Cosine of cutoff angle
                GL.Uniform3(uniform_spotLightDirection + (i * 9), -Vector3.UnitY); //Cone direction
            }

            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            GL.UniformMatrix4(uniform_mview, false, ref GameScene.gameInstance.playerCamera.view);
            GL.UniformMatrix4(uniform_mproj, false, ref GameScene.gameInstance.playerCamera.projection);

            geometry.Render(uniform_mdiffuse);   // OBJ CHANGED
        }
    }
}
