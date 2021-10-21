using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameEngine.Pathing
{
    class AStarPathfinder
    {
        public List<Vector3> Path;
        Graph grid;
        Vector3 worldTrans;

        public AStarPathfinder(string inMapFilePath)
        {
            Path = new List<Vector3>();
            grid = new Graph("GameCode\\testmap.txt");
        }

        public void GeneratePath(Vector3 startPos, Vector3 targetPos, Vector3 worldTranslate)
        {
            worldTrans = worldTranslate;
            Vector2 start2D = new Vector2(startPos.X - worldTranslate.X, startPos.Z - worldTranslate.Z);
            Vector2 target2D = new Vector2(targetPos.X - worldTranslate.X, targetPos.Z - worldTranslate.Z);
            Dictionary<int, double> actualDistance = new Dictionary<int, double>();
            Dictionary<int, double> predictedDistance = new Dictionary<int, double>();
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            List<int> openSet = new List<int>();
            List<int> closedSet = new List<int>();

            int startNode = VectorToNode(start2D);
            int targetNode = VectorToNode(target2D);

            openSet.Add(startNode);

            for (int i = 0; i < grid.MatrixSize; i++)
            {
                if (i != startNode)
                {
                    actualDistance.Add(i, double.MaxValue);
                }
            }

            actualDistance.Add(startNode, 0);
            predictedDistance.Add(startNode, Vector2.Distance(start2D, target2D));

            while (openSet.Count > 0)
            {
                int currentNode = (from n in openSet orderby predictedDistance[n] ascending select n).First();
                if (currentNode == targetNode)
                {
                    Path = RebuildPath(cameFrom, currentNode);
                    return;
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                for (int neighbour = 0; neighbour < grid.MatrixSize; neighbour++)
                {
                    if (grid.AdjacencyMatrix[currentNode,neighbour] != 0)
                    {
                        double tempDist = actualDistance[currentNode] + grid.AdjacencyMatrix[currentNode, neighbour];
                        if (actualDistance[neighbour] <= tempDist)
                        {
                            continue;
                        }
                        actualDistance[neighbour] = tempDist;
                        predictedDistance[neighbour] = actualDistance[neighbour] + Vector2.Distance(NodeToVector(neighbour), target2D);
                        if (closedSet.Contains(neighbour) && tempDist >= actualDistance[neighbour])
                        {
                            continue;
                        }
                        if (!closedSet.Contains(neighbour) || tempDist < actualDistance[neighbour])
                        {
                            if (cameFrom.Keys.Contains(neighbour))
                            {
                                cameFrom[neighbour] = currentNode;
                            }
                            else
                            {
                                cameFrom.Add(neighbour, currentNode);
                            }

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }
            Path = null; //No path found

        }

        private List<Vector3> RebuildPath(Dictionary<int, int> cameFrom, int currentNode)
        {
            if (!cameFrom.Keys.Contains(currentNode))
            {
                return new List<Vector3>();
            }

            List<Vector3> path = RebuildPath(cameFrom, cameFrom[currentNode]);
            Vector2 v = NodeToVector(currentNode);
            path.Add(new Vector3(v.X + worldTrans.X, 1.0f, v.Y + worldTrans.Z));
            return path;
        }

        private Vector2 NodeToVector(int node)
        {
            return new Vector2(node % grid.GridSize, (int)Math.Floor((double)node / grid.GridSize));
        }

        private int VectorToNode(Vector2 inPos)
        {
            return (int)Math.Round(inPos.Y,0) * grid.GridSize + (int)Math.Round(inPos.X,0);
        }
    }
}
