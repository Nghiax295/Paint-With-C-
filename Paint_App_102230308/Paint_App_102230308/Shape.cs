using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint_App_102230308
{
    public class Shape
    {
        public Rectangle GetRectangle(Point start, Point end)
        {
            return new Rectangle(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y)
            );
        }

        public Rectangle GetSquare(Point start, Point end)
        {
            int size = Math.Max(Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y));
            return new Rectangle(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                size,
                size
            );
        }

        public Point[] GetDiamondPoints(Point start, Point end)
        {
            int midX = start.X + (end.X - start.X) / 2;
            int midY = start.Y + (end.Y - start.Y) / 2;

            return new Point[]
            {
                new Point(midX, start.Y),
                new Point(end.X, midY),
                new Point(midX, end.Y),
                new Point(start.X, midY)
            };
        }

        public Point[] GetTrianglePoints(Point start, Point end)
        {
            return new Point[]
            {
                new Point(start.X + (end.X - start.X) / 2, start.Y),
                new Point(start.X, end.Y),
                new Point(end.X, end.Y)
            };
        }
    }
}
