using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BasDidon.PathFinder
{
    using BasDidon.Direction;
    // using A*

    public class PathTraced : List<Vector3Int>
    {
        public PathTraced() { }
        public PathTraced(IEnumerable<Vector3Int> collection)
        {
            AddRange(collection);
        }

        public List<Vector3Int> ToWayPoint()
        {
            List<Vector3Int> WayPoints = new();
            Directions currentDir = Directions.None;

            WayPoints.Add(this[0]);

            for (int i = 0; i < Count - 1; i++)
            {
                var newDir = Direction.Vector3IntToDirection(this[i + 1] - this[i]);

                Debug.Log($"{newDir}:{currentDir}");
                if (newDir == currentDir)
                {
                    WayPoints[^1] = this[i + 1];
                }
                else
                {
                    WayPoints.Add(this[i + 1]);
                    currentDir = newDir;
                }
            }

            Debug.Log("-------------------------");
            foreach (var wayPoint in WayPoints)
            {
                Debug.Log(wayPoint);
            }
            Debug.Log("-------------------------");

            return WayPoints;
        }
    }

    public interface IMoveable
    {
        public Vector3Int CellPos { get; }
        public bool CanMoveTo(Vector3Int cellPos);
        public bool TryMove(Vector3Int from, Directions direction, out Vector3Int moveResult);
    }

    public struct DirectionsToCell
    {
        public Vector3Int ResultCell { get; }
        public IReadOnlyList<Directions> Directions { get; }
        public int MoveUsed => Directions.Count;

        public DirectionsToCell(Vector3Int resultCell, IEnumerable<Directions> dirCollection)
        {
            ResultCell = resultCell;
            Directions = dirCollection.ToList();
        }
    }

    public class GridPathFinder
    {
        struct AStarNode
        {
            public Vector3Int CellPosition => Path.Last();

            PathTraced path;
            public IReadOnlyList<Vector3Int> Path => path;

            public int G { get; private set; }  // cumulative cost to this node, In this context mean ActionPoint
            public float H { get; private set; }  // cost to targetCell,that ignore all obstacles
            public float F => G + H;

            public AStarNode(Vector3Int cellPosition, int g, Vector3Int targetCell)
            {
                path = new PathTraced() { cellPosition };
                G = g;
                H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
            }

            public AStarNode(Vector3Int cellPosition, int g, Vector3Int targetCell, PathTraced _path)
            {
                path = _path;
                path.Add(cellPosition);
                G = g;
                H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
            }

            public void UpdateNode(PathTraced newPath, int g)
            {
                path = newPath;
                G = g;
            }
        }

        /// <param name="moveableObject"></param>
        /// <param name="startCell"></param>
        /// <param name="targetCell"></param>
        /// <param name="dirs">list of direction that can move</param>
        /// <param name="resultPath">list of every cell from <paramref name="startCell"/> to <paramref name="targetCell"/> </param>
        public static bool TryFindPath(IMoveable moveableObject, Vector3Int startCell, Vector3Int targetCell, Directions dirs, out PathTraced resultPath)
        {
            return TryFindPath(moveableObject.CanMoveTo, startCell, targetCell, dirs, out resultPath);
        }

        /// <param name="predicate"></param>
        /// <param name="startCell"></param>
        /// <param name="targetCell"></param>
        /// <param name="dirs">list of direction that can move</param>
        /// <param name="resultPath">list of every cell from <paramref name="startCell"/> to <paramref name="targetCell"/> </param>
        public static bool TryFindPath(Func<Vector3Int,bool> predicate, Vector3Int startCell, Vector3Int targetCell, Directions dirs, out PathTraced resultPath)
        {
            resultPath = new();

            if (!predicate(targetCell))
            {
                Debug.Log("target can't reach");
                return false;
            }
            else if (startCell == targetCell)
            {
                resultPath = new PathTraced() { startCell };
                return true;
            }

            AStarNode startNode = new(startCell, 0, targetCell);
            var toSearch = new List<AStarNode>() { startNode };
            var processed = new List<AStarNode>();

            while (toSearch.Count > 0)
            {
                AStarNode currentNode = toSearch[0];
                foreach (var t in toSearch)
                    if (t.F < currentNode.F || t.F == currentNode.F && t.H < currentNode.H)
                        currentNode = t;

                processed.Add(currentNode);
                toSearch.Remove(currentNode);

                foreach (var direction in Direction.DirectionsToVector3Ints(dirs))
                {
                    var nextPos = currentNode.CellPosition + direction;
                    if (predicate(nextPos))
                    {
                        var newPath = new PathTraced(currentNode.Path) { nextPos };

                        if (processed.Exists(cell => cell.CellPosition == nextPos))
                        {
                            var processedNode = processed.Find(cell => cell.CellPosition == nextPos);
                            // if new path use cost less than old node ,update that node
                            var g = currentNode.G + 1 < processedNode.G ? currentNode.G : processedNode.G;
                            processedNode.UpdateNode(newPath, g);
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
                            toSearch.Add(new AStarNode(currentNode.CellPosition + direction, currentNode.G + 1, targetCell, newPath));
                        }
                    }
                }
            }

            Debug.Log($"not found : ({processed.Count}) processed node");
            return false;
        }

        public static List<DirectionsToCell> PredictMoves(IMoveable moveableObject,int moveCount,Directions directions = Directions.Cardinal)
        {
            if (moveCount < 0)
                return null;

            List<DirectionsToCell> toSearch = new();
            List<DirectionsToCell> processed = new();
            var extractedDir = Direction.Extract(directions);

            foreach (var dir in extractedDir)
            {
                if (moveableObject.TryMove(moveableObject.CellPos, dir, out Vector3Int moveResult))
                {
                    toSearch.Add(new(moveResult, new List<Directions>() { dir }));
                }
            }

            while(toSearch.Count > 0)
            {
                var _cur = toSearch[0];
                toSearch.RemoveAt(0);

                if (_cur.MoveUsed > moveCount)
                    continue;

                foreach (var dir in extractedDir)
                {
                    if (moveableObject.TryMove(_cur.ResultCell,dir,out Vector3Int moveResult))
                    {
                        toSearch.Add(new(moveResult, _cur.Directions.Append(dir)));
                    }
                }

                processed.Add(_cur);
            }

            return processed;

        }
    }
}