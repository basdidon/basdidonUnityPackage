using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Direction
{
    public enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }

    [Flags]
    public enum DirectionGroup
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Cardinal = 15, // Left,Right,Up,Down 
    };
    
    public static class Direction
    {
        public static Vector3Int[] CardinalVector => new Vector3Int[] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };
        
        // Comparer

        // Convertor
        public static Directions Vector3IntToDirection(Vector3Int directionVector)
        {
            if (directionVector == Vector3Int.left)
            {
                return Directions.Left;
            }
            else if (directionVector == Vector3Int.right)
            {
                return Directions.Right;
            }
            else if (directionVector == Vector3Int.up)
            {
                return Directions.Up;
            }
            else //(directionVector == Vector3Int.down)
            {
                return Directions.Down;
            }
        }

        public static Vector3Int DirectionToVector3Int(Directions direction)
        {
            if ((direction & Directions.Left) != 0)
            {
                return Vector3Int.left;
            }
            else if ((direction & Directions.Right) != 0)
            {
                return Vector3Int.right;
            }
            else if ((direction & Directions.Up) != 0)
            {
                return Vector3Int.up;
            }
            else if ((direction & Directions.Down) != 0)
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

        public static List<Directions> GroupToSingleDirections(DirectionGroup directions)
        {
            List<Directions> extracted = new();

            if ((directions & DirectionGroup.Left) != 0)
            {
                extracted.Add(Directions.Left);
            }

            if ((directions & DirectionGroup.Right) != 0)
            {
                extracted.Add(Directions.Right);
            }

            if ((directions & DirectionGroup.Up) != 0)
            {
                extracted.Add(Directions.Up);
            }

            if ((directions & DirectionGroup.Down) != 0)
            {
                extracted.Add(Directions.Down);
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
    }
}