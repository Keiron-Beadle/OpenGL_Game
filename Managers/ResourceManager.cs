﻿using System;
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

        public static IGeometry LoadGeometry(string filename, ISystem renderSystem)
        {
            IGeometry geometry;
            geometryDictionary.TryGetValue(filename, out geometry);
            if (geometry == null)
            {
                if (renderSystem is OpenGLRenderer)
                    geometry = new OpenGLGeometry();
                geometry.LoadObject(filename);
                geometryDictionary.Add(filename, geometry);
            }

            return geometry;
        }
        
        public static ITexture LoadTexture(string filename, ISystem renderSystem) //This return type will need to change for D3D I think
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);
            
            if (renderSystem is OpenGLRenderer)
            {
                return LoadOpenGLTexture(filename);
            }
            return null;
        }

        private static ITexture LoadOpenGLTexture(string filename)
        {
            var tempRenderer = new OpenGLRenderer();
            return tempRenderer.LoadTexture(filename, ref textureDictionary);      
        }
    }
}