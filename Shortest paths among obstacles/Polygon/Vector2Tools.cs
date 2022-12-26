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
        public static bool IsPointToRightOfVector(Vector2 start, Vector2 end, Vector2 point)
        {
            // Calculate the signed area of the parallelogram formed by the two input vectors
            float signedArea = (point.X - start.X) * (end.Y - start.Y) - (point.Y - start.Y) * (end.X - start.X);

            // Return true if the signed area is positive (point is to the right of the line), false otherwise
            return signedArea > 0;
        }

        public static Vector2 GetLineDirection(Vector2 point1, Vector2 point2)
        {
            return Vector2.Normalize(point2 - point1);
        }
        public static bool IsPointOnSegment(Vector2 point1, Vector2 point2, Vector2 point)
        {
            //Vector2 lineDirection = GetLineDirection(point1, point2);
            //Vector2 projection = ProjectPointOnLine(point1, lineDirection, point);
            return Vector2.Distance(point, point1) + Vector2.Distance(point, point2) == Vector2.Distance(point1, point2);
        }
        public static Vector2? GetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            double slope2, slope1, yIntercept1, yIntercept2, y, x;
            if (p1.X == p2.X && p3.X==p4.X)
            {
                //Mozda treba drugacije
                return null;
            }
            // Check if the first line is vertical
            if (p1.X == p2.X)
            {
                // Calculate the slope and y-intercept of the second line
                slope2 = (p4.Y - p3.Y) / (p4.X - p3.X);
                yIntercept2 = p3.Y - slope2 * p3.X;

                // Calculate the y-coordinate of the intersection point
                y = slope2 * p1.X + yIntercept2;

                // Return the intersection point as a Vector2 object
                return new Vector2(p1.X, (float)y);
            }

            // Check if the second line is vertical
            if (p3.X == p4.X)
            {
                // Calculate the slope and y-intercept of the first line
                slope1 = (p2.Y - p1.Y) / (p2.X - p1.X);
                yIntercept1 = p1.Y - slope1 * p1.X;

                // Calculate the y-coordinate of the intersection point
                y = slope1 * p3.X + yIntercept1;

                // Return the intersection point as a Vector2 object
                return new Vector2(p3.X, (float)y);
            }

            // Calculate the slope and y-intercept of the first line
            slope1 = (p2.Y - p1.Y) / (p2.X - p1.X);
            yIntercept1 = p1.Y - slope1 * p1.X;

            // Calculate the slope and y-intercept of the second line
            slope2 = (p4.Y - p3.Y) / (p4.X - p3.X);
            yIntercept2 = p3.Y - slope2 * p3.X;

            // Return null if the two lines have the same slope
            if (slope1 == slope2)
            {
                return null;
            }

            // Solve the system of equations formed by the two lines
            x = (yIntercept2 - yIntercept1) / (slope1 - slope2);
            y = slope1 * x + yIntercept1;

            // Create a new Vector2 object with the calculated x- and y-coordinates
            return new Vector2((float)x, (float)y);
        }
        public static bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4) return false;
            Vector2? intersection = GetIntersection(p1, p2, p3, p4);
            if (intersection == null) return false;
            if (intersection.Value.X > MathF.Max(p1.X, p2.X)) return false;
            if (intersection.Value.X > MathF.Max(p3.X, p4.X)) return false;
            if (intersection.Value.X < MathF.Min(p1.X, p2.X)) return false;
            if (intersection.Value.X < MathF.Min(p3.X, p4.X)) return false;
            if (intersection.Value.Y > MathF.Max(p1.Y, p2.Y)) return false;
            if (intersection.Value.Y > MathF.Max(p3.Y, p4.Y)) return false;
            if (intersection.Value.Y < MathF.Min(p1.Y, p2.Y)) return false;
            if (intersection.Value.Y < MathF.Min(p3.Y, p4.Y)) return false;
            return true;
            //Da li je presek na duzima?
        }
        public static bool IsPointBetweenVectorsCounterclockwise(Vector2 v1, Vector2 v2, Vector2 p)
        {
            float angle1 = MathF.Atan2(v1.Y, v1.X);
            float angle2 = MathF.Atan2(v2.Y, v2.X);
            float angleP = MathF.Atan2(p.Y, p.X);
            if (angle1 < 0)
            {
                angle1 += 2 * MathF.PI;
            }
            if (angle2 < 0)
            {
                angle2 += 2 * MathF.PI;
            }
            if (angleP < 0)
            {
                angleP += 2 * MathF.PI;
            }
            if (angle2 > angle1)
            {
                return (angleP>angle1) && (angleP<angle2);
            }
            if (angle2 < angle1)
            {
                return (angleP > angle1) || (angleP < angle2);
            }
            return false;
        }
    }
}
