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
        public static Vector3 WorldTranslate;

        public AStarPathfinder(string inMapFilePath)
        {
            Path = new List<Vector3>();
            grid = new Graph(inMapFilePath);
        }

        public void ResetPath() { Path = new List<Vector3>(); }

        public bool IsOnPath() { return Path != null && Path.Count != 0; }

        public bool IsOnNode(Vector3 position)
        {
            Vector3 undoneWorld = position - WorldTranslate;
            Vector2 converted2D = new Vector2((float)Math.Round(undoneWorld.X), (float)Math.Round(undoneWorld.Z));
            return VectorToNode(converted2D) != -1;
        }

        public Vector3 GetClosestNode(Vector3 position)
        {
            Vector3 undoneWorld = position - WorldTranslate;
            Vector2 converted2D = new Vector2((float)Math.Round(undoneWorld.X), (float)Math.Round(undoneWorld.Z));
            Vector2 closest2D = grid.ClosestNodePositionToTarget(converted2D);
            return new Vector3(closest2D.X, 1.0f, closest2D.Y);
        }

        public void GenerateRandomPath(Vector3 startPos, Random rnd)
        {
            GeneratePath(startPos, GetRandomNode(rnd));
        }

        private Vector3 GetRandomNode(Random rnd)
        {
            int nodeToGet = rnd.Next(0, grid.GridSize);
            Vector2 node2DPos = grid.grid[nodeToGet].Position;
            return new Vector3(node2DPos.X + WorldTranslate.X, 1.0f, node2DPos.Y + WorldTranslate.X);
        }

        public void GeneratePath(Vector3 startPos, Vector3 targetPos)
        {
            Vector2 start2D = new Vector2(startPos.X - WorldTranslate.X, startPos.Z - WorldTranslate.Z);
            start2D.X = (float)Math.Round(start2D.X);
            start2D.Y = (float)Math.Round(start2D.Y);
            Vector2 target2D = new Vector2(targetPos.X - WorldTranslate.X, targetPos.Z - WorldTranslate.Z);
            target2D = grid.ClosestNodePositionToTarget(target2D);
            Dictionary<int, double> actualDistance = new Dictionary<int, double>();
            Dictionary<int, double> predictedDistance = new Dictionary<int, double>();
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            List<int> openSet = new List<int>();
            List<int> closedSet = new List<int>();

            int startNode = VectorToNode(start2D);
            int targetNode = VectorToNode(target2D);

            openSet.Add(startNode);

            for (int i = 0; i <= grid.MatrixSize; i++)
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
                for (int neighbour = 0; neighbour <= grid.MatrixSize; neighbour++)
                {
                    if (grid.AdjacencyMatrix[currentNode,neighbour] != 0)
                    {
                        double tempDist = actualDistance[currentNode] + grid.AdjacencyMatrix[currentNode, neighbour];
                        if (actualDistance[neighbour] <= tempDist)
                        {
                            continue;
                        }

                        if (closedSet.Contains(neighbour) && tempDist >= actualDistance[neighbour])
                        {
                            continue;
                        }
                        if (!closedSet.Contains(neighbour) || tempDist < actualDistance[neighbour])
                        {
                            actualDistance[neighbour] = tempDist;
                            predictedDistance[neighbour] = actualDistance[neighbour] + Vector2.Distance(NodeToVector(neighbour), target2D);
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
            Path = new List<Vector3>(); //No path found

        }

        private List<Vector3> RebuildPath(Dictionary<int, int> cameFrom, int currentNode)
        {
            if (!cameFrom.Keys.Contains(currentNode))
            {
                return new List<Vector3> { };
            }

            List<Vector3> path = RebuildPath(cameFrom, cameFrom[currentNode]);
            Vector2 v = NodeToVector(currentNode);
            path.Add(new Vector3(v.X + WorldTranslate.X, 1.0f, v.Y + WorldTranslate.Z));
            return path;
        }

        private Vector2 NodeToVector(int node)
        {
            for (int i = 0; i < grid.grid.Count; i++)
            {
                if (i == node)
                {
                    return grid.grid[i].Position;
                }
            }
            return Vector2.Zero;
        }

        private int VectorToNode(Vector2 inPos)
        {
            for (int i = 0; i < grid.grid.Count; i++)
            {
                if (grid.grid[i].Position == inPos)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
