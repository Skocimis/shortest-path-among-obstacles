using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Shortest_paths_among_obstacles
{
    internal class Vector2Tools
    {
        public static Vector2 ProjectPointOnLine(Vector2 linePoint, Vector2 lineDirection, Vector2 point)
        {
            float scale = Vector2.Dot(point - linePoint, lineDirection) / Vector2.Dot(lineDirection, lineDirection);
            return linePoint + scale * lineDirection;
        }

        public static Vector2 GetLineDirection(Vector2 point1, Vector2 point2)
        {
            return Vector2.Normalize(point2 - point1);
        }
        public static bool IsPointOnSegment(Vector2 point1, Vector2 point2, Vector2 point)
        {
            Vector2 lineDirection = GetLineDirection(point1, point2);
            Vector2 projection = ProjectPointOnLine(point1, lineDirection, point);
            return Vector2.Distance(projection, point1) + Vector2.Distance(projection, point2) == Vector2.Distance(point1, point2);
        }
    }
}
