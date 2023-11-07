using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.PathFinder
{
    using Direction;

    public interface IPredictMoveable : IBoardObject
    {
        public bool TryMove(Vector3Int from, Directions direction, out Vector3Int moveResult);
    }

    public static class PredictPathFinder
    {
        public static List<DirectionsToCell> PredictMoves(IPredictMoveable moveableObject, int moveCount, Directions directions = Directions.Cardinal)
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

            while (toSearch.Count > 0)
            {
                var _cur = toSearch[0];
                toSearch.RemoveAt(0);

                if (_cur.MoveUsed > moveCount)
                    continue;

                foreach (var dir in extractedDir)
                {
                    if (moveableObject.TryMove(_cur.ResultCell, dir, out Vector3Int moveResult))
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
