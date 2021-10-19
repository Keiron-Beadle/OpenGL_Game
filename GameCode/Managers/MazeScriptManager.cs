using OpenGL_Game.Components;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
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
            worldTranslate = new Vector3(float.Parse(worldNode.Attributes["XTranslate"].Value), 0.0f,
                                            float.Parse(worldNode.Attributes["ZTranslate"].Value));

            LoadLights(doc);
            LoadWorldObjects(entityManager, renderSystem, doc, worldTranslate);
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

        private void LoadWorldObjects(EntityManager entityManager, SystemRender renderSystem, XmlDocument doc, Vector3 worldTranslate)
        {
            XmlNodeList objectNodeList = doc.SelectSingleNode("MapConfig/Objects").ChildNodes;
            Random rnd = new Random();
            foreach (XmlNode n in objectNodeList)
            {
                TAGS tag;
                if (n.Attributes["Type"].Value != "Floor")
                    tag = TAGS.WORLD;
                else
                    tag = TAGS.NONE;
                Entity temp = new Entity(n.Attributes["Name"].Value, tag);
                XmlNodeList components = n.ChildNodes;
                foreach (XmlNode component in components)
                {
                    switch (component.Name)
                    {
                        case "Transform":
                            AddTransformComponent(ref temp, component.Attributes, worldTranslate);
                            break;
                        case "Geometry":
                            AddGeometryComponent(ref temp, n.Attributes["Type"].Value, renderSystem);
                            AddShaderComponent(ref temp, n.Attributes["Type"].Value);
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
    }
}
