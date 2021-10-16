using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;

namespace OpenGL_Game.Components
{
    class ComponentGeometry : IComponent
    {
        IGeometry geometry;

        public ComponentGeometry(string geometryName, SystemRender renderSystem)
        {
            this.geometry = ResourceManager.LoadGeometry(geometryName, renderSystem);
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_GEOMETRY; }
        }

        public IGeometry Geometry()
        {
            return geometry;
        }
    }
}
