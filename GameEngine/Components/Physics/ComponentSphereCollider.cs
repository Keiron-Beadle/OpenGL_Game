using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Colliders;
using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentSphereCollider : ComponentCollider
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_COLLIDER; } }
        public List<SphereCollider> Colliders = new List<SphereCollider>();
        public Vector3 localOffSet = Vector3.Zero;

        public ComponentSphereCollider(Entity entity, Vector3 pCenter, float pRadius)
        {
            IComponent trans = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);
            transform = trans as ComponentTransform;
            Colliders.Add(new SphereCollider(pCenter, pRadius));
        }

        public ComponentSphereCollider(Entity entity)
        {
            IComponent trans = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);
            IComponent geom = entity.FindComponentByType(ComponentTypes.COMPONENT_GEOMETRY);
            transform = trans as ComponentTransform;
            Vector3[][] vertices = (geom as ComponentGeometry).GetVertices();
            Vector3 midpoint = new Vector3();
            int counter = 0;
            float maxDist = -float.MaxValue;
            foreach (Vector3[] v in vertices)
            {
                foreach (Vector3 u in v)
                {
                    midpoint += u;
                    counter++;
                    float dist = Vector3.Distance(Vector3.Zero, u);
                    if (dist > maxDist)
                    {
                        maxDist = dist;
                    }
                }
            }
            midpoint /= counter; //Work out mid point of vertices to form sphere around
            Colliders.Add(new SphereCollider(transform.Position, maxDist));
        }

        public override void Update()
        {
            foreach (SphereCollider c in Colliders)
                c.Update(transform.Position + localOffSet);
        }

        public void AddCollider(SphereCollider sphereCollider)
        {
            Colliders.Add(sphereCollider);
        }
    }
}
