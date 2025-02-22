using System;
using System.Numerics;

public struct Vector2i
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2i(Vector2 vector2){
        X = (int)vector2.X;
        Y = (int)vector2.Y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        Vector2i b = (Vector2i )obj;

        return X == b.X && Y == b.Y;
    }


    public static  bool operator == ( Vector2i a, Vector2i b){
        return a.Equals(b);
    }

    public static  bool operator != ( Vector2i a, Vector2i b){

        return a.X != b.X || a.Y != b.Y;
    }

    public readonly bool Equals(Vector2 other){
        return X == other.X && Y == other.Y;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.X - b.X, a.Y - b.Y);
    }
    public static Vector2i operator -(Vector2i a)
    {
        return new Vector2i(-a.X, -a.Y);
    }

    public static Vector2i operator *(Vector2i a, int scalar)
    {
        return new Vector2i(a.X * scalar, a.Y * scalar);
    }

    public static Vector2i operator *(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.X * b.X, a.Y * b.Y);
    }

    public static Vector2i operator /(Vector2i a, int scalar)
    {
        return new Vector2i(a.X / scalar, a.Y / scalar);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}