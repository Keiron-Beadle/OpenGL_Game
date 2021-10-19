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
    class ComponentBoxCollider : ComponentCollider
    {
        public override ComponentTypes ComponentType { get { return ComponentTypes.COMPONENT_COLLIDER; } }
        public List<BoxCollider> Colliders = new List<BoxCollider>();

        /// <summary>
        /// Use this constructor to create an arbitrarily-sized bounding box
        /// around an entity's transform
        /// </summary>
        /// <param name="entity">Entity which has Transform Component</param>
        /// <param name="minPos">Min Vector3 point for Bounding Box</param>
        /// <param name="maxPos">Max Vector3 point for Bounding Box</param>
        public ComponentBoxCollider(Entity entity, Vector3 minPos, Vector3 maxPos)
        {
            IComponent trans = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);
            transform = trans as ComponentTransform;
            Colliders.Add(new BoxCollider(minPos, maxPos, transform));
        }

        /// <summary>
        /// Use this constructor if entity has geometry component we can 
        /// get the vertices from
        /// </summary>
        /// <param name="entity">Entity which has Transform & Geometry Component</param>
        public ComponentBoxCollider(Entity entity)
        {
            IComponent trans = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM);
            IComponent geom = entity.FindComponentByType(ComponentTypes.COMPONENT_GEOMETRY);
            transform = trans as ComponentTransform;
            Vector3[][] vertices = (geom as ComponentGeometry).GetVertices();

            //Create a collider for each group in the mesh
            //Allows for some simple means of colliding with concave meshes, if 
            //mesh is split into convex groups in blender/3D modelling software
            for (int i = 0; i <= vertices.GetUpperBound(0); i++) 
            {                                         
                Colliders.Add(new BoxCollider(vertices[i], transform));
            }
        }

        public override void Update()
        {
            foreach (var collider in Colliders)
                collider.Update(transform);
        }
    }
}
