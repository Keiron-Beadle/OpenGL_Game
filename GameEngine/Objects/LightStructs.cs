using OpenTK;

namespace OpenGL_Game.Objects
{
    struct PointLight
    {
        public Vector3 position;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;

        public float constant;
        public float linear;
        public float quadratic;
    }

    struct SpotLight
    {
        public Vector3 position;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public Vector3 coneDirection;

        public float constant;
        public float linear;
        public float quadratic;
        public float cutoff;
    }
}
