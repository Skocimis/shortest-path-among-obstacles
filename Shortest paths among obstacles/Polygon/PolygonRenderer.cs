using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Shortest_paths_among_obstacles
{
    internal class PolygonRenderer
    {
        public static void DrawPoint(Vector2 position, Graphics graphics, int radius = 4)
        {
            graphics.FillEllipse(Brushes.Black, position.X - radius/2, position.Y - radius / 2, radius, radius);
        }
        public static void DrawPoint(Vector2 position, Graphics graphics, Color color, int radius = 4)
        {
            graphics.FillEllipse(new SolidBrush(color), position.X - radius / 2, position.Y - radius / 2, radius, radius);
        }
        public static void DrawPolygon(Polygon polygon, Graphics graphics, int width, int height)
        {
            PolygonPoint start = polygon.Start;
            List<Vector2> vertices = polygon.Vectices;
            PointF[] points = vertices.Select(v=> new PointF(v.X, v.Y)).ToArray();
            graphics.FillPolygon(Brushes.Yellow, points);
            bool first = true;
            PolygonPoint current = start;
            while (current != start || first)
            {
                first = false;
                //Crtam duz
                graphics.DrawLine(Pens.Black, new Point((int)current.Position.X, (int)current.Position.Y), new Point((int)current.Next.Position.X, (int)current.Next.Position.Y));
                //Crtam tacku na curent
                DrawPoint(current.Position, graphics, Color.Blue, 6);
                current = current.Next;
            }
            DrawPoint(start.Position, graphics, Color.Green, 6);
        }
    }
}
