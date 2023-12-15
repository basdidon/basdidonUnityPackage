using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Direction
{
    public enum Directions
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }

    [Flags]
    public enum DirectionsGroup
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Cardinal = 15, // Left,Right,Up,Down 
    };
    /*
    public static class GridDirection
    {
        public static Vector3Int[] CardinalVector => new Vector3Int[] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };
        
        // Comparer
        public static bool IsDirectionInGroup(Direction direction,DirectionGroup group)
        {
            // if direction == Direction.None return false
            if(((byte)direction & (byte)group) != 0)
            {
                return true;
            }
            return false;
        }

        // Convertor
        public static Direction Vector3IntToDirection(Vector3Int directionVector)
        {
            if (directionVector == Vector3Int.zero)
                return Direction.None;

            if (directionVector.x * directionVector.y != 0)
            {
                return Direction.None;
            }

            if(directionVector.x < 0)
            {
                return Direction.Left;
            }
            else if (directionVector.x > 0)
            {
                return Direction.Right;
            }
            else if(directionVector.y < 0)
            {
                return Direction.Down;
            }
            else if (directionVector.y > 0)
            {
                return Direction.Up;
            }

            return Direction.None;
            
        }

        public static Vector3Int DirectionToVector3Int(Direction direction)
        {
            if ((direction & Direction.Left) != 0)
            {
                return Vector3Int.left;
            }
            else if ((direction & Direction.Right) != 0)
            {
                return Vector3Int.right;
            }
            else if ((direction & Direction.Up) != 0)
            {
                return Vector3Int.up;
            }
            else if ((direction & Direction.Down) != 0)
            {
                return Vector3Int.down;
            }

            return Vector3Int.zero;
        }

        public static List<Vector3Int> DirectionGroupToVector3Ints(DirectionGroup directions)
        {
            List<Vector3Int> vecs = new();
            
            if ((directions & DirectionGroup.Left) != 0)
            {
                vecs.Add(Vector3Int.left);
            }

            if((directions&DirectionGroup.Right)!= 0)
            {
                vecs.Add(Vector3Int.right);
            }

            if ((directions & DirectionGroup.Up) != 0)
            {
                vecs.Add(Vector3Int.up);
            }

            if ((directions & DirectionGroup.Down) != 0)
            {
                vecs.Add(Vector3Int.down);
            }

            return vecs;
        }

        public static List<Direction> GroupToSingleDirections(DirectionGroup directions)
        {
            List<Direction> extracted = new();

            if ((directions & DirectionGroup.Left) != 0)
            {
                extracted.Add(Direction.Left);
            }

            if ((directions & DirectionGroup.Right) != 0)
            {
                extracted.Add(Direction.Right);
            }

            if ((directions & DirectionGroup.Up) != 0)
            {
                extracted.Add(Direction.Up);
            }

            if ((directions & DirectionGroup.Down) != 0)
            {
                extracted.Add(Direction.Down);
            }

            return extracted;
        }

        public static bool IsCellInDirection(Vector3Int pivot, Vector3Int cell, DirectionGroup dirs)
        {
            Vector3Int _dir = cell - pivot;

            if ((dirs & DirectionGroup.Left) != 0 && _dir.x < 0 && _dir.y == 0)
            {
                return true;
            }
            else if ((dirs & DirectionGroup.Right) != 0 && _dir.x > 0 && _dir.y == 0)
            {
                return true;
            }
            else if ((dirs & DirectionGroup.Up) != 0 && _dir.x == 0 && _dir.y > 0)
            {
                return true;
            }
            else if ((dirs & DirectionGroup.Down) != 0 && _dir.x == 0 && _dir.y < 0)
            {
                return true;
            }

            return false;
        }

        public static void AddDirectionToDirectionGroup(ref DirectionGroup group, Direction direction)
        {
            // Perform a bitwise OR operation to add the direction to the group
            int updatedGroup = (byte)group | (byte)direction;

            // cast value back to DirectionGroup and update group
            group = (DirectionGroup)updatedGroup;
        }

        public static void AddDirectionVectorToDirectionGroup(ref DirectionGroup group, Vector3Int dirVec)
        {
            var dir = Vector3IntToDirection(dirVec);
            AddDirectionToDirectionGroup(ref group, dir);
        }
    }*/

    public class Direction
    {
        public Directions Value { get; private set; }

        #region Constructor
        public Direction(Directions directions)
        {
            Value = directions;
        }

        public Direction(Vector3Int directionVector)
        {
            if (directionVector == Vector3Int.zero)
                Value = Directions.None;

            if (directionVector.x * directionVector.y != 0)
                Value = Directions.None;

            if (directionVector.x < 0)
            {
                Value = Directions.Left;
            }
            else if (directionVector.x > 0)
            {
                Value = Directions.Right;
            }
            else if (directionVector.y < 0)
            {
                Value = Directions.Down;
            }
            else if (directionVector.y > 0)
            {
                Value = Directions.Up;
            }
        }
        #endregion

        public Vector3Int ToVector3Int()
        {
            if ((Value & Directions.Left) != 0)
            {
                return Vector3Int.left;
            }
            else if ((Value & Directions.Right) != 0)
            {
                return Vector3Int.right;
            }
            else if ((Value & Directions.Up) != 0)
            {
                return Vector3Int.up;
            }
            else if ((Value & Directions.Down) != 0)
            {
                return Vector3Int.down;
            }

            return Vector3Int.zero;
        }

        // (byte)
        public static implicit operator Direction(byte code)
        {
            if (!Enum.IsDefined(typeof(Directions),code))
                return Directions.None;
            
            return new((Directions)code);  // may be cause error
        }
        public static explicit operator byte(Direction direction) => (byte) direction.Value;
        // (Directions)
        public static implicit operator Direction(Directions directions) => new(directions);
        public static explicit operator Directions(Direction direction) => direction.Value;
        // (Vector3Int)
        public static implicit operator Direction(Vector3Int vector) => new(vector);
        public static explicit operator Vector3Int(Direction direction) => direction.ToVector3Int();
    }

    public class DirectionGroup
    {
        public DirectionsGroup Value { get; private set; }

        public DirectionGroup(DirectionsGroup directionsGroup)
        {
            Value = directionsGroup;
        }

        public DirectionGroup(IEnumerable<Directions> directions)
        {
            byte flagValue = 0;

            foreach(var dir in directions)
            {
                flagValue |= (byte)dir;
            }

            Value = (DirectionsGroup) flagValue;
        }

        public DirectionGroup(IEnumerable<Direction> directions)
        {
            byte flagValue = 0;

            foreach(var dir in directions)
            {
                flagValue |= (byte) dir;
            }

            Value = (DirectionsGroup) flagValue;
        }

        public DirectionGroup(IEnumerable<Vector3Int> dirVecs)
        {
            byte flagValue = 0;

            foreach (var dirVec in dirVecs)
            {
                flagValue |= (byte)(new Direction(dirVec));
            }

            Value = (DirectionsGroup)flagValue;
        }

        // Comparer
        public static bool IsContains(Direction direction)
        {
            // if direction == Direction.None return false
            if (((byte)direction & (byte)Value) != 0)
            {
                return true;
            }
            return false;
        }
    }
}