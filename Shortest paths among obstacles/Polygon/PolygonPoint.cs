using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace Shortest_paths_among_obstacles
{
    internal class PolygonPoint
    {
        public Vector2 Position { get; set; }

        public PolygonPoint Previous { get; set; }
        public PolygonPoint Next { get; set; }

        public PolygonPoint(float X, float Y, PolygonPoint? Previous, PolygonPoint? Next)
        {
            this.Position = new Vector2(X, Y);
            if (Previous == null)
            {
                this.Previous = this;
            }
            else
            {
                this.Previous = Previous;
            }
            if (Next == null)
            {
                this.Next = this;
            }
            else
            {
                this.Next = Next;
            }
        }

        public PolygonPoint(Vector2 position, PolygonPoint? Previous, PolygonPoint? Next)
        {
            this.Position = position;
            if (Previous == null)
            {
                this.Previous = this;
            }
            else
            {
                this.Previous = Previous;
            }
            if (Next == null)
            {
                this.Next = this;
            }
            else
            {
                this.Next = Next;
            }
        }


    }
}
