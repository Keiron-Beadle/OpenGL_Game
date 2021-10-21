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
            for (int i = 0; i < grid.Count; i++)
            {
                AdjacencyMatrix[i,0] = i;
                for (int j = 0; j < grid[i].neighbours.Count; j++)
                {
                    int indexOf = grid.IndexOf(grid[i].neighbours[j]);
                    AdjacencyMatrix[i, indexOf] = Distance(grid[i], grid[i].neighbours[j]);
                }
            }
        }

        public Vector2 ClosestNodePositionToTarget(Vector2 inPos)
        {
            Vector2 converted2D = new Vector2(inPos.X, inPos.Y);
            float dist = float.MaxValue;
            Vector2 closestPoint = Vector2.Zero;
            for (int i = 0; i < grid.Count; i++)
            {
                float currDist = Vector2.Distance(grid[i].Position, converted2D);
                if (currDist < dist)
                {
                    closestPoint = grid[i].Position;
                    dist = currDist;
                }
            }
            return closestPoint;
        }

        private double Distance(Node node1, Node node2)
        {
            return Vector2.Distance(node1.Position, node2.Position);
        }
    }
}
