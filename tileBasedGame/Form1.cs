using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace tileBasedGame
{
    /*
     * 
     * add to designer if panel starts flickering 
    typeof(System.Windows.Forms.Panel).InvokeMember("DoubleBuffered",
    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
    null, panel1, new object[] { true });
     */
    public partial class Form1 : Form
    {
        /// <summary>
        /// timer for the frames (fps)
        /// </summary>
        Timer drawFrameTimer = new Timer() { Interval = 1000 / 30 };
        /// <summary>
        /// timer for the game ticks
        /// </summary>
        Timer gameTickTimer = new Timer() { Interval = 1000 / 5 };
        /// <summary>
        /// timer for the drag ticks
        /// </summary>
        Timer dragTimer = new Timer() { Interval = 100 };
        // to alterate the painging frame
        Boolean frameId = true;
        // 2 frames to alter the painting canvas
        private Bitmap frame1, frame2;
        //number of frames for debug use
        int frameNum =0;
        //last drag locations
        Point lastDragPoint;
        /// <summary>
        /// randomizer undifined seed
        /// </summary>
        //Random rand = new Random();
        Game game ;
        Constants c = new Constants();
        Point movedPosition = new Point(0, 0);

        List<Button> Buttons = new List<Button>();


        Font drawFont = new Font("Arial", 16);
        SolidBrush drawBrush = new SolidBrush(Color.Black);

        public Form1()
        {
            
            InitializeComponent();
            game = new Game(panel1.Width, panel1.Height);
            Size s = new Size(80, 30);
            Place addbutton = new Place(new Point(panel1.Width - s.Width, 0), s, "add");
            addbutton.doaction += new EventHandler(AddButtonClick);
            Buttons.Add(addbutton);

            Size b = new Size(80, 30);
            Place RotateButton = new Place(new Point(panel1.Width - b.Width-s.Width, 0), b, "rotate");
            RotateButton.doaction += new EventHandler(RotateButtonClick);
            Buttons.Add(RotateButton);

            Constants.GenerateDeck();
          //  Constants.ShuffleDeck();
            drawFrameTimer.Elapsed += new ElapsedEventHandler(drawFrame);
            gameTickTimer.Elapsed += new ElapsedEventHandler(GameTick);
            dragTimer.Elapsed += new ElapsedEventHandler(DragTick);
            frame1 = new Bitmap(panel1.Size.Width, panel1.Size.Height);
            frame2 = new Bitmap(panel1.Size.Width, panel1.Size.Height);
            drawFrameTimer.Start();
            gameTickTimer.Start();
        }

        private void RotateButtonClick(object sender, EventArgs e)
        {
            game.RotateLast();
            //game.RotateRandom(10);
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            game.AddTile();
        }

        private void DragTick(object sender, ElapsedEventArgs e)
        {
            Point thispoint = c.getPoint();
            Point movedPoint = new Point(thispoint.X - lastDragPoint.X, thispoint.Y - lastDragPoint.Y);
            movedPosition = new Point(movedPosition.X + movedPoint.X, movedPosition.Y + movedPoint.Y);

            movedPosition = movedPosition.X < game.minDrag.X ? new Point(game.minDrag.X, movedPosition.Y) : movedPosition;
            movedPosition = movedPosition.Y < game.minDrag.Y ? new Point(movedPosition.X, game.minDrag.Y) : movedPosition;
            movedPosition = movedPosition.X > game.maxDrag.X ? new Point(game.maxDrag.X, movedPosition.Y) : movedPosition;
            movedPosition = movedPosition.Y > game.maxDrag.Y ? new Point(movedPosition.X, game.maxDrag.Y) : movedPosition;

            lastDragPoint = thispoint;
            movedPosition = new Point(0, 0);
        }

        /// <summary>
        /// timer game tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTick(object sender, ElapsedEventArgs e)
        {
            game.gametick();
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            StartDrag();
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            StopDrag();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            StopDrag();
        }

        private void StartDrag()
        {

            lastDragPoint = c.getPoint();
            dragTimer.Start();
        }
        private void StopDrag()
        {
            lastDragPoint = c.getPoint();
            dragTimer.Stop();
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (Button b in Buttons)
            {
                b.CheckClick(e.Location);
            }
        }



        /// <summary>
        /// timer draws frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawFrame(object sender, ElapsedEventArgs e)
        {
            Bitmap frame;
            int tilenum = frameNum / 15;
            if (frameId)
            {
                frameId = !frameId;
                frame = frame1;
            }
            else
            {
                frameId = !frameId;
                frame = frame2;
            }//*/



            using (Graphics g = Graphics.FromImage(frame))
            {

                game.Draw(g,movedPosition);
#if DEBUG
           //     g.DrawString(movedPosition+"", drawFont, drawBrush, new PointF(20, 20)); 
#endif
                foreach(Button b in Buttons)
                {
                    b.Draw(g);
                }
            }

            panel1.BackgroundImage = frame;

            frameNum++;
        }
    }
}
