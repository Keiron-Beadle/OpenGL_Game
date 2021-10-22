using System.Collections.Generic;
using OpenGL_Game.Systems;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    class SystemManager
    {
        List<ASystem> renderList = new List<ASystem>();
        List<ASystem> nonRenderList = new List<ASystem>();

        public SystemManager()
        {
        }

        public void ActionRenderSystems()
        {
            foreach (ASystem system in renderList)
            {
                system.OnAction();
            }
        }

        public void ActionNonRenderSystems()
        {
            foreach (ASystem system in nonRenderList)
            {
                system.OnAction();
            }
        }

        public void AddRenderSystem(ASystem system, EntityManager entities)
        {
            bool result = FindSystem(system.Name);
            system.InitialiseEntities(entities);
            renderList.Add(system);
        }

        public void AddNonRenderSystem(ASystem system, EntityManager entities)
        {
            bool result = FindSystem(system.Name);
            system.InitialiseEntities(entities);
            nonRenderList.Add(system);
        }

        public void RemoveEntityFromSystems(Entity entity)
        {
            foreach (var system in renderList)
            {
                system.RemoveEntity(entity);
            }
            foreach (var system in nonRenderList)
            {
                system.RemoveEntity(entity);
            }
        }

        private bool FindSystem(string name)
        {
            ASystem renderReturn = renderList.Find(delegate(ASystem system)
            {
                return system.Name == name;
            }
            );

            ASystem nonRenderReturn = nonRenderList.Find(delegate (ASystem system)
            {
                return system.Name == name;
            });

            return (renderReturn != null) || (nonRenderReturn != null);
        }
    }
}
