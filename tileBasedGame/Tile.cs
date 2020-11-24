using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tileBasedGame
{
    class Tile : ICloneable
    {

        Pen GrasPen = new Pen(Brushes.ForestGreen);
        Pen PenPath = new Pen(Brushes.WhiteSmoke, 10);
        Pen PenOutline = new Pen(Brushes.Black, 1);
        SolidBrush brushGrass = new SolidBrush(Color.ForestGreen);
        SolidBrush brushCity = new SolidBrush(Color.SaddleBrown);
        SolidBrush brushPath = new SolidBrush(Color.WhiteSmoke);
        SolidBrush brushwater = new SolidBrush(Color.CornflowerBlue);
        SolidBrush brushKlooster = new SolidBrush(Color.SandyBrown);
        SolidBrush brushGold = new SolidBrush(Color.Gold);
        SolidBrush brushGlow = new SolidBrush(Color.FromArgb(100,Color.Orange));

        public int[] tileVals;
        int MiddleShapeSize = 10;
        List<Point[]> triangles = new List<Point[]>();
        Rectangle rect = new Rectangle(0, 0, Constants.sizeX, Constants.sizeY);
        Point[] middleShape = new Point[8];
        Point[] middleShapeSmall = new Point[8];
        Bitmap img = new Bitmap(Constants.sizeX, Constants.sizeY);
        bool creatingNewBitmap = false;
        public bool select { get; set; } = false;
        public Boolean extrapoints { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="tilevals">values of the tile parts</param>
        /// <param name="extrapoints">has the tile a shield/bonus points mark</param>
        public Tile( int[] tilevals, Boolean extrapoints)
        {
            this.tileVals = tilevals;
            this.extrapoints = extrapoints;
            update();
        }

        /// <summary>
        /// setup the tile triangles + middle part + road lines
        /// </summary>
        public void update()
        {
            triangles.Clear();
            middleShape.Clone();

            int __x = 0 ;
            int __y = 0 ;

            int maxx = __x + Constants.sizeX;
            int maxy = __y + Constants.sizeY;
            int midx = __x + Constants.sizeX / 2;
            int midy = __y + Constants.sizeY / 2;

            Point[] triangle = new Point[3];// top
            triangle[0] = new Point(__x, __y);
            triangle[1] = new Point(maxx, __y);
            triangle[2] = new Point(midx, midy);
            triangles.Add(triangle);
            triangle = new Point[3];//right
            triangle[0] = new Point(maxx, __y);
            triangle[1] = new Point(maxx, maxy);
            triangle[2] = new Point(midx, midy);
            triangles.Add(triangle);
            triangle = new Point[3];//bottom
            triangle[0] = new Point(maxx, maxy);
            triangle[1] = new Point(__x, maxy);
            triangle[2] = new Point(midx, midy);
            triangles.Add(triangle);
            triangle = new Point[3];// left
            triangle[0] = new Point(__x, maxy);
            triangle[1] = new Point(__x, __y);
            triangle[2] = new Point(midx, midy);
            triangles.Add(triangle);

            triangle = new Point[2];
            triangle[0] = new Point(midx, midy);
            triangle[1] = new Point(midx, __y);
            triangles.Add(triangle);
            triangle = new Point[2];
            triangle[0] = new Point(midx, midy);
            triangle[1] = new Point(maxx, midy);
            triangles.Add(triangle);
            triangle = new Point[2];
            triangle[0] = new Point(midx, midy);
            triangle[1] = new Point(midx, maxy);
            triangles.Add(triangle);
            triangle = new Point[2];
            triangle[0] = new Point(midx, midy);
            triangle[1] = new Point(__x, midy);
            triangles.Add(triangle);
            rect = new Rectangle(__x, __y, maxx, maxy);

            int size = MiddleShapeSize * Constants.scale;
            middleShape[0] = new Point(midx + 0, midy + size);
            middleShape[1] = new Point(midx + size/4*3, midy + size/4*3);
            middleShape[2] = new Point(midx + size, midy + 0);
            middleShape[3] = new Point(midx + size / 4 * 3, midy - size / 4 * 3);
            middleShape[4] = new Point(midx + 0, midy - size);
            middleShape[5] = new Point(midx - size / 4 * 3, midy - size / 4 * 3);
            middleShape[6] = new Point(midx - size, midy + 0);
            middleShape[7] = new Point(midx - size / 4 * 3, midy + size / 4 * 3);


            int sizesmall = MiddleShapeSize/2 * Constants.scale;
            middleShapeSmall[0] = new Point(midx + 0, midy + sizesmall);
            middleShapeSmall[1] = new Point(midx + sizesmall / 4 * 3, midy + sizesmall / 4 * 3);
            middleShapeSmall[2] = new Point(midx + sizesmall, midy + 0);
            middleShapeSmall[3] = new Point(midx + sizesmall / 4 * 3, midy - sizesmall / 4 * 3);
            middleShapeSmall[4] = new Point(midx + 0, midy - sizesmall);
            middleShapeSmall[5] = new Point(midx - sizesmall / 4 * 3, midy - sizesmall / 4 * 3);
            middleShapeSmall[6] = new Point(midx - sizesmall, midy + 0);
            middleShapeSmall[7] = new Point(midx - sizesmall / 4 * 3, midy + sizesmall / 4 * 3);
            
            CreateBitmap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g">graphics object</param>
        /// <param name="p">location where to paint on the graphics object</param>
        public void Draw(Graphics g, Point p, bool highlighted = false)
        {

            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            if (!creatingNewBitmap)
            g.DrawImage(img, new Point(p.X, p.Y ));
            if (select|| highlighted)
            g.FillRectangle(brushGlow, new RectangleF(p, img.Size));
            Point loc = new Point(p.X + Constants.sizeX / 2, p.Y + Constants.sizeY / 2);
            g.DrawString(tileVals[0] + " ", drawFont, drawBrush, new Point(loc.X, loc.Y - Constants.sizeY / 4));
            g.DrawString(tileVals[1] + " ", drawFont, drawBrush, new Point(loc.X + Constants.sizeX / 4, loc.Y));
            g.DrawString(tileVals[2] + " ", drawFont, drawBrush, new Point(loc.X, loc.Y + Constants.sizeY / 4));
            g.DrawString(tileVals[3] + " ", drawFont, drawBrush, new Point(loc.X - Constants.sizeX / 4, loc.Y));
        }

        /// <summary>
        /// create the bitmap for this tile to draw it more easely
        /// </summary>
        private void CreateBitmap()
        {
            creatingNewBitmap = true;
            Graphics g = Graphics.FromImage(img);
            Boolean streets = false;

            for (int i = 0; i < 4; i++)
            {
                g.FillPolygon(brushGrass, triangles[i]);
                int val = tileVals[i];
                Point[] p = triangles[i];

                switch (val)
                {
                    case 1:
                        g.DrawLine(PenPath, triangles[4 + i][0], triangles[4 + i][1]);
                        streets = true;
                        break;
                    case 2:
                        g.FillPolygon(brushCity, p);
                        break;
                    default:
                        break;
                }

            }

            int size = 16;
            switch (tileVals[4])
            {
                case 0:
                    g.FillEllipse(brushGrass, triangles[5][0].X - size / 2, triangles[5][0].Y - size / 2, size, size);
                    if (streets)
                    g.DrawEllipse(PenOutline, triangles[5][0].X - size / 2, triangles[5][0].Y - size / 2, size, size);
                    break;
                case 1:
                    g.FillEllipse(brushPath, triangles[5][0].X - size / 2, triangles[5][0].Y - size / 2, size, size);
                    break;
                case 2:
                    g.FillEllipse(brushCity, triangles[5][0].X - size / 2, triangles[5][0].Y - size / 2, size, size);
                    break;
                case 3:
                    g.FillEllipse(brushPath, triangles[5][0].X - size / 2+5, triangles[5][0].Y - size / 2+5, size, size);
                    g.FillEllipse(brushCity, triangles[5][0].X - size / 2, triangles[5][0].Y - size / 2, size, size);
                    break;
                case 9:
                    g.FillPolygon(brushKlooster, middleShape);
                    break;
                default:
                    break;
            }
            if (extrapoints)
            {
                g.FillPolygon(brushGold, middleShapeSmall);
            }
            creatingNewBitmap = false;

        }

        /// <summary>
        /// the tile values and image rotate 90 degrees
        /// </summary>
        /// <param name="clockwise">rotation direction clockwise or counter clockwise</param>
        public void rotate90deg(int turns = 1 , Boolean clockwise = true)
        {
            int[] tmpvals = new int[4];
            tmpvals[0] = tileVals[0];
            tmpvals[1] = tileVals[1];
            tmpvals[2] = tileVals[2];
            tmpvals[3] = tileVals[3];

            tileVals[(0 + turns) % 4] = tmpvals[0];
            tileVals[(1 + turns) % 4] = tmpvals[1];
            tileVals[(2 + turns) % 4] = tmpvals[2];
            tileVals[(3 + turns) % 4] = tmpvals[3];

            /*
            if (clockwise)
            {
                int hold = tileVals[0];
                tileVals[0] = tileVals[1];
                tileVals[1] = tileVals[2];
                tileVals[2] = tileVals[3];
                tileVals[3] = hold;
            }
            else
            {
                int hold = tileVals[3];
                tileVals[3] = tileVals[2];
                tileVals[2] = tileVals[1];
                tileVals[1] = tileVals[0];
                tileVals[0] = hold;
            }*/
            update();
        }

        public uint CanConnect(Tile t,Point poscheck,Point posThis)
        {
            uint canConnect = 0;
            int orientation = 0;
            int thisOrientation = 0;

            orientation =
                poscheck.X < posThis.X ? 1 :
                poscheck.Y < posThis.Y ? 2 :
                poscheck.X > posThis.X ? 3 :
                poscheck.Y > posThis.Y ? 0 :
                0;
            thisOrientation = (orientation + 2) % 4;


            int check = 0;
            for(int i = 0; i < 4; i++)
            {
                check += (int) Math.Pow(2, i);
                if (t.tileVals[(orientation + i)%4] == tileVals[thisOrientation])
                {
                    uint id = (uint) Math.Pow(2, i);
                    canConnect += id;
                }
            }

            return canConnect;
        }

        public object Clone()
        {

            return new Tile((int[])tileVals.Clone(),extrapoints);
        }
    }
}
