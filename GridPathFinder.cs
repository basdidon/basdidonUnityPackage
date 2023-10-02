using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.PathFinder
{
    // using A*
    public class GridPathFinder
    {
        public interface IMoveable
        {
            public bool CanMoveTo(Vector3Int cellPos);
        }

        struct Node
        {
            public Vector3Int CellPosition { get; set; }
            public List<Vector3Int> Path { get; set; }
            public int G { get; set; }  // cumulative cost to this node, In this context mean ActionPoint
            public float H { get; set; }  // cost to targetCell,that ignore all obstacles
            public float F => G + H;

            public Node(Vector3Int cellPosition, int g, Vector3Int targetCell)
            {
                CellPosition = cellPosition;
                Path = new List<Vector3Int>() { cellPosition };
                G = g;
                H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
            }

            public Node(Vector3Int cellPosition, int g, Vector3Int targetCell, List<Vector3Int> path)
            {
                CellPosition = cellPosition;
                Path = path;
                G = g;
                H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
            }
        }

        /// <param name="moveableObject"></param>
        /// <param name="startCell"></param>
        /// <param name="targetCell"></param>
        /// <param name="dirs">list of direction that can move</param>
        /// <param name="resultPath">list of every cell from <paramref name="startCell"/> to <paramref name="targetCell"/> </param>
        public static bool TryFindPath(IMoveable moveableObject, Vector3Int startCell, Vector3Int targetCell, List<Vector3Int> dirs, out List<Vector3Int> resultPath)
        {
            resultPath = new();

            if (!moveableObject.CanMoveTo(targetCell))
            {
                Debug.Log("target can't reach");
                return false;
            }
            else if (startCell == targetCell)
            {
                resultPath = new List<Vector3Int>() { startCell };
                return true;
            }

            Node startNode = new(startCell, 0, targetCell);
            var toSearch = new List<Node>() { startNode };
            var processed = new List<Node>();

            while (toSearch.Count > 0)
            {
                Node currentNode = toSearch[0];
                foreach (var t in toSearch)
                    if (t.F < currentNode.F || t.F == currentNode.F && t.H < currentNode.H)
                        currentNode = t;

                processed.Add(currentNode);
                toSearch.Remove(currentNode);

                foreach (var direction in dirs)
                {
                    var nextPos = currentNode.CellPosition + direction;
                    if (moveableObject.CanMoveTo(nextPos))
                    {
                        var newPath = new List<Vector3Int>(currentNode.Path) { nextPos };

                        if (processed.Exists(cell => cell.CellPosition == nextPos))
                        {
                            var processedNode = processed.Find(cell => cell.CellPosition == nextPos);
                            // if new path use cost less than old node ,update that node
                            processedNode.G = currentNode.G + 1 < processedNode.G ? currentNode.G : processedNode.G;
                            processedNode.Path = newPath;
                        }
                        else
                        {
                            if (currentNode.CellPosition + direction == targetCell)
                            {
                                Debug.Log("found");
                                resultPath = newPath;
                                return true;
                            }
                            // add new node
                            toSearch.Add(new Node(currentNode.CellPosition + direction, currentNode.G + 1, targetCell, newPath));
                        }
                    }
                }
            }

            Debug.Log($"not found : ({processed.Count}) processed node");
            return false;
        }

        public static bool TryFindWaypoint(IMoveable moveableObject, Vector3Int startCell, Vector3Int targetCell, List<Vector3Int> dirs, out List<Vector3Int> resultWayPoints)
        {
            resultWayPoints = new List<Vector3Int>();

            if (TryFindPath(moveableObject, startCell, targetCell, dirs, out List<Vector3Int> resultPath))
            {
                Vector3Int currentDir = new();
                resultWayPoints.Add(resultPath[0]);

                for (int i = 0; i < resultPath.Count - 1; i++)
                {
                    var newDir = resultPath[i + 1] - resultPath[i];

                    if (newDir == currentDir)
                    {
                        resultWayPoints[^1] = resultPath[i + 1];
                    }
                    else
                    {
                        resultWayPoints.Add(resultPath[i + 1]);
                        currentDir = newDir;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
