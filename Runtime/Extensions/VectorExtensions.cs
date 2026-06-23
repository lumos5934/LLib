using UnityEngine;

namespace LLib
{
    public static class VectorExtensions
    {
        public static Vector3 SetX(this Vector3 vector, float value)
        {
            return new Vector3(value, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, value);
        }

        public static Vector3 ToVector3(this Vector2 vector, float z = 0f)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float Distance(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static Vector3Int SetX(this Vector3Int vector, int value)
        {
            return new Vector3Int(value, vector.y, vector.z);
        }

        public static Vector3Int SetY(this Vector3Int vector, int value)
        {
            return new Vector3Int(vector.x, value, vector.z);
        }

        public static Vector3Int SetZ(this Vector3Int vector, int value)
        {
            return new Vector3Int(vector.x, vector.y, value);
        }

        public static Vector3Int ToVector3Int(this Vector2Int vector, int z = 0)
        {
            return new Vector3Int(vector.x, vector.y, z);
        }

        public static float SqrDistance(this Vector3Int a, Vector3Int b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float Distance(this Vector3Int a, Vector3Int b)
        {
            return Vector3.Distance(a, b);
        }

        public static Vector2 SetX(this Vector2 vector, float value)
        {
            return new Vector2(value, vector.y);
        }

        public static Vector2 SetY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, value);
        }

        public static float SqrDistance(this Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float Distance(this Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }


        public static Vector2 DirectionToType(this Vector2 a, DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Left => Vector2.left,
                DirectionType.Right => Vector2.right,
                DirectionType.Up => Vector2.up,
                DirectionType.Down => Vector2.down,
                _ => Vector2.zero
            };
        }

        public static Vector2Int SetX(this Vector2Int vector, int value)
        {
            return new Vector2Int(value, vector.y);
        }

        public static Vector2Int SetY(this Vector2Int vector, int value)
        {
            return new Vector2Int(vector.x, value);
        }

        public static float SqrDistance(this Vector2Int a, Vector2Int b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float Distance(this Vector2Int a, Vector2Int b)
        {
            return Vector2.Distance(a, b);
        }
    }
}