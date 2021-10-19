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
    class ComponentShaderBasic : ComponentShader
    {
        private int uniform_stex;
        private int uniform_mmodelviewproj;
        private int uniform_mmodelview;
        private int uniform_mview;
        private int uniform_mdiffuse;

        public ComponentShaderBasic(string vertexSource, string fragmentSource)
            : base(vertexSource, fragmentSource)
        {
            uniform_stex = GL.GetUniformLocation(ShaderID, "s_texture");
            uniform_mmodelviewproj = GL.GetUniformLocation(ShaderID, "ModelViewProjMat");
            uniform_mmodelview = GL.GetUniformLocation(ShaderID, "ModelViewMat");
            uniform_mview = GL.GetUniformLocation(ShaderID, "ViewMat");
            uniform_mdiffuse = GL.GetUniformLocation(ShaderID, "diffuse");
        }

        public override void ApplyShader(Matrix4 model, IGeometry geometry)
        {
            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            Matrix4 modelView = model * GameScene.gameInstance.playerCamera.view;
            GL.UniformMatrix4(uniform_mmodelview, false, ref modelView);
            Matrix4 modelViewProjection = modelView * GameScene.gameInstance.playerCamera.projection;
            GL.UniformMatrix4(uniform_mmodelviewproj, false, ref modelViewProjection);
            GL.UniformMatrix4(uniform_mview, false, ref GameScene.gameInstance.playerCamera.view);

            geometry.Render(uniform_mdiffuse);   // OBJ CHANGED

        }
    }
}
