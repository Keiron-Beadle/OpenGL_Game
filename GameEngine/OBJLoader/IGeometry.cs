using OpenGL_Game.Systems;
using OpenTK;

namespace OpenGL_Game.OBJLoader
{
    interface IGeometry
    {
        void Render(int diffuseLocation);
        void LoadObject(string filename, SystemRender renderSystem);
        void RemoveGeometry();
        Vector3[][] GetVertices();
    }
}
