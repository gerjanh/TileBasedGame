using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tileBasedGame
{
    class Game
    {

        Dictionary<Point, Tile> _field = new Dictionary<Point, Tile>();
        Dictionary<Point, Tile> connectingLocation = new Dictionary<Point, Tile>();
        List<Point> FieldEdge = new List<Point>();
        int gameTicks = 0;
        Point screensize;
        Point[] seround = new Point[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
        public Point maxDrag = new Point(200, 80);
        public Point minDrag = new Point(0, 0);


        public Game(int width, int height)
        {
            screensize = new Point(width, height);
            newMaxMinPoints();
        }

        public void ScreensizeChanced(int width, int height)
        {
            screensize = new Point(width, height);
            newMaxMinPoints();
        }

        private void newMaxMinPoints()
        {
            int minx = 0;
            int miny = 0;
            int maxx = 0;
            int maxy = 0;
            foreach (Point p in _field.Keys)
            {
                minx = p.X < minx ? p.X : minx;
                miny = p.Y < miny ? p.Y : miny;
                maxx = p.X > maxx ? p.X : maxx;
                maxy = p.Y > maxy ? p.Y : maxy;
            }

            minx *= Constants.sizeX;
            miny *= Constants.sizeY;
            maxx *= Constants.sizeX;
            maxy *= Constants.sizeY;


            minx += screensize.X / 2;
            miny += screensize.Y / 2;
            maxx -= screensize.X / 2;
            maxy -= screensize.Y / 2;

            maxDrag = new Point(minx, miny);
            maxDrag = new Point(maxx, maxy);
        }

        public void Draw(Graphics g, Point DrawPosition)
        {
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            g.Clear(Color.DarkSeaGreen);
           // g.DrawString(gameTicks + "", drawFont, drawBrush, new PointF(20, 20));


            foreach (Point p in _field.Keys)
            {
               // _field[p].update();
                _field[p].Draw(g, new Point(DrawPosition.X + p.X * Constants.sizeX, DrawPosition.Y + p.Y * Constants.sizeY));
            }
            foreach ( Point pos in connectingLocation.Keys)
            {
             //   connectingLocation[pos].update();
                connectingLocation[pos].Draw(g, new Point(DrawPosition.X + pos.X * Constants.sizeX, DrawPosition.Y + pos.Y * Constants.sizeY),true);
            }
        }

        public void checkIfConnectAnywhere(Tile t)
        {
            connectingLocation.Clear();
            List<uint> directions = new List<uint>();
            foreach (Point loc in FieldEdge)
            {
                directions.Clear();
                uint direction = 0;
                int idx = 0;
                foreach ( Point p in seround)
                {
                    if (_field.Keys.Contains(new Point(loc.X + p.X, loc.Y+p.Y)))
                    {
                        directions.Add(_field[new Point(loc.X + p.X, loc.Y + p.Y)].CanConnect(t, loc, new Point(loc.X + p.X, loc.Y + p.Y)));
                    }
                }
                foreach (uint id in directions)
                {
                    if (idx++ == 0) direction = id;
                    else direction &= id;
                }
                if (direction > 0)
                {
                    Tile newtile = (Tile)t.Clone(); // new Tile(t.tileVals, t.extrapoints);
                    for (int i = 0; i < 4; i++)
                    {
                        uint shifted = (uint)1 << i;
                        if ((direction & shifted) != 0)
                        {
                            // newtile.rotate90deg(true);

                            newtile.rotate90deg(i,false);
                            i = 5;
                        }
                        else
                        {
                        }


                    }
                    connectingLocation.Add(loc, newtile);
                }
            }
        }

        private void newEdge()
        {
            FieldEdge = new List<Point>();
            foreach (Point loc in _field.Keys)
            {
                FieldEdge.Add(new Point(loc.X + 1, loc.Y));
                FieldEdge.Add(new Point(loc.X - 1, loc.Y));
                FieldEdge.Add(new Point(loc.X, loc.Y + 1));
                FieldEdge.Add(new Point(loc.X, loc.Y - 1));
            }
            FieldEdge = new HashSet<Point>(FieldEdge).ToList();

            foreach (Point loc in _field.Keys)
            {
                FieldEdge.Remove(loc);
            }
        }

        public void AddTile(Tile t,Point loc)
        {
            t.update();
            //t.update(loc.X, loc.Y);
            _field.Add(loc, t);
            newEdge();
            checkIfConnectAnywhere(t);
        }

        public void AddTile()
        {

            Tile tile = Constants.getRandomTile();


            Point location = new Point( ((_field.Count / 4) % 80), (_field.Count % 4));
            if (tile != null)
                AddTile(tile, location);


            newMaxMinPoints();
        }

        public void RotateRandom(int chance=5)
        {
            foreach (Point p in connectingLocation.Keys)
            {
                if (Constants.rand.Next(100) > 100- chance)
                {
                    connectingLocation[p].rotate90deg();
                    connectingLocation[p].update();
                }
            }
        }

        public void RotateTile(Point location)
        {
            _field[location].rotate90deg();
            checkIfConnectAnywhere((Tile)_field[location].Clone());
        }

        public void RotateLast()
        {
            RotateTile(_field.Keys.Last());
        }

        public void gametick()
        {

            gameTicks++;

            if (gameTicks % 30==0)
            {
                foreach (Tile t in _field.Values)
                {
                    t.select = false;
                }
                int id = Constants.rand.Next(_field.Count);
                //_field[_field.Keys.ToArray()[id]].select = true;
            }
        }
    }

}
