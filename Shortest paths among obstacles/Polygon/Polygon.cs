using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Shortest_paths_among_obstacles
{
    internal class Polygon
    {
        public HashSet<PolygonPoint> Points { get; private set; }
        public PolygonPoint Start { get; private set; }
        public Polygon(Vector2 vector)
        {
            Points = new HashSet<PolygonPoint>();
            PolygonPoint point1 = new PolygonPoint(vector, null, null, this);
            point1.Next = point1;
            point1.Previous = point1;
            Start = point1; 
            Points.Add(point1);
        }
        public Polygon(float X1, float Y1, float X2, float Y2)
        {
            Points = new HashSet<PolygonPoint>();
            PolygonPoint point1 = new PolygonPoint(X1, Y1, null, null, this);
            PolygonPoint point2 = new PolygonPoint(X2, Y2, null, null, this);
            point1.Previous = point2;
            point1.Next = point2;
            point2.Previous = point1;
            point2.Next = point1;
            Start = point1;
            Points.Add(point1);
            Points.Add(point2);
        }
        public List<Vector2> Vertices
        {
            get { 
                List<Vector2> result = new List<Vector2>();
                PolygonPoint start = Start;
                bool first = true;
                PolygonPoint current = start;
                while (current != start || first)
                {
                    first = false;
                    result.Add(current.Position);
                    current = current.Next;
                }
                return result; 
            }
        }
        public PolygonPoint AddPoint(float X, float Y, PolygonPoint previous, PolygonPoint next)
        {
            PolygonPoint point = new PolygonPoint(X, Y, previous, next, this);
            previous.Next = point;
            next.Previous = point;
            Points.Add(point);
            return point;
        }
        public PolygonPoint AddPoint(Vector2 position, PolygonPoint previous, PolygonPoint next)
        {
            PolygonPoint point = new PolygonPoint(position, previous, next, this);
            previous.Next = point;
            next.Previous = point;
            Points.Add(point);
            return point;
        }


    }
}
