using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Components.Physics
{
    class ComponentBoxCollider : Collider
    {
        /// <summary>
        /// Use this constructor to create an arbitrarily-sized bounding box
        /// around an entity's transform
        /// </summary>
        /// <param name="entity">Entity which has Transform Component</param>
        /// <param name="minPos">Min Vector3 point for Bounding Box</param>
        /// <param name="maxPos">Max Vector3 point for Bounding Box</param>
        public ComponentBoxCollider(Entity entity, Vector3 minPos, Vector3 maxPos)
        {
            List<IComponent> components = entity.Components;
            IComponent trans = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
            });
            transform = (trans as ComponentTransform);
            Min = minPos;
            Max = maxPos;
        }

        /// <summary>
        /// Use this constructor if entity has geometry component we can 
        /// get the vertices from
        /// </summary>
        /// <param name="entity">Entity which has Transform & Geometry Component</param>
        public ComponentBoxCollider(Entity entity)
        {
            List<IComponent> components = entity.Components;
            IComponent trans = components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
            });
            IComponent geom = components.Find(delegate (IComponent component) {
                return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
            });
            transform = trans as ComponentTransform;

            Vector3[] vertices = (geom as ComponentGeometry).GetVertices();
            FindMinMax(vertices);
        }

        private void FindMinMax(Vector3[] vertices)
        { 
            Vector3 min = vertices[0];
            Vector3 max = vertices[0];

            foreach (Vector3 v in vertices)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    min = MinVec(min, vertices[i]);
                    max = MaxVec(max, vertices[i]);
                }

                Min = min;
                Max = max;
            }
        }
    }
}
