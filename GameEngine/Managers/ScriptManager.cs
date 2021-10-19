using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
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
        protected void AddShaderComponent(ref Entity temp, string type)
        {
            switch (type)
            {
                case "Wall":
                //temp.AddComponent(new ComponentShaderBasic("GameCode/Shaders/vs.glsl", "GameCode/Shaders/fs.glsl"));
                //break;
                case "Corner":
                case "Connector":
                case "Floor":
                    //temp.AddComponent(new ComponentShaderBasic("GameCode/Shaders/vs.glsl", "GameCode/Shaders/fs.glsl"));
                    temp.AddComponent(new ComponentShaderPointLight("GameCode/Shaders/vsPointLight.glsl", "GameCode/Shaders/fsPointLight.glsl"));
                    break;
                case "Portal":
                    //temp.AddComponent(new ComponentShaderBasic("GameCode/Shaders/vs.glsl", "GameCode/Shaders/fs.glsl"));
                    temp.AddComponent(new ComponentShaderPointLight("GameCode/Shaders/vsPointLight.glsl", "GameCode/Shaders/fsPointLight.glsl"));
                    break;
            }
        }

        /// <summary>
        /// Adds a velocity component to the entity
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        protected void AddVelocityComponent(ref Entity temp, XmlAttributeCollection attributes)
        {
            Vector3 velocity = new Vector3(float.Parse(attributes["XVel"].Value),
                                            float.Parse(attributes["YVel"].Value),
                                            float.Parse(attributes["ZVel"].Value));
            temp.AddComponent(new ComponentVelocity(velocity));
        }

        /// <summary>
        /// Adds a rotation component to the entity
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        protected void AddRotationComponent(ref Entity temp, XmlAttributeCollection attributes)
        {
            Vector3 rotation = new Vector3(float.Parse(attributes["XRot"].Value),
                                            float.Parse(attributes["YRot"].Value),
                                            float.Parse(attributes["ZRot"].Value));
            temp.AddComponent(new ComponentRotation(rotation));
        }

        /// <summary>
        /// Adds a geometry component to an entity by the type of the object
        /// also adds the relative shader to the entity.
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="type"></param>
        /// <param name="renderSystem"></param>
        protected void AddGeometryComponent(ref Entity temp, string type, SystemRender renderSystem)
        {
            try
            {
                temp.AddComponent(new ComponentGeometry("GameCode/Geometry/" + type + '/' + type + ".obj", renderSystem));
                temp.AddComponent(new ComponentBoxCollider(temp));
            }
            catch(Exception e)
            {
                Console.WriteLine("Error loading geometry component from xml file: " + e.Message);
            }

        }

        /// <summary>
        /// Adds a transform component to an entity by reading attributes from an XmlNode
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="attributes"></param>
        /// <param name="worldTranslate"></param>
        protected void AddTransformComponent(ref Entity temp, XmlAttributeCollection attributes, Vector3 worldTranslate)
        {
            Vector3 position = new Vector3( float.Parse(attributes["XPos"].Value),
                                            float.Parse(attributes["YPos"].Value),
                                            float.Parse(attributes["ZPos"].Value));
            Vector3 rotation = new Vector3(float.Parse(attributes["XRot"].Value),
                                float.Parse(attributes["YRot"].Value),
                                float.Parse(attributes["ZRot"].Value));
            Vector3 scale = new Vector3(float.Parse(attributes["XScale"].Value),
                                            float.Parse(attributes["YScale"].Value),
                                            float.Parse(attributes["ZScale"].Value));

            temp.AddComponent(new ComponentTransform(position + worldTranslate, scale, rotation));
        }

        /// <summary>
        /// Used for the input manager, this will load controls from an .xml file and populate
        /// a dictionary of Keys/Controls for the caller.
        /// </summary>
        /// <param name="controlBindings"></param>
        public static void LoadTKControls(ref Dictionary<Key, string> controlBindings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("GameCode/controls.xml");
            XmlNode root = null;
            root = doc.SelectSingleNode("rootElement");
            foreach (XmlNode n in root.ChildNodes)
            {
                string c = n.Attributes["Control"].Value;
                Key k = (Key)int.Parse(n.Attributes["Key"].Value);
                controlBindings.Add(k, c);
            }
        }

        public static void SaveTKControls(ref Dictionary<Key, string> controlBindings)
        {
            XmlWriter writer = XmlWriter.Create("controls.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("rootElement");
            writer.WriteString("\n");
            foreach (var pair in controlBindings)
            {
                writer.WriteStartElement("Config");
                writer.WriteAttributeString("Control",pair.Value);
                writer.WriteAttributeString("Key", ((int)pair.Key).ToString());
                writer.WriteEndElement();
                writer.WriteString("\n");
                writer.Flush();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
    }
}
