using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;
using System.IO;

namespace OpenGL_Game.Managers
{
    static class ResourceManager
    {
        static Dictionary<string, IGeometry> geometryDictionary = new Dictionary<string, IGeometry>();
        static Dictionary<string, ITexture> textureDictionary = new Dictionary<string, ITexture>();
        static Dictionary<string, int> shaderDictionary = new Dictionary<string, int>();

        public static void RemoveAllAssets()
        {
            foreach(var geometry in geometryDictionary)
            {
                geometry.Value.RemoveGeometry();
            }
            geometryDictionary.Clear();
            foreach(var texture in textureDictionary)
            {
                texture.Value.DeleteTexture();
            }
            textureDictionary.Clear();
            foreach(var shader in shaderDictionary)
            {
                GL.DeleteShader(shader.Value);
            }
            shaderDictionary.Clear();
        }

        public static IGeometry LoadGeometry(string filename, SystemRender renderSystem)
        {
            IGeometry geometry;
            geometryDictionary.TryGetValue(filename, out geometry);
            if (geometry == null)
            {
                if (renderSystem is OpenGLRenderer)
                    geometry = new OpenGLGeometry();
                geometry.LoadObject(filename, renderSystem);
                geometryDictionary.Add(filename, geometry);
            }

            return geometry;
        }
        
        public static ITexture LoadTexture(string filename, SystemRender renderSystem) //This return type will need to change for D3D I think
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            return renderSystem.LoadTexture(filename, ref textureDictionary);
        }

        public static int LoadOpenGLShader(string shaderSource, ShaderType type)
        {
            int id;
            shaderDictionary.TryGetValue(shaderSource, out id);
            if (id == 0)
            {
                id = GL.CreateShader(type);
                using (StreamReader sr = new StreamReader(shaderSource))
                {
                    GL.ShaderSource(id, sr.ReadToEnd());
                }
                GL.CompileShader(id);
                Console.WriteLine(GL.GetShaderInfoLog(id));
            }
            return id;
        }
    }
}
