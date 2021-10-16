using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Objects
{
    struct PointLight
    {
        public Vector3 position;

        public float constant;
        public float linear;
        public float quadratic;

        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
    }
}
