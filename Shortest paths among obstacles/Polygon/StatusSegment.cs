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
        //Start je prva tacka dodata u status, end je krajnja
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
            //Degenerisani slucaj ako se nalazi na sweep lineu
            if (this.start == other.start)
            {
                //MessageBox.Show("ISTO " + this.start.Position.X.ToString() + ", " + other.start.Position.Y.ToString());
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
                /*float x0 = center.X;
                float y0 = center.Y;
                float tx1 = this.start.Position.X;
                float ty1 = this.start.Position.Y;
                float tx2 = this.end.Position.X;
                float ty2 = this.end.Position.Y;
                float ox1 = other.start.Position.X;
                float oy1 = other.start.Position.Y;
                float ox2 = other.end.Position.X;
                float oy2 = other.end.Position.Y;

                float td = Math.Abs((tx2 - tx1) * (ty1 - y0) - (tx1 - x0) * (ty2 - ty1)) / MathF.Sqrt(MathF.Pow(tx2 - tx1, 2) + MathF.Pow(ty2 - ty1, 2));
                float od = Math.Abs((ox2 - ox1) * (oy1 - y0) - (ox1 - x0) * (oy2 - oy1)) / MathF.Sqrt(MathF.Pow(ox2 - ox1, 2) + MathF.Pow(oy2 - oy1, 2));

                if (td < od)
                {
                    return -1;
                }
                if (td > od)
                    return 1;
                return 0;*/
                //Pre treba da 
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
            //Start je onaj sa manjim uglom, end je sa vecim
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
