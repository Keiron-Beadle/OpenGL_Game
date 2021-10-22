using OpenGL_Game.Components;
using OpenGL_Game.GameCode.Components.Controllers;
using OpenGL_Game.GameEngine.Components.Render;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OpenGL_Game.GameCode.Managers
{
    class MazeScriptManager : ScriptManager
    {
        private SceneManager sceneManger;
        private SystemRender renderSystem;
        private EntityManager entityManager;
        private InputManager inputManager;
        private ComponentCamera cameraComponent;
        private Entity player;

        /// <summary>
        /// Used in the main game scene, this is called to populate a List of walls
        /// via reading in a map in the form of a .xml file
        /// </summary>
        public void LoadMaze(string xmlFilePath, EntityManager pEntityManager, SceneManager pSceneManager, SystemRender pRenderSystem, InputManager pInputManager)
        {
            sceneManger = pSceneManager;
            renderSystem = pRenderSystem;
            entityManager = pEntityManager;
            inputManager = pInputManager;
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            float modelScale;
            Vector3 worldTranslate;
            modelScale = float.Parse(doc.SelectSingleNode("MapConfig/ModelScale").InnerText);
            XmlNode worldNode = doc.SelectSingleNode("MapConfig/WorldTranslate");
            worldTranslate = new Vector3(float.Parse(worldNode.Attributes["XTranslate"].Value), 0.0f,
                                            float.Parse(worldNode.Attributes["ZTranslate"].Value));
            GameScene.WorldTranslate = worldTranslate;
            LoadLights(doc);
            LoadMovingObjects(doc, worldTranslate);
            LoadWorldObjects(doc, worldTranslate);
            LoadPickups(doc, worldTranslate);
        }

        private void LoadPickups(XmlDocument doc, Vector3 worldTranslate)
        {
            XmlNodeList pickups = doc.SelectSingleNode("MapConfig/Pickups").ChildNodes;
            foreach (XmlNode n in pickups)
            {
                TAGS tag = TAGS.PICKUP;
                Entity entity = new Entity(n.Attributes["Name"].Value, tag);
                XmlNodeList components = n.ChildNodes;
                AddComponents(entity, worldTranslate, n, components);
                entityManager.AddEntity(entity);
            }
        }

        private void LoadMovingObjects(XmlDocument doc, Vector3 worldTranslate)
        {
            XmlNodeList movingObjectList = doc.SelectSingleNode("MapConfig/MovingObjects").ChildNodes;
            foreach (XmlNode n in movingObjectList)
            {
                TAGS tag = (n.Attributes["Type"].Value != "Player") ? TAGS.ENEMY : TAGS.PLAYER;
                Entity entity = new Entity(n.Attributes["Name"].Value, tag);
                if (entity.Name == "Player") { player = entity; }
                XmlNodeList components = n.ChildNodes;
                AddComponents(entity, worldTranslate, n, components);
                entityManager.AddEntity(entity);
            }
        }

        private void LoadLights(XmlDocument doc)
        {
            XmlNodeList listLights = doc.SelectSingleNode("MapConfig/Lights").ChildNodes;
            foreach (XmlNode n in listLights)
            {
                PointLight light = new PointLight
                {
                    position = new Vector3(float.Parse(n.Attributes["XPos"].Value),
                                            float.Parse(n.Attributes["YPos"].Value),
                                            float.Parse(n.Attributes["ZPos"].Value)),
                    constant = float.Parse(n.Attributes["Constant"].Value),
                    linear = float.Parse(n.Attributes["Linear"].Value),
                    quadratic = float.Parse(n.Attributes["Quadratic"].Value),
                    ambient = new Vector3(float.Parse(n.Attributes["XAmb"].Value),
                                            float.Parse(n.Attributes["YAmb"].Value),
                                            float.Parse(n.Attributes["ZAmb"].Value)),
                    diffuse = new Vector3(float.Parse(n.Attributes["XDiff"].Value),
                                            float.Parse(n.Attributes["YDiff"].Value),
                                            float.Parse(n.Attributes["ZDiff"].Value)),
                    specular = new Vector3(float.Parse(n.Attributes["XSpec"].Value),
                                            float.Parse(n.Attributes["YSpec"].Value),
                                            float.Parse(n.Attributes["ZSpec"].Value)),
                };

                ComponentShaderPointLight.AddLight(light);
            }
        }

        private void LoadWorldObjects(XmlDocument doc, Vector3 worldTranslate)
        {
            XmlNodeList objectNodeList = doc.SelectSingleNode("MapConfig/Terrain").ChildNodes;
            Random rnd = new Random();
            foreach (XmlNode n in objectNodeList)
            {
                TAGS tag = (n.Attributes["Type"].Value != "Floor") ? TAGS.WORLD : TAGS.NONE;
                Entity temp = new Entity(n.Attributes["Name"].Value, tag);
                XmlNodeList components = n.ChildNodes;
                AddComponents(temp, worldTranslate, n, components);
                entityManager.AddEntity(temp);
            }
        }

        private void AddComponents(Entity entity, Vector3 worldTranslate, XmlNode n,XmlNodeList components)
        {
            foreach (XmlNode component in components)
            {
                switch (component.Name)
                {
                    case "Transform":
                        AddTransformComponent(ref entity, component.Attributes, worldTranslate);
                        break;
                    case "Geometry":
                        AddGeometryComponent(ref entity, n.Attributes["Type"].Value, renderSystem);
                        AddShaderComponent(ref entity, n.Attributes["Type"].Value);
                        break;
                    case "Rotation":
                        AddRotationComponent(ref entity, component.Attributes);
                        break;
                    case "Velocity":
                        AddVelocityComponent(ref entity, component.Attributes);
                        break;
                    case "DroneController":
                        AddDroneController(ref entity, component.Attributes);
                        break;
                    case "BouncingController":
                        AddBouncingController(ref entity, component.Attributes);
                        break;
                    case "RollingController":
                        AddRollingController(ref entity, component.Attributes);
                        break;
                    case "PlayerController":
                        AddPlayerController(ref entity);
                        break;
                    case "Audio":
                        AddAudioComponent(ref entity, component.Attributes, cameraComponent);
                        break;
                    case "Collider":
                        AddColliderComponent(ref entity, component.Attributes);
                        break;
                    case "Camera":
                        AddCameraComponent(ref entity, component.Attributes, sceneManger);
                        cameraComponent = entity.FindComponentByType(ComponentTypes.COMPONENT_CAMERA) as ComponentCamera;
                        GameScene.gameInstance.playerCamera = cameraComponent;
                        break;
                }

            }
        }

        private void AddPlayerController(ref Entity entity)
        {
            var controller = new ComponentPlayerController(sceneManger, inputManager, entity);
            entity.AddComponent(controller);
            GameScene.gameInstance.controllerManager.AddController(controller);
        }

        private void AddRollingController(ref Entity entity, XmlAttributeCollection attributes)
        {
            Vector2 minPoint = new Vector2(float.Parse(attributes["XMin"].Value), float.Parse(attributes["ZMin"].Value));
            Vector2 maxPoint = new Vector2(float.Parse(attributes["XMax"].Value), float.Parse(attributes["ZMax"].Value));
            var controller = new ComponentRollingController(entity, maxPoint, minPoint);
            entity.AddComponent(controller);
            GameScene.gameInstance.controllerManager.AddController(controller);
        }

        private void AddBouncingController(ref Entity entity, XmlAttributeCollection attributes)
        {
            Vector2 minPoint = new Vector2(float.Parse(attributes["YMin"].Value), float.Parse(attributes["ZMin"].Value));
            Vector2 maxPoint = new Vector2(float.Parse(attributes["YMax"].Value), float.Parse(attributes["ZMax"].Value));
            var controller = new ComponentBouncingController(entity, maxPoint, minPoint);
            entity.AddComponent(controller);
            GameScene.gameInstance.controllerManager.AddController(controller);
        }

        private void AddDroneController(ref Entity entity, XmlAttributeCollection attributes)
        {
            var controller = new ComponentDroneController(entity, player, attributes["GraphTextPath"].Value);
            entity.AddComponent(controller);
            GameScene.gameInstance.controllerManager.AddController(controller);
        }
    }
}
