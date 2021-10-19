using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenGL_Game.Systems;
using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentGeometry : IComponent
    {
        IGeometry geometry;

        public ComponentGeometry(string geometryName, SystemRender renderSystem)
        {
            this.geometry = ResourceManager.LoadGeometry(geometryName, renderSystem);
        }

        public override ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_GEOMETRY; }
        }

        public IGeometry Geometry()
        {
            return geometry;
        }

        public Vector3[][] GetVertices()
        {
            return geometry.GetVertices();
        }
    }
}
