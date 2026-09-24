using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    public enum Direction
    {
        UP = 0,
        RIGHT = 90,
        DOWN = 180,
        LEFT = 270,
    }

    public static class DirectionExtensions
    {
        public static Point ToVector(this Direction direction)
        {
            return direction switch
            {
                Direction.UP => (0, -1),
                Direction.DOWN => (0, 1),
                Direction.LEFT => (-1, 0),
                Direction.RIGHT => (1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Not expected direction value: {direction}")
            };
        }

        public static Direction ToDirection(this Point p) // extends the point class
        {
            return p switch
            {
                { X: 0, Y: -1 } => Direction.UP,
                { X: 0, Y: 1 } => Direction.DOWN,
                { X: 1, Y: 0 } => Direction.LEFT, // swapped for some reason
                { X: -1, Y: 0 } => Direction.RIGHT,
                _ => throw new ArgumentOutOfRangeException(nameof(p), $"Not expected point value: {p}")
            };
        }

    }
}
