using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
//using CSKicksCollection.Trees;

namespace Shortest_paths_among_obstacles
{
    internal class PathCalculator
    {
        public static List<Vector2> allVertices;
        public static Dictionary<Vector2, PolygonPoint> vertexPolygonPoints;
        private static Dictionary<Vector2, Dictionary<Vector2, float>> visibilityGraph;
        static PathCalculator()
        {
            allVertices = new List<Vector2>();
            vertexPolygonPoints = new Dictionary<Vector2, PolygonPoint>();
        }

        static bool intersectsXAxis(Vector2 upper, Vector2 lower, Vector2 start)
        {
            if (upper.Y < start.Y)
            {
                return false;
            }
            if (lower.Y > start.Y)
            {
                return false;
            }
            if (upper.Y == lower.Y)
            {
                return false;
            }
            if (upper.X == lower.X)
            {
                if (upper.X >= start.X)
                {
                    return true;
                }
                return false;
            }
            float k = (upper.Y - lower.Y) / (upper.X - lower.X);
            float n = upper.Y - k * upper.X;

            float py = start.Y;
            float px = (py - n) / k;
            if (px >= start.X)
            {
                return true;
            }
            return false;
        }
        public static List<Vector2> VisibleVertices(Vector2 start, List<Polygon> polygons)
        {
            List<Vector2> result = new List<Vector2>();
            allVertices.Sort((a, b) =>
            {
                Vector2 av = a - start;
                Vector2 bv = b - start;
                float angleA = MathF.Atan2(av.Y, av.X);
                float angleB = MathF.Atan2(bv.Y, bv.X);
                if (angleA < 0)
                {
                    angleA+=2*MathF.PI;
                }
                if (angleB < 0)
                {
                    angleB+=2*MathF.PI;
                }
                if (angleA > angleB)
                {
                    return 1;
                }  
                if(angleA < angleB)
                {
                    return -1;
                }
                if (Vector2.Distance(a, start) < Vector2.Distance(b, start))
                {
                    return -1;
                }
                if (Vector2.Distance(a, start) > Vector2.Distance(b, start))
                {
                    return 1;
                }
                return 0;
            });
            //Status
            SortedSet<StatusSegment> status = new SortedSet<StatusSegment>();
            //AVLTree<StatusSegment> status = new AVLTree<StatusSegment>();
            StatusSegment.Center = start;
            StatusSegment.Angle = 0;
            //Resetovanje segmenata
            foreach(PolygonPoint polygonPoint in vertexPolygonPoints.Values)
            {
                polygonPoint.Segments = new List<StatusSegment>();
            }
            //Pocetni status sweep linea
            foreach(Polygon polygon in polygons)
            {
                bool first = true;
                PolygonPoint current = polygon.Start;
                while (current != polygon.Start || first)
                {
                    first = false;

                    //Duz izmedju current i current.next, proverim da li se na pocetku nalazi u statusu, za sada izbegavam degenerisane
                    Vector2 upper = (current.Position.Y > current.Next.Position.Y) ? current.Position : current.Next.Position;
                    Vector2 lower = (current.Position.Y <= current.Next.Position.Y) ? current.Position : current.Next.Position;

                    if(intersectsXAxis(upper, lower, start) && upper != StatusSegment.Center && lower != StatusSegment.Center)
                    {
                        StatusSegment segment = new StatusSegment(vertexPolygonPoints[lower], vertexPolygonPoints[upper], true);
                        vertexPolygonPoints[upper].Segments.Add(segment);
                        status.Add(segment);
                    }

                    current = current.Next;
                }
            }

            //Za svaku tacku razresi dogadjaj (to su dogadjaji)

            Vector2? previous = null;
            foreach(Vector2 vertex in allVertices)
            {
                //Podesimo ugao
                Vector2 dv = vertex - start;
                float angleV = MathF.Atan2(dv.Y, dv.X);
                if (angleV < 0)
                {
                    angleV += 2 * MathF.PI;
                }
                foreach(StatusSegment segment in vertexPolygonPoints[vertex].Segments)
                {
                    status.Remove(segment);
                }
                StatusSegment.Angle = angleV;//Ugao vertexa u odnosu na start

                if (vertex!=start && Visible(vertex, status, previous, result)) //Dodaje ga ako je levo od najlevlje prave u statusu, (degenericne samo ne dodajemo u status)
                {
                    result.Add(vertex);//Ovo mozda treba da se uradi tek kasnije, kad sve bude izracunato
                }
                //Dodajemo u status u dva slucaja, prvi je kada je duz bila u inicijalnom statusu

                Vector2 upper = (vertex.Y > vertexPolygonPoints[vertex].Next.Position.Y) ? vertex : vertexPolygonPoints[vertex].Next.Position;
                Vector2 lower = (vertex.Y <= vertexPolygonPoints[vertex].Next.Position.Y) ? vertex : vertexPolygonPoints[vertex].Next.Position;
                if (lower == vertex && intersectsXAxis(upper, lower, start) && upper != StatusSegment.Center && lower != StatusSegment.Center)
                {
                    StatusSegment segment = new StatusSegment(vertexPolygonPoints[lower], vertexPolygonPoints[upper], true);
                    vertexPolygonPoints[upper].Segments.Add(segment);
                    status.Add(segment);
                }
                upper = (vertex.Y > vertexPolygonPoints[vertex].Previous.Position.Y) ? vertex : vertexPolygonPoints[vertex].Previous.Position;
                lower = (vertex.Y <= vertexPolygonPoints[vertex].Previous.Position.Y) ? vertex : vertexPolygonPoints[vertex].Previous.Position;
                if (lower == vertex && vertexPolygonPoints[vertex].Previous!= vertexPolygonPoints[vertex].Next && intersectsXAxis(upper, lower, start) && upper != StatusSegment.Center && lower != StatusSegment.Center)
                {
                    StatusSegment segment = new StatusSegment(vertexPolygonPoints[lower], vertexPolygonPoints[upper], true);
                    vertexPolygonPoints[upper].Segments.Add(segment);
                    status.Add(segment);
                }
                Vector2 dn = vertexPolygonPoints[vertex].Next.Position - start;
                float angleN = MathF.Atan2(dn.Y, dn.X);
                if (angleN < 0)
                {
                    angleN += 2 * MathF.PI;
                }

                upper = (vertex.Y > vertexPolygonPoints[vertex].Next.Position.Y) ? vertex : vertexPolygonPoints[vertex].Next.Position;
                lower = (vertex.Y <= vertexPolygonPoints[vertex].Next.Position.Y) ? vertex : vertexPolygonPoints[vertex].Next.Position;
                if (angleN > angleV && !intersectsXAxis(upper, lower, start) && vertex != StatusSegment.Center && vertexPolygonPoints[vertex].Next.Position != StatusSegment.Center)
                {
                    StatusSegment segment = new StatusSegment(vertexPolygonPoints[vertex], vertexPolygonPoints[vertex].Next, true);
                    vertexPolygonPoints[vertex].Next.Segments.Add(segment);
                    status.Add(segment);
                }
                Vector2 dp = vertexPolygonPoints[vertex].Previous.Position - start;
                float angleP = MathF.Atan2(dp.Y, dp.X);
                if (angleP < 0)
                {
                    angleP += 2 * MathF.PI;
                }

                upper = (vertex.Y > vertexPolygonPoints[vertex].Previous.Position.Y) ? vertex : vertexPolygonPoints[vertex].Previous.Position;
                lower = (vertex.Y <= vertexPolygonPoints[vertex].Previous.Position.Y) ? vertex : vertexPolygonPoints[vertex].Previous.Position;
                if (angleP > angleV && !intersectsXAxis(upper, lower, start) && vertex!=StatusSegment.Center && vertexPolygonPoints[vertex].Previous.Position!=StatusSegment.Center)
                {
                    StatusSegment segment = new StatusSegment(vertexPolygonPoints[vertex], vertexPolygonPoints[vertex].Previous, true);
                    vertexPolygonPoints[vertex].Previous.Segments.Add(segment);
                    status.Add(segment);
                }

                //Drugi slucaj, kada je ugao druge tacke veci od ugla prve
                previous = vertex;
            }

            return result;
        }
        public static bool Visible(Vector2 vertex, SortedSet<StatusSegment> status, Vector2? previous, List<Vector2> visibleVertices)
        {

            //Vidljiv je ako su komsije!!!
            //p je StatusSegment.Center
            //wi je vertex
            PolygonPoint vertexPolygonPoint = vertexPolygonPoints[vertex];
            PolygonPoint centerPolygonPoint = vertexPolygonPoints[StatusSegment.Center];
            if (vertexPolygonPoint.Polygon == centerPolygonPoint.Polygon)
            {
                if(vertexPolygonPoint!=centerPolygonPoint && (vertexPolygonPoint.Next==centerPolygonPoint || centerPolygonPoint.Next == vertexPolygonPoint))
                {
                    return true;
                }
            }
            if (vertexPolygonPoint != vertexPolygonPoint.Next)
            {
                //Stavimo v u centar, vidimo da li je c izmedju vektora vn i vp, u smeru suprotnom od kazaljke na satu
                if (Vector2Tools.IsPointBetweenVectorsCounterclockwise(vertexPolygonPoint.Next.Position - vertex, vertexPolygonPoint.Previous.Position - vertex, StatusSegment.Center - vertex))
                {
                    return false;
                }
                if (Vector2Tools.IsPointBetweenVectorsCounterclockwise(centerPolygonPoint.Next.Position - StatusSegment.Center, centerPolygonPoint.Previous.Position - StatusSegment.Center, vertex- StatusSegment.Center))
                {
                    return false;
                }
                /*if (Vector2Tools.IsPointToRightOfVector(vertexPolygonPoint.Next.Position, vertexPolygonPoint.Position, StatusSegment.Center)
                    && Vector2Tools.IsPointToRightOfVector(vertexPolygonPoint.Position, vertexPolygonPoint.Previous.Position, StatusSegment.Center))
                {
                    return false;
                }
                if (Vector2Tools.IsPointToRightOfVector(centerPolygonPoint.Next.Position, centerPolygonPoint.Position, vertex)
                    && Vector2Tools.IsPointToRightOfVector(centerPolygonPoint.Position, centerPolygonPoint.Previous.Position, vertex))
                {
                    return false;
                }*/
            }
            //IsPointOnSegment ne radi za duz (111, 262), (291, 153) i tacku (316, 306)
            if(previous==null || previous==StatusSegment.Center || !Vector2Tools.IsPointOnSegment(vertex, StatusSegment.Center, (Vector2)previous))
            {
                StatusSegment? e = status.Min();
                if (e != null && Vector2Tools.SegmentsIntersect(vertex, StatusSegment.Center, e.start.Position, e.end.Position))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (visibleVertices.Count==0 || visibleVertices.Last() != previous)
            {
                return false;
            }
            //Mogu binarnu?
            //AVLTreeNode<StatusSegment> current = status.Root;
            if (status.Min == null) return true;
            if (Vector2Tools.SegmentsIntersect(status.Min.start.Position, status.Min.end.Position, vertex, StatusSegment.Center))
                return false;
            /*while (current != null)
            {
                if(Vector2Tools.SegmentsIntersect(statusSegment.start.Position, statusSegment.end.Position, vertex, StatusSegment.Center))
                {
                    return
                }
            }*/
            /*foreach(StatusSegment statusSegment in status)
            {

                if (Vector2Tools.SegmentsIntersect(statusSegment.start.Position, statusSegment.end.Position, vertex, StatusSegment.Center))
                    return false;
            }*/
            //Pretraga u statusu?
            return true;
            /*//Ako su u istom poligonu i nesusedne, nije vidljiva
            if(vertex==StatusSegment.Center) return false;
            if (vertexPolygonPoints[vertex].Polygon == vertexPolygonPoints[StatusSegment.Center].Polygon)
            {
                if(vertexPolygonPoints[vertex].Next == vertexPolygonPoints[StatusSegment.Center] ||
                    vertexPolygonPoints[vertex].Previous == vertexPolygonPoints[StatusSegment.Center])
                {
                    return true;
                }
                return false;
            }
            if (status.Count == 0) return true;
            StatusSegment? leftmost = status.Min;

            Vector2? intersection = Vector2Tools.GetIntersection(StatusSegment.Center, StatusSegment.nextPoint, leftmost.start.Position, leftmost.end.Position);

            if(intersection== null) return true;

            if(Vector2.Distance(StatusSegment.Center, (Vector2)intersection) == 0)
            {
                status.Remove(leftmost);
                bool result = Visible(vertex, status);
                status.Add(leftmost);
                return result;
            }
            if(Vector2.Distance(StatusSegment.Center, vertex)<= Vector2.Distance(StatusSegment.Center, (Vector2)intersection))
                return true;

            return false;*/
        }
        public static void CalculateVisibilityGraph(List<Polygon> polygons)
        {
            visibilityGraph = new Dictionary<Vector2, Dictionary<Vector2, float>>();
            //Ovo ispod O(N)
            foreach (Polygon polygon in polygons)
            {
                foreach (Vector2 point in polygon.Vertices)
                {
                    visibilityGraph.Add(point, new Dictionary<Vector2, float>());
                }
            }

            foreach(Vector2 vector in visibilityGraph.Keys)
            {
                List<Vector2> visible = VisibleVertices(vector, polygons);
                foreach(Vector2 visibleVertex in visible)
                {
                    visibilityGraph[vector][visibleVertex] = Vector2.Distance(vector, visibleVertex);
                }
            }
        }
        public static List<Vector2> Dijkstra(Dictionary<Vector2, Dictionary<Vector2, float>> graph, Vector2 start, Vector2 goal)
        {
            HashSet<Vector2> visited = new HashSet<Vector2>();
            //visited.Add(start);
            Dictionary<Vector2, KeyValuePair<Vector2, float>> distances = new Dictionary<Vector2, KeyValuePair<Vector2, float>>();
            distances.Add(start, new KeyValuePair<Vector2, float>(start, 0));
            while (visited.Count != graph.Count)
            {
                KeyValuePair<Vector2, float> closestUnvisited = new KeyValuePair<Vector2, float>(goal, float.MaxValue);
                foreach (Vector2 node in distances.Keys) //Bolja struktura za log n
                {
                    if (distances[node].Value < closestUnvisited.Value && !visited.Contains(node))
                    {
                        closestUnvisited = new KeyValuePair<Vector2, float>(node, distances[node].Value);
                    }
                }
                if (closestUnvisited.Key == goal)
                {
                    //Bektrek
                    List<Vector2> result = new List<Vector2>();
                    Vector2 current = goal;
                    while (current != start)
                    {
                        result.Add(current);
                        current = distances[current].Key;
                    }
                    result.Add(start);
                    result.Reverse();
                    return result;
                }
                visited.Add(closestUnvisited.Key);
                foreach(Vector2 key in graph[closestUnvisited.Key].Keys)
                {
                    if (key == closestUnvisited.Key) continue;
                    if (visited.Contains(key)) continue;
                    if(!distances.ContainsKey(key) || distances[closestUnvisited.Key].Value + graph[closestUnvisited.Key][key] < distances[key].Value)
                    {
                        //Na key stavljamo ...
                        distances[key] = new KeyValuePair<Vector2, float>(closestUnvisited.Key, distances[closestUnvisited.Key].Value + graph[closestUnvisited.Key][key]);
                        
                    }
                }
            }
            throw new NotImplementedException();
        }
        public static List<Vector2> CalculatePath(List<Polygon> polygons, Vector2 p, Vector2 q, Graphics? g = null)
        {
            List<Polygon> newPolygons = new List<Polygon>(polygons);

            newPolygons.Add(new Polygon(p));
            newPolygons.Add(new Polygon(q));

            allVertices = new List<Vector2>();
            vertexPolygonPoints = new Dictionary<Vector2, PolygonPoint>();
            foreach (Polygon polygon in newPolygons)
            {
                foreach(PolygonPoint point in polygon.Points)
                {
                    allVertices.Add(point.Position);
                    vertexPolygonPoints.Add(point.Position, point);
                }
            }

            CalculateVisibilityGraph(newPolygons);
            if (g != null)
            {
                foreach(Vector2 source in visibilityGraph.Keys)
                {
                    foreach(Vector2 destination in visibilityGraph[source].Keys)
                    {
                        if (!visibilityGraph[destination].ContainsKey(source) || visibilityGraph[destination][source]!= visibilityGraph[source][destination])
                        {
                            MessageBox.Show("LOSE");
                        }
                        //g.DrawLine(Pens.Green, new Point((int)source.X, (int)source.Y), new Point((int)destination.X, (int)destination.Y));
                    }               
                }
            }

            return Dijkstra(visibilityGraph, p, q);
        }

    }
}
