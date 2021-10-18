using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace OpenGL_Game.Managers
{
    static class ResourceManager
    {
        static Dictionary<string, IGeometry> geometryDictionary = new Dictionary<string, IGeometry>();
        static Dictionary<string, ITexture> textureDictionary = new Dictionary<string, ITexture>();
        static Dictionary<string, int> shaderDictionary = new Dictionary<string, int>();
        static Dictionary<string, int> audioBufferDictionary = new Dictionary<string, int>();

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
            audioBufferDictionary.Clear();
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

        public static int LoadAudioBuffer(string audioSource)
        {
            int id;
            audioBufferDictionary.TryGetValue(audioSource, out id);
            if (id == 0)
            {
                id = AL.GenBuffer();
                audioBufferDictionary.Add(audioSource, id);

                //Load .wav from file
                int channels, bits_per_sample, sample_rate;
                byte[] sound_data = LoadWave(
                    File.Open(audioSource, FileMode.Open),
                    out channels,
                    out bits_per_sample,
                    out sample_rate);
                ALFormat sound_format =
                    channels == 1 && bits_per_sample == 8 ? ALFormat.Mono8 :
                    channels == 1 && bits_per_sample == 16 ? ALFormat.Mono16 :
                    channels == 2 && bits_per_sample == 8 ? ALFormat.Stereo8 :
                    channels == 2 && bits_per_sample == 16 ? ALFormat.Stereo16 :
                    (ALFormat)0;
                AL.BufferData(id, sound_format, sound_data, sound_data.Length, sample_rate);
                if (AL.GetError() != ALError.NoError)
                {
                    Console.WriteLine(AL.GetError());
                }

            }
            return id;
        }

        private static byte[] LoadWave(FileStream fileStream, out int channels, out int bits_per_sample, out int rate)
        {
            if (fileStream == null)
                throw new ArgumentNullException("File stream is null");

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file");

                int riff_chunk_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified wave file is not supported");

                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int BPS = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported");
              
                int data_chunk_size = reader.ReadInt32();
                channels = num_channels;
                bits_per_sample = BPS;
                rate = sample_rate;

                return reader.ReadBytes(data_chunk_size);
            }

        }
    }
}
