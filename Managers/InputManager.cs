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
        }

        private void SaveControls()
        {
            XmlWriter writer = XmlWriter.Create("controls.xml");
            writer.WriteStartDocument();
            foreach (var pair in controlBindings)
            {
                writer.WriteStartElement("Config");
                writer.WriteAttributeString("Control", pair.Value.ToString());
                writer.WriteAttributeString("Key", pair.Key.ToString());
                writer.WriteEndElement();
            }
            writer.Close();
        }

        private void LoadControls()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("controls.xml");

            XmlNodeList nl = doc.SelectNodes("//");
            foreach (XmlNode n in nl)
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
