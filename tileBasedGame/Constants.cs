using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tileBasedGame
{
    class Constants
    {


        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        Point _p;



        static List<Tile> shoffledDeck = new List<Tile>();
        static List<Tile> unusedDeck= new List<Tile>();
        static List<Tile> deck = new List<Tile>();
        public static Random rand = new Random();
        public static int sizeX { get; } = 50;
        public static int sizeY { get; } = 50;
        public static int scale { get; set; } = 1;

        /// <summary>
        /// shuffles the deck into shoffledDeck and coppys that to unusedDeck
        /// </summary>
        public static void ShuffleDeck()
        {
            List<Tile> nonShuffledDeck = deck;
            shoffledDeck = new List<Tile>();


            while (nonShuffledDeck.Count > 0)
            {
                int num = rand.Next(nonShuffledDeck.Count - 1);
                Tile t = nonShuffledDeck[num];
                int rando = rand.Next(4);
                for (int i = 0; i < rando; i++)
                    t.rotate90deg();
                shoffledDeck.Add(t);
                nonShuffledDeck.RemoveAt(num);
            }
            unusedDeck = shoffledDeck;
        }

        /// <summary>
        /// get a random unused Tile
        /// </summary>
        /// <returns>a random tile from the unused Tile list</returns>
        public static Tile getRandomTile()
        {
            Tile ret = null;
            if (unusedDeck.Count > 1)
            {
                int rando = rand.Next(unusedDeck.Count - 1);
                ret = unusedDeck[rando];
                unusedDeck.RemoveAt(rando);
            }
            else if (unusedDeck.Count == 1)
            {
                ret = unusedDeck[0];
                unusedDeck.RemoveAt(0);
            }

            return ret;
        }

        /// <summary>
        /// generate a dack of tiles from ..\..\tiles\tiles.txt
        /// </summary>
        public static void GenerateDeck()
        {
            deck = new List<Tile>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\tiles\tiles.txt");
            string[] lines = System.IO.File.ReadAllLines(path);

            foreach (string line in lines)
            {
                int number;
                if(int.TryParse(line, out number))
                {
                    int amount = number % 10;
                    int shielded = (number % 100) / 10;
                    number /= 100;
                    for(int i = 0; i < amount; i++)
                    {
                        int[] tilevals = new int[5];
                        tilevals[4] = (number % 10);
                        tilevals[3] = (number % 100) / 10;
                        tilevals[2] = (number % 1000) / 100;
                        tilevals[1] = (number % 10000) / 1000;
                        tilevals[0] = (number % 100000) / 10000;

                        Boolean extrpoints = i < shielded;
                        Tile t = new Tile(tilevals, extrpoints);
                        deck.Add(t);
                    }
                }
            }
            unusedDeck = deck;
        }



        public static bool CursorPos(ref Point lpPoint)
        {
            try
            {
                GetCursorPos(ref lpPoint);
                return true;
            }
            catch (Exception er)
            {
                string err = er.Message;
                return false;
            }
        }

        public Point getPoint()
        {
            CursorPos(ref _p);
            return _p;
        }

    }

    abstract class Button
    {
        public  Point _location { get; set; }
        public  Size _size { get; set; }
        public  string _buttonText { get; set; }

        public event EventHandler doaction;
        public bool CheckClick(Point p)
        {
            if (p.X > _location.X && p.X < _location.X + _size.Width)
                if (p.Y > _location.Y && p.Y < _location.Y + _size.Height)
                {
                    EventHandler handler = doaction;
                    if (null != handler) handler(this, EventArgs.Empty);
                   // doaction();
                    return true;
                }

            return false;
        }


        //public void OnJump()
       // {
      //  }

        public abstract void Draw(Graphics g);

      //  public abstract void Doaction();
    }

    class Place : Button
    {
        readonly int TextSize = 16;
        Font DrawFont;
        readonly SolidBrush DrawBrush = new SolidBrush(Color.Black);
        readonly Pen PenOutline = new Pen(Brushes.Black, 1);
        public Place(Point loc, Size size, string text)
        {
            _location = loc;
            _size = size;
            _buttonText = text;
            DrawFont = new Font("Arial", TextSize);
        }


        public override void Draw(Graphics g)
        {

            g.DrawRectangle(PenOutline, _location.X, _location.Y, _size.Width, _size.Height);
            g.DrawString(_buttonText, DrawFont, DrawBrush, new Point(_location.X + _size.Width / 2 - 8, _location.Y + _size.Height / 2 - TextSize / 2));
        }
    }
}
