using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BasDidon.PathFinder
{
    using Direction;

    public interface IPredictMoveable : IBoardObject
    {
        public bool TryMove(Vector3Int from, DirectionGroup direction, out Vector3Int moveResult);
    }

    public static class PredictPathFinder
    {
        public static List<DirectionsToCell> PredictMoves(IPredictMoveable moveableObject, int moveCount, DirectionGroup directions = DirectionGroup.Cardinal)
        {
            if (moveCount < 0)
                return null;

            List<DirectionsToCell> toSearch = new();
            List<DirectionsToCell> processed = new();
            var extractedDir = Direction.GroupToSingleDirections(directions);

            foreach (var dir in extractedDir)
            {
                if (moveableObject.TryMove(moveableObject.CellPos, dir, out Vector3Int moveResult))
                {
                    toSearch.Add(new(moveResult, new List<DirectionGroup>() { dir }));
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
