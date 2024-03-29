﻿using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Render;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentAudio : IComponent
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_AUDIO; } }

        public int Source { get; private set; }
        public int Buffer;
        private string entityName;
        private ComponentTransform sourceTransform;
        private ComponentCamera listener;

        public ComponentAudio(string audioFilePath, ComponentCamera pListener, Entity pSource)
        {
            entityName = pSource.Name;
            Source = AL.GenSource();
            Buffer = ResourceManager.LoadAudioBuffer(audioFilePath);
            AL.Source(Source, ALSourcei.Buffer, Buffer);
            listener = pListener;
            sourceTransform = pSource.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            listener.AttachObserver(this);
            sourceTransform.AttachObserver(this);
        }

        protected override void OnPropertyChanged(IComponent changedComponent)
        {
            if (changedComponent is ComponentCamera camera)
            {
                Vector3 pos = camera.cameraPosition;
                Vector3 dir = camera.cameraDirection;
                Vector3 up = camera.cameraUp;
                
                AL.Listener(ALListener3f.Position, ref pos);
                AL.Listener(ALListenerfv.Orientation, ref dir, ref up);
            }
            else if (changedComponent is ComponentTransform transform)
            {
                Update(transform);
            }
        }

        public void Update(ComponentTransform transform)
        {
            Vector3 pos = transform.Position;
            AL.Source(Source, ALSource3f.Position, ref pos);
        }

        public void Update(ComponentTransform transform, ComponentVelocity velocity)
        {
            Vector3 pos = transform.Position;
            AL.Source(Source, ALSource3f.Position, ref pos);
        }
    }
}
