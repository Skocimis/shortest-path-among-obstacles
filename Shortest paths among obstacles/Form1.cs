using System;
using System.Numerics;

namespace Shortest_paths_among_obstacles
{
    public partial class Form1 : Form
    {
        private List<Polygon> Polygons;
        private bool NewPolygon;
        private Point? NewPolygonPoint1;
        private Point? NewPolygonPoint2;
        private Point? MouseLocation;
        private Graphics g;
        private PolygonPoint? MovingPoint;
        private Polygon? MovingPointPolygon;
        private Vector2 start;
        private Vector2 finish;
        private bool movingStart;
        private bool movingFinish;


        public Form1()
        {
            DoubleBuffered = true;
            Polygons = new List<Polygon>();
            NewPolygon = false;
            NewPolygonPoint1 = null;
            NewPolygonPoint2 = null;
            MouseLocation = null;
            start = new Vector2(100, 100);
            finish = new Vector2(200, 200);
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            movingFinish = false;
            movingStart = false;
            render();//?
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Gledamo na sta je kliknuto
            //Provera da li je kliknuta tacka, ako jeste, treba da se pomera
            Vector2 P = new Vector2(e.X, e.Y);
            if (Vector2.Distance(P, start) < 10)
            {
                movingStart = true;
                return;
            }
            if (Vector2.Distance(P, finish) < 10)
            {
                movingFinish = true;
                return;
            }
            foreach (Polygon polygon in Polygons)
            {
                PolygonPoint start = polygon.Start;
                bool first = true;
                PolygonPoint current = start;
                while (current != start || first)
                {
                    first = false;
                    if (Vector2.Distance(current.Position, P) < 10)
                    {
                        MovingPoint = current;
                        MovingPointPolygon = polygon;
                        return;
                    }
                    current = current.Next;
                }
            }

            foreach(Polygon polygon in Polygons)
            {
                PolygonPoint start = polygon.Start;
                bool first = true;
                PolygonPoint current = start;
                while (current != start || first)
                {
                    first = false;
                    Vector2 A = current.Position;
                    Vector2 B = current.Next.Position;
                    Vector2 projection = Vector2Tools.ProjectPointOnLine(A, Vector2Tools.GetLineDirection(A, B), P);

                    //polygon.AddPoint();
                    if(Vector2.Distance(P, projection) < 10 
                        && Vector2Tools.IsPointOnSegment(A, B, P))
                    {
                        //Dve uzastopne tacke ne mogu da imaju iste koordinate, treba proveriti kad se pomerajku tacke
                        if(A.X < B.X)
                        {
                            P.Y--;
                        }
                        else if(A.X > B.X)
                        {
                            P.Y++;
                        }
                        else if (A.Y < B.Y)
                        {
                            P.X--;
                        }
                        else
                        {
                            P.X++;
                        }
                        polygon.AddPoint(P, current, current.Next);
                        render();
                        //PolygonRenderer.DrawPoint(projection, g, Color.Red);
                        return;
                    }
                    current = current.Next;
                }
            }
            NewPolygon = true;
            NewPolygonPoint1 = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Vector2 P = new Vector2(e.X, e.Y);

            if (NewPolygon && NewPolygonPoint1 != null && NewPolygonPoint2==null)
            {
                Point A = NewPolygonPoint1.Value;
                Point B = new Point(e.X, e.Y);
                if (Vector2.Distance(new Vector2( A.X, A.Y),new Vector2(B.X, B.Y)) > 20)
                    Polygons.Add(new Polygon(A.X, A.Y, B.X, B.Y));
                NewPolygon = false;
                NewPolygonPoint1 = null;
                render();
            }

            if (MovingPoint != null)
            {
                MovingPoint.Position = P;
                render();
                MovingPoint = null;
                MovingPointPolygon = null;
            }
            if (movingStart)
            {
                start = P;
                render();
                movingStart = false;
            }
            if (movingFinish)
            {
                finish = P;
                render();
                movingFinish = false;
            }
        }

        private void render()
        {
            g.Clear(Color.White);
            foreach(Polygon polygon in Polygons)
            {
                PolygonRenderer.DrawPolygon(polygon, g, pictureBox1.Width, pictureBox1.Height);
            }
            if(NewPolygon && MouseLocation != null && NewPolygonPoint1 != null)
            {
                g.DrawLine(Pens.Red, new Point(NewPolygonPoint1.Value.X, NewPolygonPoint1.Value.Y), (Point)MouseLocation);
            }
            PolygonRenderer.DrawPoint(start, g, Color.Red, 8);
            PolygonRenderer.DrawPoint(finish, g, Color.Blue, 8);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Vector2 P = new Vector2(e.X, e.Y);
            if (NewPolygon && NewPolygonPoint1 != null)
            {
                MouseLocation = new Point(e.X, e.Y);
                render();
            }
            if (MovingPoint != null)
            {
                MovingPoint.Position = P;
                render();
            }
            if (movingStart)
            {
                start = P;
                render();
            }
            if (movingFinish)
            {
                finish = P;
                render();
            }
        }

        private void btnCalculatePath_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CALCULATING PATH");
        }
    }
}