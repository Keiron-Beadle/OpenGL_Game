using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Colliders;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.GameEngine.Components.Render;
using OpenGL_Game.GameEngine.Pathing;
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

        protected void AddShaderComponent(ref Entity temp, string type)
        {
            switch (type)
            {
                default:
                    //temp.AddComponent(new ComponentShaderPointLight("GameCode/Shaders/vsPointLight.glsl", "GameCode/Shaders/fsPointLight.glsl"));
                    temp.AddComponent(new ComponentShaderBasic("GameCode/Shaders/vs.glsl", "GameCode/Shaders/fs.glsl"));
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
                //temp.AddComponent(new ComponentBoxCollider(temp));
            }
            catch (Exception e)
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
            Vector3 position = new Vector3(float.Parse(attributes["XPos"].Value),
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
        /// Adds an audio component to an enitty, by reading attributes from the xml node
        /// </summary>
        protected void AddAudioComponent(ref Entity entity, XmlAttributeCollection attributes, ComponentCamera listener)
        {
            entity.AddComponent(new ComponentAudio(attributes["SourceFile"].Value, listener, entity));
        }

        protected void AddCameraComponent(ref Entity entity, XmlAttributeCollection attributes, SceneManager pSceneManger)
        {
            Vector3 target = new Vector3(float.Parse(attributes["XTarget"].Value),
                            float.Parse(attributes["YTarget"].Value),
                            float.Parse(attributes["ZTarget"].Value));
            float nearPlane = float.Parse(attributes["Near"].Value);
            float farPlane = float.Parse(attributes["Far"].Value);
            entity.AddComponent(new ComponentCamera(entity, target, 
                (float)pSceneManger.Width / (float)pSceneManger.Height, nearPlane, farPlane));
        }

        protected void AddColliderComponent(ref Entity entity, XmlAttributeCollection attributes)
        {
            switch (attributes["Type"].Value)
            {
                case "Sphere":
                    AddSphereColliderComponent(ref entity, attributes);
                    break;
                case "Box":
                    AddBoxColliderComponent(ref entity, attributes);
                    break;
            }
        }
        
        private void AddBoxColliderComponent(ref Entity entity, XmlAttributeCollection attributes)
        {
            ComponentBoxCollider bc = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentBoxCollider;
            if (bc == null)
            {
                if (attributes["XMin"] != null)
                {
                    Vector3 minPos = new Vector3(float.Parse(attributes["XMin"].Value),
                            float.Parse(attributes["YMin"].Value),
                            float.Parse(attributes["ZMin"].Value));
                    Vector3 maxPos = new Vector3(float.Parse(attributes["XMax"].Value),
                            float.Parse(attributes["YMax"].Value),
                            float.Parse(attributes["ZMax"].Value));
                    entity.AddComponent(new ComponentBoxCollider(entity, minPos, maxPos));
                }
                else
                {
                    entity.AddComponent(new ComponentBoxCollider(entity));
                }
            }
            else
            {
                ComponentTransform transform = entity.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
                if (attributes["XMin"] != null)
                {
                    Vector3 minPos = new Vector3(float.Parse(attributes["XMin"].Value),
                                    float.Parse(attributes["YMin"].Value),
                                    float.Parse(attributes["ZMin"].Value));
                    Vector3 maxPos = new Vector3(float.Parse(attributes["XMax"].Value),
                            float.Parse(attributes["YMax"].Value),
                            float.Parse(attributes["ZMax"].Value));
                    bc.Colliders.Add(new BoxCollider(minPos, maxPos, transform));
                }
                else
                {
                    ComponentGeometry geom = entity.FindComponentByType(ComponentTypes.COMPONENT_GEOMETRY) as ComponentGeometry;
                    bc.Colliders.Add(new BoxCollider(geom.GetVertices()[0], transform));
                }
            }
        }

        private void AddSphereColliderComponent(ref Entity entity, XmlAttributeCollection attributes) 
        {
            Vector3 center = Vector3.Zero;
            float radius = -1;
            if (attributes["XCenter"] != null)
            {
                center = new Vector3(float.Parse(attributes["XCenter"].Value),
                                            float.Parse(attributes["YCenter"].Value),
                                            float.Parse(attributes["ZCenter"].Value));
                radius = float.Parse(attributes["Radius"].Value);
            }

            ComponentSphereCollider compCollider = entity.FindComponentByType(ComponentTypes.COMPONENT_COLLIDER) as ComponentSphereCollider;

            if (compCollider == null)
            {
                if (attributes["XCenter"] != null)
                {
                    entity.AddComponent(new ComponentSphereCollider(entity, center,radius));
                }
                else
                {
                    entity.AddComponent(new ComponentSphereCollider(entity));
                }
            }
            else
            {
                if (attributes["XOffSet"] == null)
                {
                    compCollider.AddCollider(new GameEngine.Colliders.SphereCollider(center, radius));
                }
                else if (attributes["XCenter"] != null)
                {
                    Vector3 offset = new Vector3(float.Parse(attributes["XOffSet"].Value),
                        float.Parse(attributes["YOffSet"].Value),
                        float.Parse(attributes["ZOffSet"].Value));
                    compCollider.AddCollider(new GameEngine.Colliders.SphereCollider(center, radius, offset));
                }
            }
        }

        public static List<Node> LoadMap(string inMapFilePath)
        {
            List<string> lines = new List<string>();
            List<int> columnCounts = new List<int>();
            using (StreamReader s = new StreamReader(inMapFilePath)) 
            {
                string line;
                while ((line = s.ReadLine()) != null) 
                {
                    lines.Add(line);
                    columnCounts.Add(line.Length);
                }
            }
            char[][] map = new char[lines.Count][];
            for (int i = 0; i <= map.GetUpperBound(0); i++) 
            {
                map[i] = new char[columnCounts[i]]; 
            }
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (lines[i][j] == 'n')
                    {
                        Node n = new Node
                        {
                            Position = new Vector2(j, i),
                            neighbours = new List<Node>()
                        };
                        nodes.Add(n);
                    }
                    map[i][j] = lines[i][j];
                }
            }

            foreach (Node node in nodes)
            {
                foreach (Node neighbour in nodes)
                {
                    if (neighbour.Position == node.Position) continue;
                    if (StraightLineToNeighbour(map, node.Position, neighbour.Position))
                    {
                        node.neighbours.Add(neighbour);
                    }
                }
            }

            //Code for writing nodes + neighbours to text file to check connections
            //using (StreamWriter s = new StreamWriter("nodes.txt"))
            //{
            //    for (int i = 0; i < nodes.Count; i++)
            //    {
            //        s.Write("Node: " + i + ' ' + nodes[i].Position + "   Neighbours: ");
            //        for (int j = 0; j < nodes.Count; j++)
            //        {
            //            if (nodes[i].neighbours.Contains(nodes[j]))
            //            {
            //                s.Write(j + " ");
            //            }
            //        }
            //        s.WriteLine();
            //    }
            //}

            return nodes;
        }

        private static bool StraightLineToNeighbour(char[][] map, Vector2 position1, Vector2 position2)
        {
            //Bresenham Line Rasterization algorithm
            //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#Algorithm

            if (Math.Abs(position2.Y - position1.Y) < Math.Abs(position2.X - position1.X))
            {
                if (position1.X > position2.X)
                    return PlotLowLine(map, position2, position1);
                else
                    return PlotLowLine(map, position1, position2);
            }
            else
            {
                if (position1.Y > position2.Y)
                    return PlotHighLine(map, position2, position1);
                else
                    return PlotHighLine(map, position1, position2);
            }
        }

        private static bool PlotLowLine(char[][] map, Vector2 start, Vector2 end)
        {
            try
            {
                Vector2 dir = end - start;
                int yi = 1;
                if (dir.Y < 0)
                {
                    yi = -1;
                    dir.Y *= -1;
                }
                int d = (2 * (int)dir.Y) - (int)dir.X;
                int y = (int)start.Y;

                for (int x = (int)start.X; x <= (int)end.X; x++) //Traverse line 
                {
                    char c = map[y][x];
                    if (c == 'x') return false;
                    if (d > 0)
                    {
                        y = y + yi;
                        d = d + (2 * ((int)dir.Y - (int)dir.X));
                    }
                    else
                        d = d + 2 * (int)dir.Y;
                }
                return true;
            }
            catch { return false; }

        }

        private static bool PlotHighLine(char[][] map, Vector2 start, Vector2 end)
        {
            try
            {
                Vector2 dir = end - start;
                int xi = 1;
                if (dir.X < 0)
                {
                    xi = -1;
                    dir.X = -dir.X;
                }
                int d = (2 * (int)dir.X) - (int)dir.Y;
                int x = (int)start.X;
                for (int y = (int)start.Y; y < (int)end.Y; y++)
                {
                    char c = map[y][x];
                    if (c == 'x') return false;
                    if (d > 0)
                    {
                        x = x + xi;
                        d = d + (2 * ((int)dir.X - (int)dir.Y));
                    }
                    else
                    {
                        d = d + (2 * (int)dir.X);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
