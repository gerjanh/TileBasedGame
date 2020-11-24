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

        /// <summary>
        /// resize the screen size
        /// </summary>
        /// <param name="width">width of the new screen</param>
        /// <param name="height">height of the new screen</param>
        public void ScreensizeChanced(int width, int height)
        {
            screensize = new Point(width, height);
            newMaxMinPoints();
        }

        /// <summary>
        /// calculate the new max and min points (for scrolling in the screen
        /// </summary>
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

        /// <summary>
        /// the draw method
        /// </summary>
        /// <param name="g">the graphics object to draw on</param>
        /// <param name="DrawPosition">the position you have moved your coördinates to</param>
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

        /// <summary>
        /// generates an updated connectinglocations list 4 directions connected
        /// </summary>
        /// <param name="t"></param>
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
                    Tile newtile = (Tile)t.Clone(); 
                    for (int i = 0; i < 4; i++)
                    {
                        uint shifted = (uint)1 << i;
                        if ((direction & shifted) != 0)
                        {
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

        /// <summary>
        /// generates the edge all tiles 4 way connected to exsisting tiles
        /// </summary>
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

        /// <summary>
        /// add a tile at defined location
        /// </summary>
        /// <param name="t">tile to add</param>
        /// <param name="loc">location to add the tile</param>
        public void AddTile(Tile t,Point loc)
        {
            t.update();
            _field.Add(loc, t);
            newEdge();
            checkIfConnectAnywhere(t);
        }

        /// <summary>
        /// add a random tile on a location defined by the number of existing tiles
        /// </summary>
        public void AddTile()
        {

            Tile tile = Constants.getRandomTile();


            Point location = new Point( ((_field.Count / 4) % 80), (_field.Count % 4));
            if (tile != null)
                AddTile(tile, location);


            newMaxMinPoints();
        }

        /// <summary>
        /// rotate random tiles
        /// </summary>
        /// <param name="chance">chance for each tile to rotate</param>
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


        /// <summary>
        /// rotate tile 90 degrees at location
        /// </summary>
        /// <param name="location">location to rotate</param>
        public void RotateTile(Point location)
        {
            _field[location].rotate90deg();
            checkIfConnectAnywhere((Tile)_field[location].Clone());
        }

        /// <summary>
        /// rotate the latest added tile 90 degrees
        /// </summary>
        public void RotateLast()
        {
            RotateTile(_field.Keys.Last());
        }

        /// <summary>
        /// preform a gametick
        /// </summary>
        public void gametick()
        {

            gameTicks++;

            if (gameTicks % 30==0)
            {
                foreach (Tile t in _field.Values)
                {
                    t.select = false;
                }
            }
        }
    }

}
