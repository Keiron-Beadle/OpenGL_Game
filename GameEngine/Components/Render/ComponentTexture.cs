﻿using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;

namespace OpenGL_Game.Components
{
    class ComponentTexture : IComponent
    {
        ITexture texture;

        public ComponentTexture(string textureName, SystemRender renderSystem)
        {
            texture = ResourceManager.LoadTexture(textureName, renderSystem);
        }

        public ITexture Texture
        {
            get { return texture; }
        }

        public override ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }
    }
}
