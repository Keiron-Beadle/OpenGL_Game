using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OpenGL_Game.Managers
{
    class InputManager
    {
        private KeyboardState prevKeyState;
        private MouseState prevMouseState;
        private bool[] controlFlags;
        private Dictionary<Key, CONTROLS> controlBindings;

        public enum CONTROLS
        {
            Forward,
            Backward,
            Left,
            Right,
        }

        public InputManager()
        {
            controlFlags = new bool[sizeof(CONTROLS)];
            controlBindings = new Dictionary<Key, CONTROLS>();
            LoadControls();
            SaveControls();

        }

        private void SaveControls()
        {
            XmlWriter writer = XmlWriter.Create("controls.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("rootElement");
            writer.WriteString("\n");
            foreach (var pair in controlBindings)
            {
                writer.WriteStartElement("Config");
                writer.WriteAttributeString("Control", ((int)pair.Value).ToString());
                writer.WriteAttributeString("Key", ((int)pair.Key).ToString());
                writer.WriteEndElement();
                writer.WriteString("\n");
                writer.Flush();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

        private void GenerateControls()
        {
            Key w = Key.W;
            Key a = Key.A;
            Key s = Key.S;
            Key d = Key.D;
            Key up = Key.Up;
            Key down = Key.Down;
            Key right = Key.Right;
            Key left = Key.Left;

            CONTROLS forward = CONTROLS.Forward;
            CONTROLS backward = CONTROLS.Backward;
            CONTROLS leftC = CONTROLS.Left;
            CONTROLS rightC = CONTROLS.Right;

            controlBindings.Add(w,forward);
            controlBindings.Add(s,backward);
            controlBindings.Add(a,leftC);
            controlBindings.Add(d,rightC);
            controlBindings.Add(up,forward);
            controlBindings.Add(down,backward);
            controlBindings.Add(left,leftC);
            controlBindings.Add(right,rightC);

            SaveControls();
        }

        private void LoadControls()
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

        public void Update(FrameEventArgs e)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            


            prevMouseState = currentMouseState;
            prevKeyState = currentKeyState;
        }
    }
}
