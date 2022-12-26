using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;

namespace Shortest_paths_among_obstacles
{
    internal class StatusSegment:IComparable
    {
        private static Vector2 center = new Vector2(0, 0);
        private static float angle = 0;
        public static Vector2 nextPoint = new Vector2(1, 0);
        public PolygonPoint start { get; set; }
        public PolygonPoint end { get; set; }
        private static void setNextPoint()
        {
            float x = MathF.Cos(angle);
            float y = MathF.Sin(angle);
            if (MathF.Abs(x) == 1) y = 0;
            if (MathF.Abs(y) == 1) x = 0;
            nextPoint = new Vector2(x, y)+center;
        }

        public static Vector2 Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
                setNextPoint();
            }
        }
        public static float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                setNextPoint();
            }
        }
        
        public int CompareTo(object? obj)
        {
            if (!(obj is StatusSegment other)) return 0;
            if (other == null) return 1;
            if (this.start == other.start)
            {
                Vector2 tunit = this.start.Position + Vector2.Normalize(this.end.Position - this.start.Position);
                Vector2 ounit = other.start.Position + Vector2.Normalize(other.end.Position - other.start.Position);
                if (Vector2.Distance(tunit, center) < Vector2.Distance(ounit, center))
                {
                    return -1;
                }
                if (Vector2.Distance(tunit, center) > Vector2.Distance(ounit, center))
                {
                    return 1;
                }
                return 0;
            }
            Vector2? thisIntersection = Vector2Tools.GetIntersection(start.Position, end.Position, center, nextPoint);
            Vector2? otherIntersection = Vector2Tools.GetIntersection(other.start.Position, other.end.Position, center, nextPoint);
            if (thisIntersection == null) return -1;
            if (otherIntersection == null) return 1;
            if (Vector2.Distance((Vector2)thisIntersection, center) < Vector2.Distance((Vector2)otherIntersection, center))
            {
                return -1;
            }
            if (Vector2.Distance((Vector2)thisIntersection, center) > Vector2.Distance((Vector2)otherIntersection, center))
            {
                return 1;
            }
            return 0;
        }

        public StatusSegment(PolygonPoint start, PolygonPoint end, bool startingStatus = false)
        {
            if (startingStatus)
            {
                this.start = start;
                this.end = end;
                return;
            }
            Vector2 vs = start.Position - center;
            Vector2 ve = end.Position - center;

            float angleS= MathF.Atan2(vs.Y, vs.X);
            float angleE = MathF.Atan2(ve.Y, ve.X);
            if (angleS < 0)
            {
                angleS += 2 * MathF.PI;
            }
            if (angleE < 0)
            {
                angleE += 2 * MathF.PI;
            }
            if (angleS < angleE)
            {
                this.start = start;
                this.end = end;
            }
            else if(angleS > angleE)
            {
                this.start = end;
                this.end = start;
            }
            else if (vs.Length()>ve.Length())
            {
                this.start = end;
                this.end = start;
            }
            else
            {
                this.start = start;
                this.end = end;
            }
        }
    }
}
