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
            masks.Add(ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_AUDIO);
        }

        public void PlaySound(Entity entity)
        {
            try
            {
                IComponent audioComp = entity.Components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });
                ComponentAudio acomp = audioComp as ComponentAudio;
                AL.SourcePlay(acomp.Source);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        protected override void OnAction(ComponentTypes currentMask)
        {
            AL.Listener(ALListener3f.Position, ref GameScene.gameInstance.camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref GameScene.gameInstance.camera.cameraDirection, ref GameScene.gameInstance.camera.cameraUp);

            for (int i = 0; i < entities.Count; i++)
            {
                List<IComponent> components = entities[i].Components;

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });

                IComponent audioComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });

                ((ComponentAudio)audioComponent).Update((ComponentTransform)transformComponent, (ComponentVelocity)velocityComponent);
            }
        }
    }
}
