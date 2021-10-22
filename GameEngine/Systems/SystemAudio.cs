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
            MASK = ComponentTypes.COMPONENT_AUDIO | ComponentTypes.COMPONENT_TRANSFORM;
        }

        public void PlaySound(ComponentAudio audioComp, bool pLooping)
        {
            try
            {
                AL.Source(audioComp.Source, ALSourceb.Looping, pLooping);
                AL.SourcePlay(audioComp.Source);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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

        public void StopSound(ComponentAudio audioComp)
        {
            AL.SourceStop(audioComp.Source);
        }

        public void StopAllSounds()
        {
            foreach (Entity e in entities)
            {
                ComponentAudio ca = e.FindComponentByType(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
                AL.SourceStop(ca.Source);
            }
        }

        public override void OnAction()
        {

        }

        public override void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public void ReplaceSound(ComponentAudio portalAudio, int portalOnlineBuffer)
        {
            AL.SourceStop(portalAudio.Source);
            portalAudio.Buffer = portalOnlineBuffer;
            AL.Source(portalAudio.Source, ALSourcei.Buffer, portalAudio.Buffer);
            AL.SourcePlay(portalAudio.Source);
        }
    }
}
