using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasDidon.PathFinder2D
{
    public interface IPathMap
    {
        bool CanMoveTo(Vector2Int from, Vector2Int to);
    }

    struct Node
    {
        public Vector2Int CellPosition { get; set; }
        public List<Vector2Int> Path { get; set; }
        public int G { get; set; }      // cumulative cost to this node
        public float H { get; set; }    // cost to targetCell,that ignore all obstacles
        public float F => G + H;

        public Node(Vector2Int cellPosition, int g, Vector2Int targetCell)
        {
            CellPosition = cellPosition;
            Path = new List<Vector2Int>() { cellPosition };
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }

        public Node(Vector2Int cellPosition, int g, Vector2Int targetCell, List<Vector2Int> path)
        {
            CellPosition = cellPosition;
            Path = path;
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }
    }

    public static class PathFinder
    {
        public static bool TryFindPath(IPathMap pathMap, Vector2Int startCell, Vector2Int targetCell, List<Vector2Int> dirs, out List<Vector2Int> resultPath)
        {
            resultPath = new();

            if(startCell == targetCell)
                return false;

            Node startNode = new(startCell, 0, targetCell);
            var toSearch = new List<Node> { startNode };
            var processed = new List<Node>();


            while (toSearch.Count > 0)
            {
                Node currentNode = toSearch[0]; //default currentNode
                toSearch.ForEach(node =>
                {
                    if (node.F < currentNode.F || node.F == currentNode.F && node.H < currentNode.H)
                        currentNode = node;
                });

                processed.Add(currentNode);
                toSearch.Remove(currentNode);

                foreach (var direction in dirs)
                {
                    var nextPos = currentNode.CellPosition + direction;
                    if (pathMap.CanMoveTo(currentNode.CellPosition, nextPos))
                    {
                        var newPath = new List<Vector2Int>(currentNode.Path) { nextPos };

                        if (processed.Exists(cell => cell.CellPosition == nextPos))
                        {
                            // Update old Node
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


            return false;
        }
    }


}
