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

        public void SetDiffuse(float maximumColourBeforeChange, Vector3 colour, SystemRender renderSystem)
        {
            if (renderSystem is OpenGLRenderer)
            {
                OpenGLGeometry glGeom = geometry as OpenGLGeometry;
                glGeom.ChangeDiffuse(maximumColourBeforeChange, colour);
            }
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
