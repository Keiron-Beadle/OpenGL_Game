﻿using OpenGL_Game.Components;
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
        /// via reading in a map in the form of a .txt file
        /// </summary>
        /// <param name="pFilePath"></param>
        /// <param name="walls"></param>
        public void LoadMaze(string pFilePath, EntityManager entityManager, ISystem renderSystem)
        {
            LoadWalls(pFilePath, entityManager, renderSystem);
            LoadFloor("defaultFloor.txt", entityManager, renderSystem);
        }

        private void LoadFloor(string pFilePath, EntityManager entityManager, ISystem renderSystem)
        {
            const string WALL_OBJ_RELPATH = "Geometry/Wall/wall.obj";
            float wallScale = 4.0f; //This is how wide 1 wall is in the model of wall.obj. 
            int col = 0, row = 0;
            using (StreamReader sRead = new StreamReader(pFilePath))
            {
                string line;
                while ((line = sRead.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (c == ' ') { col++; continue; }
                        Entity floorboard = new Entity("Floor" + col + row); //needs unique name 
                        floorboard.AddComponent(new ComponentGeometry(WALL_OBJ_RELPATH, renderSystem));
                        Vector3 position = new Vector3(col * wallScale, 0.0f, row * wallScale);
                        Vector3 scale = new Vector3(33.333f, 0.06f, 1.0f);

                        floorboard.AddComponent(new ComponentTransform(position, scale, Vector3.Zero));
                        entityManager.AddEntity(floorboard);
                        col++;
                    }
                    col = 0;
                    row++;
                }
            }
        }

        private void LoadWalls(string pFilePath, EntityManager entityManager, ISystem renderSystem)
        {
            const string WALL_OBJ_RELPATH = "Geometry/Wall/wall.obj";
            const string CONNECTOR_OBJ_RELPATH = "Geometry/ConnectorWall/connector.obj";
            float wallScale = 4.0f; //This is how wide 1 wall is in the model of wall.obj. 
            int col = 0, row = 0;

            using (StreamReader sRead = new StreamReader(pFilePath))
            {
                string line;
                while ((line = sRead.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (c == ',') { col++; continue; }
                        Entity newWall = new Entity("Wall" + col + row); //needs unique name 
                        if (c == 'o')
                            newWall.AddComponent(new ComponentGeometry(CONNECTOR_OBJ_RELPATH, renderSystem));
                        else
                            newWall.AddComponent(new ComponentGeometry(WALL_OBJ_RELPATH, renderSystem));
                        Vector3 position = new Vector3(col * wallScale, 0.0f, row * wallScale);
                        Vector3 rotation;
                        Vector3 scale;
                        if (c == '-')
                            rotation = new Vector3(0.0f, -MathHelper.PiOver2, 0.0f);
                        else
                            rotation = Vector3.Zero;
                        if (c == 'x')
                            scale = new Vector3(35.0f, 1.0f, 1.0f);
                        else if (c == ' ')
                            scale = new Vector3(33.333f, 0.06f, 1.0f);
                        else
                            scale = Vector3.One;

                        newWall.AddComponent(new ComponentTransform(position, scale, rotation));
                        entityManager.AddEntity(newWall);
                        col++;
                    }
                    col = 0;
                    row++;
                }
            }
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
