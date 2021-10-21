using OpenGL_Game.Managers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Pathing
{
    public struct Node
    {
        public Vector2 Position;
        public List<Node> neighbours;
    }

    class Graph
    {
        public List<Node> grid;
        public double[,] AdjacencyMatrix;
        public int GridSize => grid.Count;
        public int MatrixSize => AdjacencyMatrix.GetUpperBound(0);

        public Graph(string inMapFilePath)
        {
            grid = ScriptManager.LoadMap(inMapFilePath);
            AdjacencyMatrix = new double[GridSize, GridSize];
            ConstructMatrix();
        }

        private void ConstructMatrix()
        {

        }
    }
}
