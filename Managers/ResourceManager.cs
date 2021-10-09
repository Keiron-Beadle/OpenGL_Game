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
        static Dictionary<string, int> textureDictionary = new Dictionary<string, int>();

        public static void RemoveAllAssets()
        {
            foreach(var geometry in geometryDictionary)
            {
                geometry.Value.RemoveGeometry();
            }
            geometryDictionary.Clear();
            foreach(var texture in textureDictionary)
            {
                GL.DeleteTexture(texture.Value);
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
      
        public static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int texture;
            textureDictionary.TryGetValue(filename, out texture);
            if (texture == 0)
            {
                texture = GL.GenTexture();
                textureDictionary.Add(filename, texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                Bitmap bmp = new Bitmap(filename);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);
            }
 
            return texture;
        }
    }
}
