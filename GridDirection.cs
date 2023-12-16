using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace BasDidon.Direction
{
    public class Direction
    {
        public static Direction Left => new(Vector3Int.left);
        public static Direction Right => new(Vector3Int.right);
        public static Direction Up => new(Vector3Int.up);
        public static Direction Down => new(Vector3Int.down);

        public static IReadOnlyList<Direction> AllValue => new Direction[] { Left, Right, Up, Down };

        public Vector3Int DirectionVector { get; }

        #region Constructor
        Direction(Vector3Int dirVec)
        {
            DirectionVector = dirVec;
        }
        #endregion

        public static bool TryGetDirectionByDirectionVector(Vector3Int dirVec,out Direction direction)
        {
            if(AllValue.Any(v=>v.DirectionVector == dirVec))
            {
                direction = AllValue.First(v => v.DirectionVector == dirVec);
                return true;
            }

            direction = null;
            return true;
        }

        public bool IsCellInDirection(Vector3Int pivot, Vector3Int cell)
        {
            // Calculate the direction vector from pivot to cell
            var direction = cell - pivot;

            // Calculate the sum of direction components (x, y, z)
            var sumOfComponents = direction.x + direction.y + direction.z;

            // Check if DirectionVector is a unit vector in any direction
            // (assuming DirectionVector is a known variable containing one axis vector)
            var scaledDirection = Vector3Int.Scale(direction, DirectionVector);
            var scaledSumOfComponents = scaledDirection.x + scaledDirection.y + scaledDirection.z;

            // Check if the scaled sum of direction components is positive and the absolute sum equals the scaled sum
            return scaledSumOfComponents > 0 && MathF.Abs(sumOfComponents) == scaledSumOfComponents;
        }

        public override bool Equals(object obj)
        {
            if (obj is Direction otherDirection)
            {
                return DirectionVector== otherDirection.DirectionVector;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DirectionGroup:ICollection<Direction>
    {
        List<Direction> directions;
        public IReadOnlyList<Vector3Int> DirectionVectors => directions.Select(d => d.DirectionVector).ToList();
        public int Count => directions.Count;
        public bool IsReadOnly => false;

        public static DirectionGroup CardinalDirection => new(){ Direction.Left, Direction.Right,Direction.Up,Direction.Down};


        public DirectionGroup()
        {
            directions = new();
        }

        public DirectionGroup(IEnumerable<Direction> directions)
        {
            this.directions = new(directions);
        }

        public static bool IsCellInDirectionGroup(Vector3Int pivot, Vector3Int cell, DirectionGroup dirs)
        {
           return dirs.Any(dir => dir.IsCellInDirection(pivot, cell));
        }

        #region ICollection implements
        public void Add(Direction item)
        {
            if (directions.Contains(item))
                return;
            directions.Add(item);
        }

        public void Clear()
        {
            directions.Clear();
        }

        public bool Contains(Direction item)
        {
            return directions.Contains(item);
        }

        public void CopyTo(Direction[] array, int arrayIndex)
        {
            directions.CopyTo(array,arrayIndex);
        }

        public bool Remove(Direction item)
        {
            return directions.Remove(item);
        }

        public IEnumerator<Direction> GetEnumerator()
        {
            return directions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}