using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentAudio : IComponent
    {
        public ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_AUDIO; } }

        public int Source { get; private set; }
        public int Buffer;

        public ComponentAudio(string audioFilePath)
        {
            Source = AL.GenSource();
            Buffer = ResourceManager.LoadAudioBuffer(audioFilePath);
            AL.Source(Source, ALSourcei.Buffer, Buffer);
        }
    }
}
