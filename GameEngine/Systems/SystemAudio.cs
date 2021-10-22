using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Systems
{
    class SystemAudio : ASystem
    {
        public static AudioContext Context = new AudioContext();

        public SystemAudio()
        {
            Name = "System Audio";
            MASK = ComponentTypes.COMPONENT_AUDIO | ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY;
        }

        public void PlaySound(ComponentAudio audioComp)
        {
            try
            {
                AL.SourcePlay(audioComp.Source);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public override void OnAction()
        {

        }

        public override void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
