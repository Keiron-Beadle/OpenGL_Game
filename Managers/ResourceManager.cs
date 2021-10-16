using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;

namespace OpenGL_Game.Managers
{
    static class ResourceManager
    {
        static Dictionary<string, IGeometry> geometryDictionary = new Dictionary<string, IGeometry>();
        static Dictionary<string, ITexture> textureDictionary = new Dictionary<string, ITexture>();

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
        }

        public static IGeometry LoadGeometry(string filename, ISystem renderSystem, string optionalTextureOverride = null)
        {
            IGeometry geometry;
            string overrideString = optionalTextureOverride == null ? "null" : optionalTextureOverride;
            bool present = geometryDictionary.TryGetValue(filename + overrideString, out geometry);

            if (present) //We need to check if the geometry has the same texture as well, otherwise we need to load a new geometry with this new texture
            {
                if (optionalTextureOverride != ((OpenGLGeometry)geometry).OverrideTexturePath)
                    present = false;
            }

            if (!present) //If geometry not present in dictionary, we load it
            {
                if (renderSystem is OpenGLRenderer)
                    geometry = new OpenGLGeometry();
                geometry.LoadObject(filename, renderSystem, optionalTextureOverride);
                geometryDictionary.Add(filename + overrideString, geometry);
            }

            return geometry;
        }
        
        public static ITexture LoadTexture(string filename, ISystem renderSystem) //This return type will need to change for D3D I think
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);
            
            if (renderSystem is OpenGLRenderer)
            {
                return LoadOpenGLTexture(filename, renderSystem as OpenGLRenderer);
            }
            return null;
        }

        private static ITexture LoadOpenGLTexture(string filename, OpenGLRenderer renderSystem)
        {
            return renderSystem.LoadTexture(filename, ref textureDictionary);      
        }
    }
}
