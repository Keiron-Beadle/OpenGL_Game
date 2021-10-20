using OpenGL_Game.Managers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Pathing
{
    class Graph
    {
        public int[,] grid;
        public double[,] AdjacencyMatrix;
        public int GridSize => grid.GetUpperBound(0);
        public int MatrixSize => AdjacencyMatrix.GetUpperBound(0);

        public Graph(string inMapFilePath)
        {
            grid = ScriptManager.LoadMap(inMapFilePath);
            int size = grid.GetUpperBound(0) * grid.GetUpperBound(1);
            AdjacencyMatrix = new double[size, size];
            ConstructMatrix();
        }

        private void ConstructMatrix()
        {
            int row = 0, col = 0;
            for (int i = 0; i < grid.GetUpperBound(0) * grid.GetUpperBound(1); i++)
            {
                //Left
                int v_adj = i - 1;
                if (ValidPosition(col-1,row))
                {
                    AdjacencyMatrix[i, v_adj] = 1;
                }

                //Right
                v_adj = i + 1;
                if (ValidPosition(col + 1, row))
                {
                    AdjacencyMatrix[i, v_adj] = 1;
                }
                //Up
                v_adj = i + GridSize;
                if (ValidPosition(col , row+1))
                {
                    AdjacencyMatrix[i, v_adj] = 1;
                }
                //Down
                v_adj = i - GridSize;
                if (ValidPosition(col, row - 1))
                {
                    AdjacencyMatrix[i, v_adj] = 1;
                }
                //LeftUP
                v_adj = i - GridSize - 1;
                if (ValidPosition(col - 1, row - 1))
                {
                    AdjacencyMatrix[i, v_adj] = 1.414;
                }

                //LeftDown
                v_adj = i + GridSize -1;
                if (ValidPosition(col - 1, row + 1))
                {
                    AdjacencyMatrix[i, v_adj] = 1.414;
                }
                //rightup
                v_adj = i - GridSize + 1;
                if (ValidPosition(col + 1, row - 1))
                {
                    AdjacencyMatrix[i, v_adj] = 1.414;
                }

                //rightdown
                v_adj = i + GridSize + 1;
                if (ValidPosition(col + 1, row + 1))
                {
                    AdjacencyMatrix[i, v_adj] = 1.414;
                }
                col++;
                if (col >= GridSize)
                {
                    col = 0;
                    row++;
                }

            }
        }

        private bool ValidPosition(int x, int y)
        {
            if (x < 0) return false;
            if (x >= GridSize) return false;
            if (y < 0) return false;
            if (y >= GridSize) return false;
            return grid[x, y] == 1;
        }

    }
}
