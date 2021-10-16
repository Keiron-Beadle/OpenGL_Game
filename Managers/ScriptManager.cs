using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.Systems;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using static OpenGL_Game.Managers.InputManager;

namespace OpenGL_Game.Managers
{
    class ScriptManager
    {
        /// <summary>
        /// Used in the main game scene, this is called to populate a List of walls
        /// via reading in a map in the form of a .xml file
        /// </summary>
        public void LoadMaze(string xmlFilePath, EntityManager entityManager, SystemRender renderSystem)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            float modelScale;
            Vector3 worldTranslate;
            modelScale = float.Parse(doc.SelectSingleNode("MapConfig/ModelScale").InnerText);
            XmlNode worldNode = doc.SelectSingleNode("MapConfig/WorldTranslate");
            worldTranslate = new Vector3( float.Parse(worldNode.Attributes["XTranslate"].Value), 0.0f,
                                            float.Parse(worldNode.Attributes["ZTranslate"].Value));

            XmlNodeList objectNodeList = doc.SelectSingleNode("MapConfig/Objects").ChildNodes;
            Random rnd = new Random();
            foreach (XmlNode n in objectNodeList)
            {
                Entity temp = new Entity(n.Attributes["Name"].Value);
                XmlNodeList components = n.ChildNodes;
                foreach (XmlNode component in components)
                {
                    switch (component.Name)
                    {
                        case "Transform":
                            AddTransformComponent(ref temp, component.Attributes, worldTranslate);
                            break;
                        case "Geometry":
                            AddGeometryComponent(ref temp, component.Attributes["Type"].Value, renderSystem);
                            break;
                        case "Rotation":
                            AddRotationComponent(ref temp, component.Attributes);
                            break;
                        case "Velocity":
                            AddVelocityComponent(ref temp, component.Attributes);
                            break;
                    }
                }
                entityManager.AddEntity(temp);
            }
        }

        /// <summary>
        /// Adds a velocity component to the entity,
        /// does not add velocity if the magnitude of vector is == 0
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        private void AddVelocityComponent(ref Entity temp, XmlAttributeCollection attributes)
        {
            Vector3 velocity = new Vector3(float.Parse(attributes["XVel"].Value),
                                            float.Parse(attributes["YVel"].Value),
                                            float.Parse(attributes["ZVel"].Value));
            if (velocity.Length == 0) return; //If no velocity, don't bother adding the component
            temp.AddComponent(new ComponentVelocity(velocity));
        }
        
        /// <summary>
        /// Adds a rotation component to the entity,
        /// does not add rotation if magnitude of vector == 0
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        private void AddRotationComponent(ref Entity temp, XmlAttributeCollection attributes)
        {
            Vector3 rotation = new Vector3(float.Parse(attributes["XRot"].Value),
                                            float.Parse(attributes["YRot"].Value),
                                            float.Parse(attributes["ZRot"].Value));
            if (rotation.Length == 0) return; //If no rotation, don't bother adding the component
            temp.AddComponent(new ComponentRotation(rotation));
        }

        /// <summary>
        /// Adds a geometry component to an entity by the type of the object
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="type"></param>
        /// <param name="renderSystem"></param>
        private void AddGeometryComponent(ref Entity temp, string type, SystemRender renderSystem)
        {
            switch (type)
            {
                case "Corner":
                case "XWall":
                case "ZWall":
                case "Floor":
                    temp.AddComponent(new ComponentGeometry("Geometry/Wall/wall.obj", renderSystem));
                    break;
                case "XConnector":
                case "ZConnector":
                    temp.AddComponent(new ComponentGeometry("Geometry/Connector/connector.obj", renderSystem));
                    break;
                default:
                    throw new Exception("Undefined type when forming geometry component for .xml map object");
            }

            //Temporarily removed while I create the other models / textures. 

            //switch (type)
            //{
            //    case "Corner":
            //        temp.AddComponent(new ComponentGeometry("Geometry/Corner/corner.obj", renderSystem));
            //        break;
            //    case "XWall":
            //    case "ZWall":
            //        temp.AddComponent(new ComponentGeometry("Geometry/Wall/wall.obj", renderSystem));
            //        break;
            //    case "XConnector":
            //    case "ZConnector":
            //        temp.AddComponent(new ComponentGeometry("Geometry/Connector/connector.obj", renderSystem));
            //        break;
            //    case "Floor":
            //        temp.AddComponent(new ComponentGeometry("Geometry/Floor/floor.obj", renderSystem));
            //        break;
            //    default:
            //        throw new Exception("Undefined type when forming geometry component for .xml map object");
            //}
        }

        /// <summary>
        /// Adds a transform component to an entity by reading attributes from an XmlNode
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        /// <param name="worldTranslate"></param>
        private void AddTransformComponent(ref Entity temp, XmlAttributeCollection attributes, Vector3 worldTranslate)
        {
            Vector3 position = new Vector3( float.Parse(attributes["XPos"].Value),
                                            float.Parse(attributes["YPos"].Value),
                                            float.Parse(attributes["ZPos"].Value));
            Vector3 scale = Vector3.One;
            Vector3 rotation = Vector3.Zero;
            switch (attributes["Type"].Value)
            {
                case "Floor":
                    scale = new Vector3(33.333f, 0.06f, 1.0f);
                    break;
                case "Corner":
                    scale = new Vector3(35.0f, 0.7f, 1.0f);
                    break;
                case "XWall":
                    scale = new Vector3(1.0f, 0.5f, 1.0f);
                    rotation = new Vector3(0.0f, -MathHelper.PiOver2, 0.0f);
                    break;
                case "ZWall":
                    scale = new Vector3(1.0f, 0.5f, 1.0f);
                    break;
                case "XConnector":
                    scale = new Vector3(1.0f, 1.2f, 1.0f);
                    break;
                case "ZConnector":
                    scale = new Vector3(1.0f, 1.2f, 1.0f);
                    rotation = new Vector3(0.0f, -MathHelper.PiOver2, 0.0f);
                    break;
                default:
                    throw new Exception("Undefined type of object in map .xml");
            }
            temp.AddComponent(new ComponentTransform(position + worldTranslate, scale, rotation));
        }

        /// <summary>
        /// Used for the input manager, this will load controls from an .xml file and populate
        /// a dictionary of Keys/Controls for the caller.
        /// </summary>
        /// <param name="controlBindings"></param>
        public static void LoadControls(ref Dictionary<Key, CONTROLS> controlBindings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("controls.xml");
            XmlNode root = null;
            root = doc.SelectSingleNode("rootElement");
            foreach (XmlNode n in root.ChildNodes)
            {
                CONTROLS c = (CONTROLS)int.Parse(n.Attributes["Control"].Value);
                Key k = (Key)int.Parse(n.Attributes["Key"].Value);
                controlBindings.Add(k, c);
            }
        }
    }
}
